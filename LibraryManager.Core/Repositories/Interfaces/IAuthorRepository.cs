using LibraryManager.Core.Entities;

namespace LibraryManager.Core.Repositories.Interfaces;

public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync();
    Task<Author?> GetByIdAsync(int id);
    Task AddAsync(Author author);
    Task UpdateAsync(Author author);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
