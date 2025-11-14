using LibraryManager.Core.Data;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Core.Repositories.Implementations;

public class StaffRepository : IStaffRepository
{
    private readonly LibraryContext _context;

    public StaffRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<List<Staff>> GetAllAsync()
    {
        return await _context.Staff.ToListAsync();
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.StaffId == id);
    }

    public async Task<Staff?> GetByUserNameAsync(string userName)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.UserName == userName);
    }

    public async Task AddAsync(Staff staff)
    {
        await _context.Staff.AddAsync(staff);
    }

    public async Task UpdateAsync(Staff staff)
    {
        _context.Staff.Update(staff);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
