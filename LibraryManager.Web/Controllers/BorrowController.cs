using Microsoft.AspNetCore.Mvc;
using LibraryManager.Core.Services.Interfaces;

namespace LibraryManager.Web.Controllers;

public class BorrowController : Controller
{
    private readonly IBorrowService _borrowService;
    private readonly IBookService _bookService;

    public BorrowController(IBorrowService borrowService, IBookService bookService)
    {
        _borrowService = borrowService;
        _bookService = bookService;
    }

    public async Task<IActionResult> MyBorrows()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Account");

        var borrows = await _borrowService.GetBorrowsByCustomerIdAsync(customerId.Value);
        return View(borrows);
    }

    [HttpPost]
    public async Task<IActionResult> Request(int bookId)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Account");

        try
        {
            await _borrowService.RequestBorrowAsync(customerId.Value, bookId);
            TempData["Success"] = "Borrow request submitted successfully! Wait for admin approval.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Details", "Books", new { id = bookId });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int borrowId)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Account");

        try
        {
            await _borrowService.CancelBorrowAsync(borrowId);
            TempData["Success"] = "Borrow request cancelled.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("MyBorrows");
    }

    [HttpPost]
    public async Task<IActionResult> RequestExtension(int borrowId)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Account");

        try
        {
            await _borrowService.ExtendBorrowAsync(borrowId, 7); // Extend 7 days
            TempData["Success"] = "Extension request submitted (7 days).";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("MyBorrows");
    }
}
