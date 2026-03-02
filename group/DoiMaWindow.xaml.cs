using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace group
{
    public partial class DoiMaWindow : Window
    {
        string connStr =
            "server=localhost;database=phieuxuat;uid=root;pwd=123456;SslMode=none";

        public DoiMaWindow()
        {
            InitializeComponent();
        }

        private void BtnDoi_Click(object sender, RoutedEventArgs e)
        {
            if (txtOld.Password == "" || txtNew.Password == "")
            {
                MessageBox.Show("Nhập đầy đủ thông tin");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                // ===== Lấy mã cũ trong DB =====
                string sqlCheck = "SELECT MaXacNhan FROM xacnhan LIMIT 1";

                MySqlCommand cmdCheck = new MySqlCommand(sqlCheck, conn);

                object result = cmdCheck.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Database chưa có mã");
                    return;
                }

                string maDB = result.ToString();

                // ===== Kiểm tra mã cũ =====
                if (txtOld.Password != maDB)
                {
                    MessageBox.Show("Sai mã cũ");
                    return;
                }

                // ===== Update mã mới =====
                string sqlUpdate = "UPDATE xacnhan SET Ma=@ma";

                MySqlCommand cmd = new MySqlCommand(sqlUpdate, conn);
                cmd.Parameters.AddWithValue("@ma", txtNew.Password);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Đổi mã thành công");
                this.Close();
            }
        }
    }
}