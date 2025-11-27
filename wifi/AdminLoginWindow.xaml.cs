using System.Windows;

namespace wifi
{
    public partial class AdminLoginWindow : Window
    {
        public bool IsAdminAuthorized { get; private set; } = false;
        public AdminLoginWindow()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == "admin" && PasswordBox.Password == "admin")
            {
                IsAdminAuthorized = true;
                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
