using LibraryManager.Core.Data;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Core.Repositories.Implementations;

public class BorrowRepository : IBorrowRepository
{
    private readonly LibraryContext _context;

    public BorrowRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<List<Borrow>> GetAllAsync()
    {
        return await _context.Borrows
            .Include(b => b.Customer)
            .Include(b => b.Book)
            .Include(b => b.ProcessedByStaff)
            .ToListAsync();
    }

    public async Task<Borrow?> GetByIdAsync(int id)
    {
        return await _context.Borrows
            .Include(b => b.Customer)
            .Include(b => b.Book)
            .Include(b => b.ProcessedByStaff)
            .FirstOrDefaultAsync(b => b.BorrowId == id);
    }

    public async Task<List<Borrow>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Borrows
            .Include(b => b.Customer)
            .Include(b => b.Book)
            .ThenInclude(book => book.Author)
            .Include(b => b.ProcessedByStaff)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.RequestDate)
            .ToListAsync();
    }

    public async Task<List<Borrow>> GetByStatusAsync(string status)
    {
        return await _context.Borrows
            .Include(b => b.Customer)
            .Include(b => b.Book)
            .ThenInclude(book => book.Author)
            .Include(b => b.ProcessedByStaff)
            .Where(b => b.Status == status)
            .OrderByDescending(b => b.RequestDate)
            .ToListAsync();
    }

    public async Task<List<Borrow>> GetBorrowedBooksAsync()
    {
        return await _context.Borrows
            .Include(b => b.Customer)
            .Include(b => b.Book)
            .ThenInclude(book => book.Author)
            .Include(b => b.ProcessedByStaff)
            .Where(b => b.Status == "Borrowed")
            .OrderBy(b => b.DueDate)
            .ToListAsync();
    }

    public async Task<List<Borrow>> GetOverdueBooksAsync()
    {
        var now = DateTime.Now;
        return await _context.Borrows
            .Include(b => b.Customer)
            .Include(b => b.Book)
            .ThenInclude(book => book.Author)
            .Include(b => b.ProcessedByStaff)
            .Where(b => b.Status == "Borrowed" && b.DueDate < now)
            .OrderBy(b => b.DueDate)
            .ToListAsync();
    }

    public async Task AddAsync(Borrow borrow)
    {
        await _context.Borrows.AddAsync(borrow);
    }

    public async Task UpdateAsync(Borrow borrow)
    {
        _context.Borrows.Update(borrow);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow != null)
        {
            _context.Borrows.Remove(borrow);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
