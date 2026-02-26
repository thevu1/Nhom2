using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace group
{
    /// <summary>
    /// Interaction logic for QuenMatKhau.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string input = txtEmailOrUser.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                lblMessage.Text = "Vui lòng nhập Email hoặc Tên đăng nhập!";
                return;
            }

            // Giả lập kiểm tra trong hệ thống
            if (input == "admin" || input == "admin@example.com")
            {
                lblMessage.Text = "Yêu cầu đặt lại mật khẩu đã được gửi!";
                lblMessage.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                lblMessage.Text = "Không tìm thấy tài khoản!";
                lblMessage.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}