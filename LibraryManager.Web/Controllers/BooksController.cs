using Microsoft.AspNetCore.Mvc;
using LibraryManager.Core.Services.Interfaces;

namespace LibraryManager.Web.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Account");

        var books = string.IsNullOrWhiteSpace(search)
            ? await _bookService.GetAllBooksAsync()
            : await _bookService.SearchBooksAsync(search);

        ViewBag.Search = search;
        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Account");

        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound();

        return View(book);
    }
}
