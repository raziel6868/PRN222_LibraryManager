using LibraryManager.Core.Data;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Core.Repositories.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly LibraryContext _context;

    public CustomerRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
    }

    public async Task<Customer?> GetByUserNameAsync(string userName)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.UserName == userName);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Customers.AnyAsync(c => c.CustomerId == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
