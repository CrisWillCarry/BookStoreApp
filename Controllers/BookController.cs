using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreApp.Controllers
{
  
    public class BookController : Controller
    {

        private BookstoreContext context { get; set; }
        public BookController(BookstoreContext ctx) =>  context = ctx;

        [AllowAnonymous]
        public IActionResult Index(int page = 1, int pageSize = 5, int selectedGenreId = 0)
        {
			int skip = (page - 1) * pageSize;
			var genres = context.Genres.ToList();

			// Search the books based on the selected genre.
			var booksQuery = context.Books.Include(m => m.Genre).Include(m => m.authorObject).OrderBy(m => m.Title);
			if (selectedGenreId != 0)
			{
				booksQuery = (IOrderedQueryable<Book>)booksQuery.Where(b => b.GenreId == selectedGenreId);
			}
			var totalCount = booksQuery.Count();
			var books = booksQuery.Skip(skip).Take(pageSize).ToList();

			var pagedList = new PagedList<Book>
			{
				Items = books,
				PageNumber = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};

			var viewModel = new BookViewModel
			{
				PageResult = pagedList,
				Genres = new SelectList(genres, "GenreId", "Name"),
				SelectedGenreId = selectedGenreId
			};

			return View(viewModel);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var bookToEdit = context.Books.Find(id);

            ViewBag.Authors = context.Authors.ToList();
            ViewBag.Genres = context.Genres.ToList();

            return View(bookToEdit);
        }


        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Update(Book book)
        {
            if (ModelState.IsValid)
            {
                var existingGenre = context.Genres.FirstOrDefault(g => g.Name == book.Genre.Name);
                var existingAuthor = context.Authors.FirstOrDefault(a => a.AuthorId == book.authorObject.AuthorId);

                if (existingGenre != null)
                {
                    // Genre already exists, update the book's genre reference
                    book.GenreId = existingGenre.GenreId;
                    book.Genre = null; // Set Genre to null to avoid EF Core confusion in tracking
                }
                else
                {
                    // Genre doesn't exist, consider adding error handling or informing the user
                    ViewBag.ErrorMessage = "The provided genre does not exist in the database.";
                    return View("Edit", book);
                }

                if (existingAuthor != null)
                {
                    // Author already exists, update the book's author reference
                    book.AuthorId = existingAuthor.AuthorId;
                    book.authorObject = null; // Set authorObject to null to avoid EF Core confusion in tracking
                }
                else
                {
                    
                    ViewBag.ErrorMessage = "The provided author does not exist in the database.";
                    return View("Edit", book);
                }

                context.Books.Update(book);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "Oops! Something went wrong. Please check the entered information.";
                return View("Edit", book);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var bookToDelete = context.Books.Find(id);

            return View(bookToDelete);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            var bookToDelete = context.Books.Find(id);

            if (bookToDelete != null)
            {
                context.Books.Remove(bookToDelete);
                context.SaveChanges();
            }

            return RedirectToAction("Index", "Book");
        }
    }
}
