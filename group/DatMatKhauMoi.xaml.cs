using MySql.Data.MySqlClient;
using System.Windows;

namespace group
{
    public partial class DatMatKhauMoi : Window
    {
        public DatMatKhauMoi()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string pass = txtNewPass.Password.Trim();
            string confirm = txtConfirmPass.Password.Trim();

            if (pass == "" || confirm == "")
            {
                lblMessage.Text = "Vui lòng nhập đầy đủ mật khẩu.";
                return;
            }

            if (pass != confirm)
            {
                lblMessage.Text = "Mật khẩu xác nhận không khớp.";
                return;
            }

            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "UPDATE taikhoan SET Password=@p WHERE Username=@u";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@p", pass);
                cmd.Parameters.AddWithValue("@u", QuenMatKhau.UserReset);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Đổi mật khẩu thành công!");

                this.Close();
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}