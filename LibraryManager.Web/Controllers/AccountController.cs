using Microsoft.AspNetCore.Mvc;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Services.Interfaces;
using LibraryManager.Web.Models;

namespace LibraryManager.Web.Controllers;

public class AccountController : Controller
{
    private readonly ICustomerService _customerService;

    public AccountController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("CustomerId") != null)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var customer = await _customerService.LoginAsync(model.Username, model.Password);
        
        if (customer == null)
        {
            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        if (customer.CardStatus != "Active")
        {
            ModelState.AddModelError("", "Your library card is disabled. Please contact admin.");
            return View(model);
        }

        HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
        HttpContext.Session.SetString("CustomerName", customer.FullName);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("CustomerId") != null)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
            return View(model);
        }

        try
        {
            var customer = new Customer
            {
                Username = model.Username,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                CardStatus = "Active"
            };

            await _customerService.AddCustomerAsync(customer, model.Password);
            
            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
