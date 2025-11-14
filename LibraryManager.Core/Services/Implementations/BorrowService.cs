using LibraryManager.Core.Entities;
using LibraryManager.Core.Repositories.Interfaces;
using LibraryManager.Core.Services.Interfaces;

namespace LibraryManager.Core.Services.Implementations;

public class BorrowService : IBorrowService
{
    private readonly IBorrowRepository _borrowRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ICustomerRepository _customerRepository;

    public BorrowService(
        IBorrowRepository borrowRepository,
        IBookRepository bookRepository,
        ICustomerRepository customerRepository)
    {
        _borrowRepository = borrowRepository;
        _bookRepository = bookRepository;
        _customerRepository = customerRepository;
    }

    public async Task<List<Borrow>> GetAllBorrowsAsync()
    {
        return await _borrowRepository.GetAllAsync();
    }

    public async Task<Borrow?> GetBorrowByIdAsync(int id)
    {
        return await _borrowRepository.GetByIdAsync(id);
    }

    public async Task<List<Borrow>> GetBorrowsByCustomerIdAsync(int customerId)
    {
        return await _borrowRepository.GetByCustomerIdAsync(customerId);
    }

    public async Task<List<Borrow>> GetBorrowedBooksAsync()
    {
        return await _borrowRepository.GetBorrowedBooksAsync();
    }

    public async Task<List<Borrow>> GetOverdueBooksAsync()
    {
        return await _borrowRepository.GetOverdueBooksAsync();
    }

    public async Task<List<Borrow>> GetRequestedBorrowsAsync()
    {
        return await _borrowRepository.GetByStatusAsync("Requested");
    }

    public async Task RequestBorrowAsync(int customerId, int bookId, bool createdByCustomer = false)
    {
        // Check if customer exists and is active
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            throw new Exception("Customer not found");
        }
        if (customer.CardStatus != "Active")
        {
            throw new Exception("Customer card is not active");
        }

        // Check if book exists and available
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
        {
            throw new Exception("Book not found");
        }
        if (book.AvailableCopies <= 0)
        {
            throw new Exception("No copies available");
        }

        // Create borrow request
        var borrow = new Borrow
        {
            CustomerId = customerId,
            BookId = bookId,
            RequestDate = DateTime.Now,
            Status = "Requested",
            CreatedByCustomer = createdByCustomer
        };

        await _borrowRepository.AddAsync(borrow);
        await _borrowRepository.SaveChangesAsync();
    }

    public async Task ApproveBorrowAsync(int borrowId, int staffId, int daysToLend = 14)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId);
        if (borrow == null)
        {
            throw new Exception("Borrow record not found");
        }
        if (borrow.Status != "Requested")
        {
            throw new Exception("Borrow is not in Requested status");
        }

        // Check book availability
        var book = await _bookRepository.GetByIdAsync(borrow.BookId);
        if (book == null || book.AvailableCopies <= 0)
        {
            throw new Exception("Book not available");
        }

        // Update borrow
        borrow.Status = "Borrowed";
        borrow.BorrowDate = DateTime.Now;
        borrow.DueDate = DateTime.Now.AddDays(daysToLend);
        borrow.ProcessedByStaffId = staffId;

        // Decrease available copies
        book.AvailableCopies--;

        await _borrowRepository.UpdateAsync(borrow);
        await _bookRepository.UpdateAsync(book);
        await _borrowRepository.SaveChangesAsync();
        await _bookRepository.SaveChangesAsync();
    }

    public async Task ReturnBookAsync(int borrowId, int staffId)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId);
        if (borrow == null)
        {
            throw new Exception("Borrow record not found");
        }
        if (borrow.Status != "Borrowed")
        {
            throw new Exception("Borrow is not in Borrowed status");
        }

        // Calculate fine if overdue
        await CalculateFineAsync(borrowId);

        // Update borrow
        borrow.Status = "Returned";
        borrow.ReturnDate = DateTime.Now;
        borrow.ProcessedByStaffId = staffId;

        // Increase available copies
        var book = await _bookRepository.GetByIdAsync(borrow.BookId);
        if (book != null)
        {
            book.AvailableCopies++;
            await _bookRepository.UpdateAsync(book);
        }

        await _borrowRepository.UpdateAsync(borrow);
        await _borrowRepository.SaveChangesAsync();
        await _bookRepository.SaveChangesAsync();
    }

    public async Task ExtendDueDateAsync(int borrowId, int additionalDays)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId);
        if (borrow == null)
        {
            throw new Exception("Borrow record not found");
        }
        if (borrow.Status != "Borrowed")
        {
            throw new Exception("Borrow is not in Borrowed status");
        }

        borrow.DueDate = borrow.DueDate?.AddDays(additionalDays);
        await _borrowRepository.UpdateAsync(borrow);
        await _borrowRepository.SaveChangesAsync();
    }

    public async Task CancelBorrowAsync(int borrowId)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId);
        if (borrow == null)
        {
            throw new Exception("Borrow record not found");
        }
        if (borrow.Status != "Requested")
        {
            throw new Exception("Can only cancel requested borrows");
        }

        borrow.Status = "Cancelled";
        await _borrowRepository.UpdateAsync(borrow);
        await _borrowRepository.SaveChangesAsync();
    }

    public async Task CalculateFineAsync(int borrowId)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId);
        if (borrow == null || borrow.DueDate == null)
        {
            return;
        }

        if (DateTime.Now > borrow.DueDate)
        {
            var daysOverdue = (DateTime.Now - borrow.DueDate.Value).Days;
            borrow.FineAmount = daysOverdue * 5000; // 5000 per day
            await _borrowRepository.UpdateAsync(borrow);
            await _borrowRepository.SaveChangesAsync();
        }
    }
}
