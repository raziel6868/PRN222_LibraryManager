using System.Windows;
using LibraryManager.AdminClient.Services;
using LibraryManager.Core.Entities;

namespace LibraryManager.AdminClient.Views;

public partial class BookDialog : Window
{
    private readonly TcpApiClient _apiClient;
    private readonly Book? _existingBook;
    public bool Success { get; private set; }

    // For Add
    public BookDialog(List<Category> categories, List<Author> authors)
    {
        InitializeComponent();
        _apiClient = new TcpApiClient();
        _existingBook = null;

        cmbCategory.ItemsSource = categories;
        cmbAuthor.ItemsSource = authors;

        Title = "Add New Book";
        txtAvailableCopies.Text = "0";
        txtTotalCopies.Text = "0";
    }

    // For Edit
    public BookDialog(Book book, List<Category> categories, List<Author> authors)
    {
        InitializeComponent();
        _apiClient = new TcpApiClient();
        _existingBook = book;

        cmbCategory.ItemsSource = categories;
        cmbAuthor.ItemsSource = authors;

        Title = "Edit Book";
        txtTitle.Text = book.Title;
        txtIsbn.Text = book.Isbn;
        txtPublishYear.Text = book.PublishYear?.ToString();
        cmbCategory.SelectedValue = book.CategoryId;
        cmbAuthor.SelectedValue = book.AuthorId;
        txtTotalCopies.Text = book.TotalCopies.ToString();
        txtAvailableCopies.Text = book.AvailableCopies.ToString();
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        txtError.Visibility = Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            ShowError("Title is required");
            return;
        }

        if (cmbCategory.SelectedValue == null)
        {
            ShowError("Category is required");
            return;
        }

        if (cmbAuthor.SelectedValue == null)
        {
            ShowError("Author is required");
            return;
        }

        if (!int.TryParse(txtTotalCopies.Text, out int totalCopies) || totalCopies < 0)
        {
            ShowError("Total Copies must be a valid number >= 0");
            return;
        }

        if (!int.TryParse(txtAvailableCopies.Text, out int availableCopies) || availableCopies < 0)
        {
            ShowError("Available Copies must be a valid number >= 0");
            return;
        }

        if (availableCopies > totalCopies)
        {
            ShowError("Available Copies cannot exceed Total Copies");
            return;
        }

        int? publishYear = null;
        if (!string.IsNullOrWhiteSpace(txtPublishYear.Text))
        {
            if (!int.TryParse(txtPublishYear.Text, out int year))
            {
                ShowError("Publish Year must be a valid number");
                return;
            }
            publishYear = year;
        }

        try
        {
            var book = new Book
            {
                BookId = _existingBook?.BookId ?? 0,
                Title = txtTitle.Text.Trim(),
                Isbn = txtIsbn.Text.Trim(),
                PublishYear = publishYear,
                CategoryId = (int)cmbCategory.SelectedValue,
                AuthorId = (int)cmbAuthor.SelectedValue,
                TotalCopies = totalCopies,
                AvailableCopies = availableCopies
            };

            if (_existingBook == null)
            {
                await _apiClient.SendRequestAsync("Book.Add", book);
            }
            else
            {
                await _apiClient.SendRequestAsync("Book.Update", book);
            }

            Success = true;
            Close();
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        Success = false;
        Close();
    }

    private void ShowError(string message)
    {
        txtError.Text = message;
        txtError.Visibility = Visibility.Visible;
    }
}
