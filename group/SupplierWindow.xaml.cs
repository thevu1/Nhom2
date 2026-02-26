using System.Windows;

namespace group
{
    public partial class SupplierWindow : Window
    {
        public SupplierWindow()
        {
            InitializeComponent();
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thêm");
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sửa");
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Xóa");
        }

        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tìm kiếm");
        }
    }
}
