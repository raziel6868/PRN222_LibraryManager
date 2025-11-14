using System.Windows;
using LibraryManager.AdminClient.Services;
using LibraryManager.Core.Entities;

namespace LibraryManager.AdminClient.Views;

public partial class LoginWindow : Window
{
    private readonly TcpApiClient _apiClient;

    public LoginWindow()
    {
        InitializeComponent();
        _apiClient = new TcpApiClient();
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        txtError.Visibility = Visibility.Collapsed;
        btnLogin.IsEnabled = false;

        try
        {
            var loginData = new { UserName = txtUsername.Text, Password = txtPassword.Password };
            var staff = await _apiClient.SendRequestAsync<Staff>("Staff.Login", loginData);

            if (staff != null && staff.Role == "Admin")
            {
                var mainWindow = new MainWindow(staff);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                txtError.Text = "You must be an Admin to access this application.";
                txtError.Visibility = Visibility.Visible;
            }
        }
        catch (Exception ex)
        {
            txtError.Text = ex.Message;
            txtError.Visibility = Visibility.Visible;
        }
        finally
        {
            btnLogin.IsEnabled = true;
        }
    }
}
