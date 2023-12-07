using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private BookstoreContext context;
        public CartController(BookstoreContext ctx) => context = ctx;

        [HttpGet]
        public ViewResult Index()
        {
            var session = new BookSession(HttpContext.Session);

            int? count = session.GetMyBookCount();
            if (!count.HasValue)
            {
                var cookies = new BookCookies(Request.Cookies);
                string[] ids = cookies.GetMyBooksIds();

                if (ids.Length > 0)
                {
                    var mybooks = context.Books
                        .Where(t => ids.Contains(t.BookId.ToString())).ToList();
                    session.SetMyBooks(mybooks);
                }
            }


            var model = new BookListViewModel
            {
                Books = session.GetMyBooks()
            };
            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult Add(int id)
        {
            Book book = context.Books
                .Where(t => t.BookId == id)
                .FirstOrDefault() ?? new Book();

            var session = new BookSession(HttpContext.Session);
            var cookies = new BookCookies(Response.Cookies);
            var books = session.GetMyBooks();
            books.Add(book);
            session.SetMyBooks(books);
            cookies.SetMyBookIds(books);

            TempData["message"] =
                $"{book.Title} added to your cart";
            return RedirectToAction("Index", "Book");


        }

        [HttpPost]
        public RedirectToActionResult Delete()
        {
            var session = new BookSession(HttpContext.Session);
            var cookies = new BookCookies(Response.Cookies);

            session.RemoveMyBooks();
            cookies.RemoveMyBookIds();



            TempData["message"] = "Cart cleared";
            return RedirectToAction("Index", "Book");
        }



    }
}