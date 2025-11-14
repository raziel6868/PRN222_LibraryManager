using LibraryManager.Core.Data;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Core.Repositories.Implementations;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;

    public BookRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _context.Books
            .Include(b => b.Category)
            .Include(b => b.Author)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Category)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.BookId == id);
    }

    public async Task<List<Book>> SearchAsync(string? keyword, int? categoryId, int? authorId)
    {
        var query = _context.Books
            .Include(b => b.Category)
            .Include(b => b.Author)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(b => b.Title.Contains(keyword) || 
                                     b.Author.Name.Contains(keyword) ||
                                     (b.Isbn != null && b.Isbn.Contains(keyword)));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
        }

        if (authorId.HasValue && authorId.Value > 0)
        {
            query = query.Where(b => b.AuthorId == authorId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Books.AnyAsync(b => b.BookId == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
