using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace group
{
    public partial class QuenMatKhau : Window
    {
        public static string OTPCode = "";
        public static string UserReset = "";

        public QuenMatKhau()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string user = txtEmailOrUser.Text.Trim();

            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM taikhoan WHERE Username=@u";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", user);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    lblMessage.Text = "Tài khoản không tồn tại";
                    return;
                }

                Random rnd = new Random();
                OTPCode = rnd.Next(100000, 999999).ToString();

                UserReset = user;

                MessageBox.Show("OTP của bạn: " + OTPCode);

                XacNhanOTP win = new XacNhanOTP();
                win.Show();

                this.Close();
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}