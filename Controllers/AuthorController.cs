using BookStoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class AuthorController : Controller
{
    private readonly BookstoreContext _context;

    public AuthorController(BookstoreContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public IActionResult Index(int page = 1, int pageSize = 5, string search = "")
    {
        int skip = (page - 1) * pageSize;

        var authorsQuery = _context.Authors.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            authorsQuery = authorsQuery.Where(a => a.FirstName.Contains(search) || a.LastName.Contains(search));
        }

        var totalCount = authorsQuery.Count();
        var authors = authorsQuery.Skip(skip).Take(pageSize).ToList();

        var pagedList = new PagedList<Author>
        {
            Items = authors,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        var viewModel = new AuthorViewModel
        {
            PageResult = pagedList,
            Search = search
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create(Author author)
    {
        if (ModelState.IsValid)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }

    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        var author = _context.Authors.Find(id);

        if (author == null)
        {
            return NotFound();
        }

        author.Books = _context.Books.Where(b => b.AuthorId == id).ToList();

        return View(author);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var author = _context.Authors.Find(id);
        if (author == null)
        {
            return NotFound();
        }

        return View(author);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Author author)
    {
        if (ModelState.IsValid)
        {
            var existingAuthor = _context.Authors.Include(a => a.Books).FirstOrDefault(a => a.AuthorId == author.AuthorId);
            if (existingAuthor == null)
            {
                return NotFound();
            }

            existingAuthor.FirstName = author.FirstName;
            existingAuthor.LastName = author.LastName;

            foreach (var book in existingAuthor.Books)
            {
                book.authorObject = existingAuthor;
                _context.Books.Update(book);
            }

            _context.Authors.Update(existingAuthor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        return View(author);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var author = _context.Authors.Include(a => a.Books).FirstOrDefault(a => a.AuthorId == id);
        if (author == null)
        {
            return NotFound();
        }

        return View(author);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var author = _context.Authors.Include(a => a.Books).FirstOrDefault(a => a.AuthorId == id);
        if (author == null)
        {
            return NotFound();
        }

        _context.Authors.Remove(author);

        _context.Books.RemoveRange(author.Books);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public class AuthorViewModel
    {
        public PagedList<Author> PageResult { get; set; }
        public string Search { get; set; }
    }
}
