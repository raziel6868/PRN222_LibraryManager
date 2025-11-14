using LibraryManager.Core.Entities;

namespace LibraryManager.Core.Services.Interfaces;

public interface IStaffService
{
    Task<Staff?> LoginAsync(string userName, string password);
    Task<Staff?> GetStaffByIdAsync(int id);
}
