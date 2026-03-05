using System.Windows;

namespace group
{
    public partial class XacNhanOTP : Window
    {
        public XacNhanOTP()
        {
            InitializeComponent();
        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            if (txtOTP.Text == QuenMatKhau.OTPCode)
            {
                DatMatKhauMoi win = new DatMatKhauMoi();
                win.Show();
                this.Close();
            }
            else
            {
                lblMessage.Text = "OTP không đúng!";
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}