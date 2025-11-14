using System.Collections.ObjectModel;
using System.Windows;
using LibraryManager.AdminClient.Services;
using LibraryManager.Core.Entities;

namespace LibraryManager.AdminClient;

public partial class MainWindow : Window
{
    private readonly TcpApiClient _apiClient;
    private readonly Staff _currentStaff;

    public ObservableCollection<Book> Books { get; set; } = new();
    public ObservableCollection<Category> Categories { get; set; } = new();
    public ObservableCollection<Author> Authors { get; set; } = new();

    public MainWindow(Staff staff)
    {
        InitializeComponent();
        _apiClient = new TcpApiClient();
        _currentStaff = staff;
        
        DataContext = this;
        txtUserName.Text = $"Logged in as: {staff.FullName} (Admin)";
        
        LoadData();
    }

    private async void LoadData()
    {
        await LoadBooksAsync();
        await LoadCategoriesAsync();
        await LoadAuthorsAsync();
    }

    private async Task LoadBooksAsync()
    {
        try
        {
            var books = await _apiClient.SendRequestAsync<List<Book>>("Book.GetAll");
            Books.Clear();
            if (books != null)
                foreach (var book in books) Books.Add(book);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _apiClient.SendRequestAsync<List<Category>>("Category.GetAll");
            Categories.Clear();
            if (categories != null)
                foreach (var cat in categories) Categories.Add(cat);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadAuthorsAsync()
    {
        try
        {
            var authors = await _apiClient.SendRequestAsync<List<Author>>("Author.GetAll");
            Authors.Clear();
            if (authors != null)
                foreach (var author in authors) Authors.Add(author);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading authors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnSearchBooks_Click(object sender, RoutedEventArgs e)
    {
        await LoadBooksAsync();
    }

    private async void BtnRefreshBooks_Click(object sender, RoutedEventArgs e)
    {
        await LoadBooksAsync();
    }

    private async void BtnAddBook_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Views.BookDialog(Categories.ToList(), Authors.ToList())
        {
            Owner = this
        };

        dialog.ShowDialog();

        if (dialog.Success)
        {
            await LoadBooksAsync();
            MessageBox.Show("Book added!", "Success");
        }
    }

    private async void BtnEditBook_Click(object sender, RoutedEventArgs e)
    {
        if (dgBooks.SelectedItem is not Book book)
        {
            MessageBox.Show("Please select a book to edit", "Info");
            return;
        }

        var dialog = new Views.BookDialog(book, Categories.ToList(), Authors.ToList())
        {
            Owner = this
        };

        dialog.ShowDialog();

        if (dialog.Success)
        {
            await LoadBooksAsync();
            MessageBox.Show("Book updated!", "Success");
        }
    }

    private async void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
    {
        if (dgBooks.SelectedItem is not Book book) return;

        if (MessageBox.Show($"Delete book '{book.Title}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                await _apiClient.SendRequestAsync("Book.Delete", new { Id = book.BookId });
                await LoadBooksAsync();
                MessageBox.Show("Book deleted!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
    }

    private async void BtnAddCategory_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNewCategory.Text)) return;

        try
        {
            await _apiClient.SendRequestAsync("Category.Add", new Category { Name = txtNewCategory.Text });
            txtNewCategory.Clear();
            await LoadCategoriesAsync();
            MessageBox.Show("Category added!", "Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }

    private async void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
    {
        if (lstCategories.SelectedItem is not Category cat) return;

        if (MessageBox.Show($"Delete '{cat.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                await _apiClient.SendRequestAsync("Category.Delete", new { Id = cat.CategoryId });
                await LoadCategoriesAsync();
                MessageBox.Show("Category deleted!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
    }

    private async void BtnAddAuthor_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNewAuthor.Text)) return;

        try
        {
            await _apiClient.SendRequestAsync("Author.Add", new Author { Name = txtNewAuthor.Text });
            txtNewAuthor.Clear();
            await LoadAuthorsAsync();
            MessageBox.Show("Author added!", "Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }

    private async void BtnDeleteAuthor_Click(object sender, RoutedEventArgs e)
    {
        if (lstAuthors.SelectedItem is not Author author) return;

        if (MessageBox.Show($"Delete '{author.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                await _apiClient.SendRequestAsync("Author.Delete", new { Id = author.AuthorId });
                await LoadAuthorsAsync();
                MessageBox.Show("Author deleted!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Logout?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            new Views.LoginWindow().Show();
            this.Close();
        }
    }
}
