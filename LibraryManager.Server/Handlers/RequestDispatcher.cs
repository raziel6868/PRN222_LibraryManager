using System.Text.Json;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Services.Interfaces;
using LibraryManager.Server.Models;

namespace LibraryManager.Server.Handlers;

// Helper classes for simple requests
public class IdRequest { public int Id { get; set; } }
public class LoginRequest { public string UserName { get; set; } = ""; public string Password { get; set; } = ""; }
public class BookSearchRequest { public string? Keyword { get; set; } public int? CategoryId { get; set; } public int? AuthorId { get; set; } }
public class BorrowRequestData { public int CustomerId { get; set; } public int BookId { get; set; } public bool CreatedByCustomer { get; set; } }
public class ApproveBorrowRequest { public int BorrowId { get; set; } public int StaffId { get; set; } public int DaysToLend { get; set; } = 14; }
public class ReturnBookRequest { public int BorrowId { get; set; } public int StaffId { get; set; } }
public class ExtendRequest { public int BorrowId { get; set; } public int AdditionalDays { get; set; } }

public class RequestDispatcher
{
    private readonly IBookService _bookService;
    private readonly ICustomerService _customerService;
    private readonly IBorrowService _borrowService;
    private readonly IStaffService _staffService;

    public RequestDispatcher(
        IBookService bookService,
        ICustomerService customerService,
        IBorrowService borrowService,
        IStaffService staffService)
    {
        _bookService = bookService;
        _customerService = customerService;
        _borrowService = borrowService;
        _staffService = staffService;
    }

    public async Task<ApiResponse> DispatchAsync(ApiRequest request)
    {
        try
        {
            return request.Action switch
            {
                // Staff actions
                "Staff.Login" => await HandleStaffLoginAsync(request.Data),

                // Book actions
                "Book.GetAll" => await HandleGetAllBooksAsync(),
                "Book.GetById" => await HandleGetBookByIdAsync(request.Data),
                "Book.Search" => await HandleSearchBooksAsync(request.Data),
                "Book.Add" => await HandleAddBookAsync(request.Data),
                "Book.Update" => await HandleUpdateBookAsync(request.Data),
                "Book.Delete" => await HandleDeleteBookAsync(request.Data),

                // Category actions
                "Category.GetAll" => await HandleGetAllCategoriesAsync(),
                "Category.Add" => await HandleAddCategoryAsync(request.Data),
                "Category.Update" => await HandleUpdateCategoryAsync(request.Data),
                "Category.Delete" => await HandleDeleteCategoryAsync(request.Data),

                // Author actions
                "Author.GetAll" => await HandleGetAllAuthorsAsync(),
                "Author.Add" => await HandleAddAuthorAsync(request.Data),
                "Author.Update" => await HandleUpdateAuthorAsync(request.Data),
                "Author.Delete" => await HandleDeleteAuthorAsync(request.Data),

                // Customer actions
                "Customer.GetAll" => await HandleGetAllCustomersAsync(),
                "Customer.GetById" => await HandleGetCustomerByIdAsync(request.Data),
                "Customer.Add" => await HandleAddCustomerAsync(request.Data),
                "Customer.Update" => await HandleUpdateCustomerAsync(request.Data),
                "Customer.ActivateCard" => await HandleActivateCardAsync(request.Data),
                "Customer.DeactivateCard" => await HandleDeactivateCardAsync(request.Data),
                "Customer.Login" => await HandleCustomerLoginAsync(request.Data),

                // Borrow actions
                "Borrow.GetAll" => await HandleGetAllBorrowsAsync(),
                "Borrow.GetByCustomerId" => await HandleGetBorrowsByCustomerIdAsync(request.Data),
                "Borrow.GetBorrowed" => await HandleGetBorrowedBooksAsync(),
                "Borrow.GetOverdue" => await HandleGetOverdueBooksAsync(),
                "Borrow.GetRequested" => await HandleGetRequestedBorrowsAsync(),
                "Borrow.Request" => await HandleRequestBorrowAsync(request.Data),
                "Borrow.Approve" => await HandleApproveBorrowAsync(request.Data),
                "Borrow.Return" => await HandleReturnBookAsync(request.Data),
                "Borrow.Extend" => await HandleExtendDueDateAsync(request.Data),
                "Borrow.Cancel" => await HandleCancelBorrowAsync(request.Data),

                _ => ApiResponse.FailResponse($"Unknown action: {request.Action}")
            };
        }
        catch (Exception ex)
        {
            return ApiResponse.FailResponse(ex.Message);
        }
    }

    // Staff Handlers
    private async Task<ApiResponse> HandleStaffLoginAsync(JsonElement data)
    {
        var dto = JsonSerializer.Deserialize<LoginRequest>(data.GetRawText());
        if (dto == null) return ApiResponse.FailResponse("Invalid data");

        var staff = await _staffService.LoginAsync(dto.UserName, dto.Password);
        if (staff == null) return ApiResponse.FailResponse("Invalid username or password");

        return ApiResponse.SuccessResponse(staff);
    }

