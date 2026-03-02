using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace group
{
    public partial class XacNhanWindow : Window
    {
        string connStr =
            "server=localhost;database=phieuxuat;uid=root;pwd=123456;SslMode=none";

        public XacNhanWindow()
        {
            InitializeComponent();
        }

        private void BtnXacNhan_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT MaXacNhan FROM xacnhan LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Chưa có mã xác minh trong database");
                    return;
                }

                string maDB = result.ToString();

                if (txtMa.Password == maDB)
                {
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Sai mã xác minh");
                }
            }
        }

        // mở form đổi mã
        private void BtnDoiMa_Click(object sender, RoutedEventArgs e)
        {
            DoiMaWindow f = new DoiMaWindow();
            f.ShowDialog();
        }
    }
}   