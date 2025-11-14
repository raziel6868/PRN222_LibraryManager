using LibraryManager.Core.Entities;

namespace LibraryManager.Core.Repositories.Interfaces;

public interface IBorrowRepository
{
    Task<List<Borrow>> GetAllAsync();
    Task<Borrow?> GetByIdAsync(int id);
    Task<List<Borrow>> GetByCustomerIdAsync(int customerId);
    Task<List<Borrow>> GetByStatusAsync(string status);
    Task<List<Borrow>> GetBorrowedBooksAsync();
    Task<List<Borrow>> GetOverdueBooksAsync();
    Task AddAsync(Borrow borrow);
    Task UpdateAsync(Borrow borrow);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
