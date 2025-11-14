using System.Security.Cryptography;
using System.Text;
using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using LibraryManager.Core.Services.Interfaces;

namespace LibraryManager.Core.Services.Implementations;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;

    public StaffService(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    public async Task<Staff?> LoginAsync(string userName, string password)
    {
        var staff = await _staffRepository.GetByUserNameAsync(userName);
        if (staff != null && staff.IsActive && VerifyPassword(password, staff.PasswordHash))
        {
            return staff;
        }
        return null;
    }

    public async Task<Staff?> GetStaffByIdAsync(int id)
    {
        return await _staffRepository.GetByIdAsync(id);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput.Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}
