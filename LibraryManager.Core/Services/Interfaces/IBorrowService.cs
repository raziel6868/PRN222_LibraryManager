using LibraryManager.Core.Entities;

namespace LibraryManager.Core.Services.Interfaces;

public interface IBorrowService
{
    Task<List<Borrow>> GetAllBorrowsAsync();
    Task<Borrow?> GetBorrowByIdAsync(int id);
    Task<List<Borrow>> GetBorrowsByCustomerIdAsync(int customerId);
    Task<List<Borrow>> GetBorrowedBooksAsync();
    Task<List<Borrow>> GetOverdueBooksAsync();
    Task<List<Borrow>> GetRequestedBorrowsAsync();
    Task RequestBorrowAsync(int customerId, int bookId, bool createdByCustomer = false);
    Task ApproveBorrowAsync(int borrowId, int staffId, int daysToLend = 14);
    Task ReturnBookAsync(int borrowId, int staffId);
    Task ExtendDueDateAsync(int borrowId, int additionalDays);
    Task CancelBorrowAsync(int borrowId);
    Task CalculateFineAsync(int borrowId);
}
