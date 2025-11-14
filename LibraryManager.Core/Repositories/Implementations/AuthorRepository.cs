using LibraryManager.Core.Data;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Core.Repositories.Implementations;

public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryContext _context;

    public AuthorRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<List<Author>> GetAllAsync()
    {
        return await _context.Authors.ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors.FirstOrDefaultAsync(a => a.AuthorId == id);
    }

    public async Task AddAsync(Author author)
    {
        await _context.Authors.AddAsync(author);
    }

    public async Task UpdateAsync(Author author)
    {
        _context.Authors.Update(author);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author != null)
        {
            _context.Authors.Remove(author);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
