using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace group
{
    public partial class DangNhap : Window
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        // nút hiện mật khẩu
        private void BtnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.PasswordChar == '*')
                txtPassword.PasswordChar = '\0';
            else
                txtPassword.PasswordChar = '*';
        }

        // đăng nhập
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Password.Trim();

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM taikhoan WHERE username=@u AND password=@p";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@p", pass);

                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    string role = rd["role"].ToString();

                    TrangChu f = new TrangChu(user, role);
                    f.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu");
                }
            }
        }

        // quên mật khẩu
        private void BtnForget_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Liên hệ admin để cấp lại mật khẩu");
        }

        // thoát
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}