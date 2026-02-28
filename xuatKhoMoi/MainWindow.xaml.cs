using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace xuatKhoMoi
{
    public partial class MainWindow : Window
    {
        ObservableCollection<ChiTietXuat> danhSach =
            new ObservableCollection<ChiTietXuat>();

        public MainWindow()
        {
            InitializeComponent();

            dg_chiTiet.ItemsSource = danhSach;
            dp_ngayXuat.SelectedDate = DateTime.Now;

            LoadDanhMuc();
            LoadNhanVien();

            txt_maPhieuXuat.Text = TaoMaPhieu();
        }

        //-------------------------------------------------
        // AUTO MÃ PHIẾU
        //-------------------------------------------------
        string TaoMaPhieu()
        {
            return "PX" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        //-------------------------------------------------
        // LOAD NHÂN VIÊN
        //-------------------------------------------------
        void LoadNhanVien()
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "select MaNV from nhanvien";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rd = cmd.ExecuteReader();

                cb_nhanVien.Items.Clear();

                while (rd.Read())
                    cb_nhanVien.Items.Add(rd["MaNV"].ToString());
            }
        }

        //-------------------------------------------------
        // LOAD DANH MỤC
        //-------------------------------------------------
        void LoadDanhMuc()
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "select MaDanhMuc, TenDanhMuc from danhmuc";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rd = cmd.ExecuteReader();

                comboBox_mucLuc.Items.Clear();
                // Placeholder
                comboBox_mucLuc.Items.Add(new DanhMucItem()
                {
                    MaDanhMuc = "",
                    TenDanhMuc = "-- Chọn danh mục --"
                });
                while (rd.Read())
                {
                    comboBox_mucLuc.Items.Add(new DanhMucItem()
                    {
                        MaDanhMuc = rd["MaDanhMuc"].ToString(),
                        TenDanhMuc = rd["TenDanhMuc"].ToString()
                    });
                }

                comboBox_mucLuc.DisplayMemberPath = "TenDanhMuc";
                comboBox_mucLuc.SelectedIndex = 0;   // chọn dòng placeholder
            }
        }

        //-------------------------------------------------
        // LOAD SẢN PHẨM THEO DANH MỤC
        //-------------------------------------------------
        void LoadSanPhamTheoDanhMuc(string maDanhMuc)
        {
            comboBox_sanPham.Items.Clear();

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT MaSP, TenSP
                               FROM sanpham
                               WHERE MaDanhMuc=@ma";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maDanhMuc);

                MySqlDataReader rd = cmd.ExecuteReader();

                comboBox_sanPham.Items.Clear();

                // Placeholder
                comboBox_sanPham.Items.Add(new SanPhamItem()
                {
                    MaSP = "",
                    TenSP = "-- Chọn sản phẩm --"
                });
                while (rd.Read())
                {
                    comboBox_sanPham.Items.Add(new SanPhamItem()
                    {
                        MaSP = rd["MaSP"].ToString(),
                        TenSP = rd["TenSP"].ToString()
                    });
                }

            }

            comboBox_sanPham.DisplayMemberPath = "TenSP";
            comboBox_sanPham.SelectedIndex = 0;

        }

        //-------------------------------------------------
        // CHỌN DANH MỤC
        //-------------------------------------------------
        private void comboBox_mucLuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_mucLuc.SelectedItem == null)
                return;

            DanhMucItem dm =
                comboBox_mucLuc.SelectedItem as DanhMucItem;

            if (dm == null)
                return;

            LoadSanPhamTheoDanhMuc(dm.MaDanhMuc);
        }

        //-------------------------------------------------
        // THÊM SẢN PHẨM
        //-------------------------------------------------
        private void bt_themSanPham_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_sanPham.SelectedItem == null)
            {
                MessageBox.Show("Chọn sản phẩm");
                return;
            }

            if (!int.TryParse(txt_soLuong.Text, out int soLuong))
            {
                MessageBox.Show("Sai số lượng");
                return;
            }

            if (!decimal.TryParse(txt_donGia.Text, out decimal donGia))
            {
                MessageBox.Show("Sai đơn giá");
                return;
            }

            SanPhamItem sp =
                (SanPhamItem)comboBox_sanPham.SelectedItem;

            danhSach.Add(new ChiTietXuat()
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                SoLuong = soLuong,
                DonGia = donGia
            });

            TinhTongTien();
        }

        //-------------------------------------------------
        // TÍNH TỔNG
        //-------------------------------------------------
        void TinhTongTien()
        {
            decimal tong = danhSach.Sum(x => x.ThanhTien);
            txt_tongTien.Text = tong.ToString("N0");
        }

        //-------------------------------------------------
        // RESET FORM
        //-------------------------------------------------
        void ResetForm()
        {
            danhSach.Clear();
            txt_maPhieuXuat.Text = TaoMaPhieu();
            txt_tongTien.Text = "";
        }

        //-------------------------------------------------
        // LƯU PHIẾU
        //-------------------------------------------------
        private void bt_luu_Click(object sender, RoutedEventArgs e)
        {
            if (danhSach.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm");
                return;
            }

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                MySqlTransaction tran = conn.BeginTransaction();

                try
                {
                    //-----------------------------------
                    // PHIẾU
                    //-----------------------------------
                    string sqlPX = @"insert into phieuxuat
                    (MaPhieuXuat, NgayXuat, MaNV)
                    values
                    (@Ma,@Ngay,@NV)";

                    MySqlCommand cmdPX =
                        new MySqlCommand(sqlPX, conn, tran);

                    cmdPX.Parameters.AddWithValue("@Ma", txt_maPhieuXuat.Text);
                    cmdPX.Parameters.AddWithValue("@Ngay", dp_ngayXuat.SelectedDate);
                    cmdPX.Parameters.AddWithValue("@NV", cb_nhanVien.SelectedItem.ToString());

                    cmdPX.ExecuteNonQuery();

                    //-----------------------------------
                    // CHI TIẾT
                    //-----------------------------------
                    foreach (var item in danhSach)
                    {
                        string sqlCT = @"insert into ct_phieuxuat
                        (MaPhieuXuat, MaSP, SoLuong, DonGia)
                        values
                        (@Ma,@SP,@SL,@DG)";

                        MySqlCommand cmdCT =
                            new MySqlCommand(sqlCT, conn, tran);

                        cmdCT.Parameters.AddWithValue("@Ma", txt_maPhieuXuat.Text);
                        cmdCT.Parameters.AddWithValue("@SP", item.MaSP);
                        cmdCT.Parameters.AddWithValue("@SL", item.SoLuong);
                        cmdCT.Parameters.AddWithValue("@DG", item.DonGia);

                        cmdCT.ExecuteNonQuery();
                    }

                    tran.Commit();
                    MessageBox.Show("Lưu thành công");

                    ResetForm();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void bt_taoMoi_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void bt_thoat_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //-------------------------------------------------
        // PLACEHOLDER SỐ LƯỢNG
        //-------------------------------------------------
        private void txt_soLuong_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_soLuong.Text == "Số lượng")
                txt_soLuong.Text = "";
        }

        private void txt_soLuong_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_soLuong.Text == "")
                txt_soLuong.Text = "Số lượng";
        }

        //-------------------------------------------------
        // PLACEHOLDER ĐƠN GIÁ
        //-------------------------------------------------
        private void txt_donGia_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_donGia.Text == "Đơn giá")
                txt_donGia.Text = "";
        }

        private void txt_donGia_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_donGia.Text == "")
                txt_donGia.Text = "Đơn giá";
        }
    }

    
}