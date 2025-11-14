using LibraryManager.Core.Entities;

namespace LibraryManager.Core.Services.Interfaces;

public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<Customer?> LoginAsync(string userName, string password);
    Task AddCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
    Task ActivateCardAsync(int customerId);
    Task DeactivateCardAsync(int customerId);
    Task DeleteCustomerAsync(int id);
}
