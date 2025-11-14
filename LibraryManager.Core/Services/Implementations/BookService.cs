using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using LibraryManager.Core.Services.Interfaces;

namespace LibraryManager.Core.Services.Implementations;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAuthorRepository _authorRepository;

    public BookService(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
        _authorRepository = authorRepository;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<List<Book>> SearchBooksAsync(string? keyword, int? categoryId, int? authorId)
    {
        return await _bookRepository.SearchAsync(keyword, categoryId, authorId);
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<List<Author>> GetAllAuthorsAsync()
    {
        return await _authorRepository.GetAllAsync();
    }

    public async Task AddBookAsync(Book book)
    {
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        await _bookRepository.UpdateAsync(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        await _bookRepository.DeleteAsync(id);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task AddCategoryAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _categoryRepository.DeleteAsync(id);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task AddAuthorAsync(Author author)
    {
        await _authorRepository.AddAsync(author);
        await _authorRepository.SaveChangesAsync();
    }

    public async Task UpdateAuthorAsync(Author author)
    {
        await _authorRepository.UpdateAsync(author);
        await _authorRepository.SaveChangesAsync();
    }

    public async Task DeleteAuthorAsync(int id)
    {
        await _authorRepository.DeleteAsync(id);
        await _authorRepository.SaveChangesAsync();
    }
}
