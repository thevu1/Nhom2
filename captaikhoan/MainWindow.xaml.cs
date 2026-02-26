using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CapTaiKhoan
{
    public partial class MainWindow : Window
    {
        ObservableCollection<User> users = new ObservableCollection<User>();

        public MainWindow()
        {
            InitializeComponent();
            dgUsers.ItemsSource = users;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                MessageBox.Show("Nhập user");
                return;
            }

            string role = ((ComboBoxItem)cbRole.SelectedItem).Content.ToString();

            users.Add(new User
            {
                Username = txtUser.Text,
                Password = txtPass.Password,
                Role = role
            });

            txtUser.Clear();
            txtPass.Clear();
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}