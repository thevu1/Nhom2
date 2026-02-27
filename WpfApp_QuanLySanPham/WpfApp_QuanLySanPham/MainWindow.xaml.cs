using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfApp_QuanLySanPham
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Product> _products;
        private ICollectionView _productView;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            SetupComboBox();
            _productView = CollectionViewSource.GetDefaultView(_products);
            dgProducts.ItemsSource = _productView;
        }

        // Khởi tạo dữ liệu mẫu
        private void LoadData()
        {
            _products = new ObservableCollection<Product>
            {
                new Product { MSP = "SP001", TENSP = "Sản phẩm 1", NgayNhap = DateTime.Now.AddDays(-10), GiaNhap = 10000, GiaBan = 15000, Donvi = "Cái" },
                new Product { MSP = "SP002", TENSP = "Sản phẩm 2", NgayNhap = DateTime.Now.AddDays(-5), GiaNhap = 20000, GiaBan = 28000, Donvi = "Hộp" },
                new Product { MSP = "SP003", TENSP = "Sản phẩm 3", NgayNhap = DateTime.Now, GiaNhap = 5000, GiaBan = 8000, Donvi = "Kg" }
            };
        }

        // Đổ dữ liệu cho ComboBox đơn vị tính
        private void SetupComboBox()
        {
            cboDonVi.ItemsSource = new string[] { "Cái", "Hộp", "Kg", "Lít", "Bịch", "Thùng" };
        }

        // Lấy dữ liệu từ form nhập, trả về Product nếu hợp lệ, ngược lại null
        private Product GetProductFromInput()
        {
            try
            {
                string ma = txtMaSP.Text.Trim();
                string ten = txtTenSP.Text.Trim();
                DateTime? ngayNhap = dpNgayNhap.SelectedDate;
                decimal giaNhap = decimal.Parse(txtGiaNhap.Text.Trim());
                decimal giaBan = decimal.Parse(txtGiaBan.Text.Trim());
                string donvi = cboDonVi.Text.Trim();

                if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten) || ngayNhap == null || string.IsNullOrEmpty(donvi))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                return new Product
                {
                    MSP = ma,
                    TENSP = ten,
                    NgayNhap = ngayNhap.Value,
                    GiaNhap = giaNhap,
                    GiaBan = giaBan,
                    Donvi = donvi
                };
            }
            catch (FormatException)
            {
                MessageBox.Show("Giá nhập và giá bán phải là số hợp lệ!", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // Xóa trắng form nhập
        private void ClearInput()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dpNgayNhap.SelectedDate = null;
            txtGiaNhap.Clear();
            txtGiaBan.Clear();
            cboDonVi.SelectedIndex = -1;
            cboDonVi.Text = "";
        }

        // Kiểm tra trùng mã (loại trừ sản phẩm hiện tại khi sửa)
        private bool IsProductCodeExists(string code, Product exclude = null)
        {
            return _products.Any(p => p.MSP.Equals(code, StringComparison.OrdinalIgnoreCase) && p != exclude);
        }

        // Thêm sản phẩm
        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            Product newProduct = GetProductFromInput();
            if (newProduct == null) return;

            if (IsProductCodeExists(newProduct.MSP))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!", "Trùng mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _products.Add(newProduct);
            ClearInput();
        }

        // Sửa sản phẩm
        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            Product selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Product updated = GetProductFromInput();
            if (updated == null) return;

            if (IsProductCodeExists(updated.MSP, selected))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại ở sản phẩm khác!", "Trùng mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Cập nhật
            selected.MSP = updated.MSP;
            selected.TENSP = updated.TENSP;
            selected.NgayNhap = updated.NgayNhap;
            selected.GiaNhap = updated.GiaNhap;
            selected.GiaBan = updated.GiaBan;
            selected.Donvi = updated.Donvi;

            _productView.Refresh();
            ClearInput();
        }

        // Xóa sản phẩm
        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            Product selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm '{selected.TENSP}'?", "Xác nhận xóa",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _products.Remove(selected);
                ClearInput();
            }
        }

        // Tìm kiếm theo từ khóa (mã hoặc tên)
        private void btnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtTuKhoa.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                _productView.Filter = null;  // hiển thị tất cả
            }
            else
            {
                _productView.Filter = obj =>
                {
                    Product p = obj as Product;
                    if (p == null) return false;
                    return p.MSP.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                        || p.TENSP.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
                };
            }
        }

        // Làm mới: xóa filter, xóa từ khóa, xóa form nhập
        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            _productView.Filter = null;
            txtTuKhoa.Text = "";
            ClearInput();
        }

        // Double-click vào dòng: đưa dữ liệu lên form để sửa
        private void dgProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Product selected = dgProducts.SelectedItem as Product;
            if (selected == null) return;

            txtMaSP.Text = selected.MSP;
            txtTenSP.Text = selected.TENSP;
            dpNgayNhap.SelectedDate = selected.NgayNhap;
            txtGiaNhap.Text = selected.GiaNhap.ToString();
            txtGiaBan.Text = selected.GiaBan.ToString();
            cboDonVi.Text = selected.Donvi;
        }
    }

    // Lớp sản phẩm với thông báo thay đổi (INotifyPropertyChanged)
    public class Product : INotifyPropertyChanged
    {
        private string _msp;
        private string _tensp;
        private DateTime _ngayNhap;
        private decimal _giaNhap;
        private decimal _giaBan;
        private string _donvi;

        public string MSP
        {
            get => _msp;
            set { _msp = value; OnPropertyChanged(nameof(MSP)); }
        }

        public string TENSP
        {
            get => _tensp;
            set { _tensp = value; OnPropertyChanged(nameof(TENSP)); }
        }

        public DateTime NgayNhap
        {
            get => _ngayNhap;
            set { _ngayNhap = value; OnPropertyChanged(nameof(NgayNhap)); }
        }

        public decimal GiaNhap
        {
            get => _giaNhap;
            set { _giaNhap = value; OnPropertyChanged(nameof(GiaNhap)); }
        }

        public decimal GiaBan
        {
            get => _giaBan;
            set { _giaBan = value; OnPropertyChanged(nameof(GiaBan)); }
        }

        public string Donvi
        {
            get => _donvi;
            set { _donvi = value; OnPropertyChanged(nameof(Donvi)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}