    // Book Handlers
    private async Task<ApiResponse> HandleGetAllBooksAsync()
    {
        var books = await _bookService.GetAllBooksAsync();
        return ApiResponse.SuccessResponse(books);
    }

    private async Task<ApiResponse> HandleGetBookByIdAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        var book = await _bookService.GetBookByIdAsync(req.Id);
        if (book == null) return ApiResponse.FailResponse("Book not found");

        return ApiResponse.SuccessResponse(book);
    }

    private async Task<ApiResponse> HandleSearchBooksAsync(JsonElement data)
    {
        var dto = JsonSerializer.Deserialize<BookSearchRequest>(data.GetRawText());
        if (dto == null) return ApiResponse.FailResponse("Invalid data");

        var books = await _bookService.SearchBooksAsync(dto.Keyword, dto.CategoryId, dto.AuthorId);
        return ApiResponse.SuccessResponse(books);
    }

    private async Task<ApiResponse> HandleAddBookAsync(JsonElement data)
    {
        var book = JsonSerializer.Deserialize<Book>(data.GetRawText());
        if (book == null) return ApiResponse.FailResponse("Invalid data");

        await _bookService.AddBookAsync(book);
        return ApiResponse.SuccessResponse(null, "Book added successfully");
    }

    private async Task<ApiResponse> HandleUpdateBookAsync(JsonElement data)
    {
        var book = JsonSerializer.Deserialize<Book>(data.GetRawText());
        if (book == null) return ApiResponse.FailResponse("Invalid data");

        await _bookService.UpdateBookAsync(book);
        return ApiResponse.SuccessResponse(null, "Book updated successfully");
    }

    private async Task<ApiResponse> HandleDeleteBookAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        await _bookService.DeleteBookAsync(req.Id);
        return ApiResponse.SuccessResponse(null, "Book deleted successfully");
    }

    // Category Handlers
    private async Task<ApiResponse> HandleGetAllCategoriesAsync()
    {
        var categories = await _bookService.GetAllCategoriesAsync();
        return ApiResponse.SuccessResponse(categories);
    }

    private async Task<ApiResponse> HandleAddCategoryAsync(JsonElement data)
    {
        var category = JsonSerializer.Deserialize<Category>(data.GetRawText());
        if (category == null) return ApiResponse.FailResponse("Invalid data");

        await _bookService.AddCategoryAsync(category);
        return ApiResponse.SuccessResponse(null, "Category added successfully");
    }

    private async Task<ApiResponse> HandleUpdateCategoryAsync(JsonElement data)
    {
        var category = JsonSerializer.Deserialize<Category>(data.GetRawText());
        if (category == null) return ApiResponse.FailResponse("Invalid data");

        await _bookService.UpdateCategoryAsync(category);
        return ApiResponse.SuccessResponse(null, "Category updated successfully");
    }

    private async Task<ApiResponse> HandleDeleteCategoryAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        await _bookService.DeleteCategoryAsync(req.Id);
        return ApiResponse.SuccessResponse(null, "Category deleted successfully");
    }

    // Author Handlers
    private async Task<ApiResponse> HandleGetAllAuthorsAsync()
    {
        var authors = await _bookService.GetAllAuthorsAsync();
        return ApiResponse.SuccessResponse(authors);
    }

    private async Task<ApiResponse> HandleAddAuthorAsync(JsonElement data)
    {
        var author = JsonSerializer.Deserialize<Author>(data.GetRawText());
        if (author == null) return ApiResponse.FailResponse("Invalid data");

        await _bookService.AddAuthorAsync(author);
        return ApiResponse.SuccessResponse(null, "Author added successfully");
    }

    private async Task<ApiResponse> HandleUpdateAuthorAsync(JsonElement data)
    {
        var author = JsonSerializer.Deserialize<Author>(data.GetRawText());
        if (author == null) return ApiResponse.FailResponse("Invalid data");

        await _bookService.UpdateAuthorAsync(author);
        return ApiResponse.SuccessResponse(null, "Author updated successfully");
    }

    private async Task<ApiResponse> HandleDeleteAuthorAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        await _bookService.DeleteAuthorAsync(req.Id);
        return ApiResponse.SuccessResponse(null, "Author deleted successfully");
    }

    // Customer Handlers
    private async Task<ApiResponse> HandleGetAllCustomersAsync()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return ApiResponse.SuccessResponse(customers);
    }

    private async Task<ApiResponse> HandleGetCustomerByIdAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        var customer = await _customerService.GetCustomerByIdAsync(req.Id);
        if (customer == null) return ApiResponse.FailResponse("Customer not found");

        return ApiResponse.SuccessResponse(customer);
    }

    private async Task<ApiResponse> HandleAddCustomerAsync(JsonElement data)
    {
        var customer = JsonSerializer.Deserialize<Customer>(data.GetRawText());
        if (customer == null) return ApiResponse.FailResponse("Invalid data");

        customer.CardStatus = "Active";
        customer.CreatedAt = DateTime.Now;

        await _customerService.AddCustomerAsync(customer);
        return ApiResponse.SuccessResponse(null, "Customer added successfully");
    }

    private async Task<ApiResponse> HandleUpdateCustomerAsync(JsonElement data)
    {
        var customer = JsonSerializer.Deserialize<Customer>(data.GetRawText());
        if (customer == null) return ApiResponse.FailResponse("Invalid data");

        await _customerService.UpdateCustomerAsync(customer);
        return ApiResponse.SuccessResponse(null, "Customer updated successfully");
    }

    private async Task<ApiResponse> HandleActivateCardAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        await _customerService.ActivateCardAsync(req.Id);
        return ApiResponse.SuccessResponse(null, "Card activated successfully");
    }

    private async Task<ApiResponse> HandleDeactivateCardAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        await _customerService.DeactivateCardAsync(req.Id);
        return ApiResponse.SuccessResponse(null, "Card deactivated successfully");
    }

    private async Task<ApiResponse> HandleCustomerLoginAsync(JsonElement data)
    {
        var dto = JsonSerializer.Deserialize<LoginRequest>(data.GetRawText());
        if (dto == null) return ApiResponse.FailResponse("Invalid data");

        var customer = await _customerService.LoginAsync(dto.UserName, dto.Password);
        if (customer == null) return ApiResponse.FailResponse("Invalid username or password");

        return ApiResponse.SuccessResponse(customer);
    }

    // Borrow Handlers
    private async Task<ApiResponse> HandleGetAllBorrowsAsync()
    {
        var borrows = await _borrowService.GetAllBorrowsAsync();
        return ApiResponse.SuccessResponse(borrows);
    }

    private async Task<ApiResponse> HandleGetBorrowsByCustomerIdAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        var borrows = await _borrowService.GetBorrowsByCustomerIdAsync(req.Id);
        return ApiResponse.SuccessResponse(borrows);
    }

    private async Task<ApiResponse> HandleGetBorrowedBooksAsync()
    {
        var borrows = await _borrowService.GetBorrowedBooksAsync();
        return ApiResponse.SuccessResponse(borrows);
    }

    private async Task<ApiResponse> HandleGetOverdueBooksAsync()
    {
        var borrows = await _borrowService.GetOverdueBooksAsync();
        return ApiResponse.SuccessResponse(borrows);
    }

    private async Task<ApiResponse> HandleGetRequestedBorrowsAsync()
    {
        var borrows = await _borrowService.GetRequestedBorrowsAsync();
        return ApiResponse.SuccessResponse(borrows);
    }

    private async Task<ApiResponse> HandleRequestBorrowAsync(JsonElement data)
    {
        var dto = JsonSerializer.Deserialize<BorrowRequestData>(data.GetRawText());
        if (dto == null) return ApiResponse.FailResponse("Invalid data");

        await _borrowService.RequestBorrowAsync(dto.CustomerId, dto.BookId, dto.CreatedByCustomer);
        return ApiResponse.SuccessResponse(null, "Borrow request created successfully");
    }

    private async Task<ApiResponse> HandleApproveBorrowAsync(JsonElement data)
    {
        var dto = JsonSerializer.Deserialize<ApproveBorrowRequest>(data.GetRawText());
        if (dto == null) return ApiResponse.FailResponse("Invalid data");

        await _borrowService.ApproveBorrowAsync(dto.BorrowId, dto.StaffId, dto.DaysToLend);
        return ApiResponse.SuccessResponse(null, "Borrow approved successfully");
    }

    private async Task<ApiResponse> HandleReturnBookAsync(JsonElement data)
    {
        var dto = JsonSerializer.Deserialize<ReturnBookRequest>(data.GetRawText());
        if (dto == null) return ApiResponse.FailResponse("Invalid data");

        await _borrowService.ReturnBookAsync(dto.BorrowId, dto.StaffId);
        return ApiResponse.SuccessResponse(null, "Book returned successfully");
    }

    private async Task<ApiResponse> HandleExtendDueDateAsync(JsonElement data)
    {
        var dto = JsonSerializer.Deserialize<ExtendRequest>(data.GetRawText());
        if (dto == null) return ApiResponse.FailResponse("Invalid data");

        await _borrowService.ExtendDueDateAsync(dto.BorrowId, dto.AdditionalDays);
        return ApiResponse.SuccessResponse(null, "Due date extended successfully");
    }

    private async Task<ApiResponse> HandleCancelBorrowAsync(JsonElement data)
    {
        var req = JsonSerializer.Deserialize<IdRequest>(data.GetRawText());
        if (req == null) return ApiResponse.FailResponse("Invalid data");
        
        await _borrowService.CancelBorrowAsync(req.Id);
        return ApiResponse.SuccessResponse(null, "Borrow cancelled successfully");
    }
}
