using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

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

        private void LoadData()
        {
            _products = DatabaseHelper.GetAllProducts();
        }

        private void SetupComboBox()
        {
            cboDonVi.ItemsSource = new string[] { "Cái", "Hộp", "Kg", "Lít", "Bịch", "Thùng" };
        }

        private Product GetProductFromInput()
        {
            try
            {
                string ma = txtMaSP.Text.Trim();
                string ten = txtTenSP.Text.Trim();
                decimal giaNhap = decimal.Parse(txtGiaNhap.Text.Trim());
                decimal giaBan = decimal.Parse(txtGiaBan.Text.Trim());
                string donvi = cboDonVi.Text.Trim();
                string madm = txtMaDanhMuc.Text.Trim();

                return new Product
                {
                    MSP = ma,
                    TENSP = ten,
                    GiaNhap = giaNhap,
                    GiaBan = giaBan,
                    Donvi = donvi,
                    MADM = madm,
                    NgayNhap = DateTime.Now // giá trị mặc định nếu không nhập
                };
            }
            catch (FormatException)
            {
                MessageBox.Show("Giá nhập và giá bán phải là số hợp lệ!", "Lỗi định dạng",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ClearInput()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtGiaNhap.Clear();
            txtGiaBan.Clear();
            cboDonVi.SelectedIndex = -1;
            cboDonVi.Text = "";
            txtMaDanhMuc.Clear();
        }

        private bool IsProductCodeExists(string code, Product exclude = null)
        {
            return _products.Any(p => p.MSP.Equals(code, StringComparison.OrdinalIgnoreCase) && p != exclude);
        }

        // Nút Sửa
        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            Product selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Chưa chọn",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Product updated = GetProductFromInput();
            if (updated == null) return;

            if (IsProductCodeExists(updated.MSP, selected))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại ở sản phẩm khác!", "Trùng mã",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DatabaseHelper.UpdateProduct(updated))
            {
                selected.MSP = updated.MSP;
                selected.TENSP = updated.TENSP;
                selected.GiaNhap = updated.GiaNhap;
                selected.GiaBan = updated.GiaBan;
                selected.Donvi = updated.Donvi;
                selected.MADM = updated.MADM;
                selected.NgayNhap = updated.NgayNhap; // nếu cần cập nhật ngày

                _productView.Refresh();
                ClearInput();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Nút Xóa
        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            Product selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Chưa chọn",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm '{selected.TENSP}'?",
                                         "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (DatabaseHelper.DeleteProduct(selected.MSP))
                {
                    _products.Remove(selected);
                    ClearInput();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Nút Tìm kiếm
        private void btnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            string ma = txtMaSP.Text.Trim().ToLower();
            string ten = txtTenSP.Text.Trim().ToLower();
            string madm = txtMaDanhMuc.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(ma) && string.IsNullOrEmpty(ten) && string.IsNullOrEmpty(madm))
            {
                _productView.Filter = null;
            }
            else
            {
                _productView.Filter = obj =>
                {
                    Product p = obj as Product;
                    if (p == null) return false;

                    bool matchMa = string.IsNullOrEmpty(ma) || (p.MSP?.ToLower().Contains(ma) == true);
                    bool matchTen = string.IsNullOrEmpty(ten) || (p.TENSP?.ToLower().Contains(ten) == true);
                    bool matchMadm = string.IsNullOrEmpty(madm) || (p.MADM?.ToLower().Contains(madm) == true);

                    return matchMa && matchTen && matchMadm;
                };
            }
        }

        // Nút Làm mới
        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            _productView.Filter = null;
            ClearInput();
        }

        // Double-click vào dòng để đưa lên form
        private void dgProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Product selected = dgProducts.SelectedItem as Product;
            if (selected == null) return;

            txtMaSP.Text = selected.MSP;
            txtTenSP.Text = selected.TENSP;
            txtGiaNhap.Text = selected.GiaNhap.ToString();
            txtGiaBan.Text = selected.GiaBan.ToString();
            cboDonVi.Text = selected.Donvi;
            txtMaDanhMuc.Text = selected.MADM;
            // Không hiển thị ngày nhập vì không có ô nhập riêng
        }
    }

    // ==================== DatabaseHelper ====================
    public static class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["QLSanPham"].ConnectionString;

        public static ObservableCollection<Product> GetAllProducts()
        {
            var products = new ObservableCollection<Product>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT MaSP, TenSP, DonViTinh, GiaNhap, GiaBan, MaDanhMuc FROM sanpham";
                var cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            MSP = reader["MaSP"].ToString(),
                            TENSP = reader["TenSP"].ToString(),
                            Donvi = reader["DonViTinh"].ToString(),
                            GiaNhap = Convert.ToDecimal(reader["GiaNhap"]),
                            GiaBan = Convert.ToDecimal(reader["GiaBan"]),
                            MADM = reader["MaDanhMuc"].ToString(),
                            NgayNhap = DateTime.Now // tạm thời
                        });
                    }
                }
            }
            return products;
        }

        public static bool UpdateProduct(Product p)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE sanpham 
                                 SET TenSP = @TenSP, DonViTinh = @DonViTinh, GiaNhap = @GiaNhap, 
                                     GiaBan = @GiaBan, MaDanhMuc = @MaDanhMuc 
                                 WHERE MaSP = @MaSP";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSP", p.MSP);
                cmd.Parameters.AddWithValue("@TenSP", p.TENSP);
                cmd.Parameters.AddWithValue("@DonViTinh", p.Donvi);
                cmd.Parameters.AddWithValue("@GiaNhap", p.GiaNhap);
                cmd.Parameters.AddWithValue("@GiaBan", p.GiaBan);
                cmd.Parameters.AddWithValue("@MaDanhMuc", p.MADM);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteProduct(string maSP)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM sanpham WHERE MaSP = @MaSP";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSP", maSP);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    // ==================== Lớp Product ====================
    public class Product : INotifyPropertyChanged
    {
        private string _msp;
        private string _tensp;
        private DateTime _ngayNhap;
        private decimal _giaNhap;
        private decimal _giaBan;
        private string _donvi;
        private string _madm;

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

        public string MADM
        {
            get => _madm;
            set { _madm = value; OnPropertyChanged(nameof(MADM)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}