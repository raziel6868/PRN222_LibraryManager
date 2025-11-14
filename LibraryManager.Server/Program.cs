using LibraryManager.Core.Data;
using LibraryManager.Core.Repositories.Implementations;
using LibraryManager.Core.Repositories.Interfaces;
using LibraryManager.Core.Services.Implementations;
using LibraryManager.Core.Services.Interfaces;
using LibraryManager.Server.Handlers;
using LibraryManager.Server.Networking;

Console.WriteLine("=== LibraryManager TCP Server ===");
Console.WriteLine("Initializing services...");

// Initialize DbContext
var context = new LibraryContext();

// Initialize Repositories
IBookRepository bookRepo = new BookRepository(context);
ICategoryRepository categoryRepo = new CategoryRepository(context);
IAuthorRepository authorRepo = new AuthorRepository(context);
ICustomerRepository customerRepo = new CustomerRepository(context);
IBorrowRepository borrowRepo = new BorrowRepository(context);
IStaffRepository staffRepo = new StaffRepository(context);

// Initialize Services
IBookService bookService = new BookService(bookRepo, categoryRepo, authorRepo);
ICustomerService customerService = new CustomerService(customerRepo);
IBorrowService borrowService = new BorrowService(borrowRepo, bookRepo, customerRepo);
IStaffService staffService = new StaffService(staffRepo);

// Initialize Request Dispatcher
var dispatcher = new RequestDispatcher(bookService, customerService, borrowService, staffService);

// Initialize and Start TCP Server
const int port = 5000;
var server = new TcpServer(port, dispatcher);

Console.WriteLine("Services initialized successfully!");
Console.WriteLine();

await server.StartAsync();
