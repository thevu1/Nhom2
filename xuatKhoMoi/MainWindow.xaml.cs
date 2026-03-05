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
        // TẠO MÃ PHIẾU
        //-------------------------------------------------
        string TaoMaPhieu()
        {
            return "PX" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        //-------------------------------------------------
        // LOAD NHÂN VIÊN
        //-------------------------------------------------
        void LoadNhanVien()
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "SELECT MaNV FROM nhanvien";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rd = cmd.ExecuteReader();

                cb_nhanVien.Items.Clear();

                while (rd.Read())
                {
                    cb_nhanVien.Items.Add(rd["MaNV"].ToString());
                }
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

                string sql = "SELECT MaDanhMuc, TenDanhMuc FROM danhmuc";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rd = cmd.ExecuteReader();

                comboBox_mucLuc.Items.Clear();

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
                comboBox_mucLuc.SelectedIndex = 0;
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

                string sql = @"SELECT MaSP, TenSP, GiaBan
                               FROM sanpham
                               WHERE MaDanhMuc = @ma";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maDanhMuc);

                MySqlDataReader rd = cmd.ExecuteReader();

                comboBox_sanPham.Items.Add(new SanPhamItem()
                {
                    MaSP = "",
                    TenSP = "-- Chọn sản phẩm --",
                    DonGia = 0
                });

                while (rd.Read())
                {
                    comboBox_sanPham.Items.Add(new SanPhamItem()
                    {
                        MaSP = rd["MaSP"].ToString(),
                        TenSP = rd["TenSP"].ToString(),
                        DonGia = Convert.ToDecimal(rd["GiaBan"])
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

            DanhMucItem dm = comboBox_mucLuc.SelectedItem as DanhMucItem;

            if (dm == null)
                return;

            LoadSanPhamTheoDanhMuc(dm.MaDanhMuc);
        }

        //-------------------------------------------------
        // CHỌN SẢN PHẨM -> HIỆN ĐƠN GIÁ
        //-------------------------------------------------
        private void comboBox_sanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_sanPham.SelectedItem == null)
                return;

            SanPhamItem sp = comboBox_sanPham.SelectedItem as SanPhamItem;

            if (sp == null)
                return;

            txt_donGia.Text = sp.DonGia.ToString("N0");
        }
        //-------------------------------------------------
        // THÊM SẢN PHẨM
        //-------------------------------------------------
        private void bt_themSanPham_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_sanPham.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm");
                return;
            }

            if (!int.TryParse(txt_soLuong.Text, out int soLuong))
            {
                MessageBox.Show("Số lượng không hợp lệ");
                return;
            }

            SanPhamItem sp = comboBox_sanPham.SelectedItem as SanPhamItem;

            if (sp == null || sp.MaSP == "")
                return;

            danhSach.Add(new ChiTietXuat()
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                SoLuong = soLuong,
                DonGia = sp.DonGia
            });

            dg_chiTiet.Items.Refresh();

            TinhTongTien();

            txt_soLuong.Text = "Số lượng";
        }

        //-------------------------------------------------
        // TÍNH TỔNG TIỀN
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

            dg_chiTiet.Items.Refresh();
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
                    //---------------- PHIẾU XUẤT ----------------
                    string sqlPX = @"INSERT INTO phieuxuat
                    (MaPhieuXuat, NgayXuat, MaNV)
                    VALUES
                    (@Ma,@Ngay,@NV)";

                    MySqlCommand cmdPX = new MySqlCommand(sqlPX, conn, tran);

                    cmdPX.Parameters.AddWithValue("@Ma", txt_maPhieuXuat.Text);
                    cmdPX.Parameters.AddWithValue("@Ngay", dp_ngayXuat.SelectedDate);
                    cmdPX.Parameters.AddWithValue("@NV", cb_nhanVien.SelectedItem.ToString());

                    cmdPX.ExecuteNonQuery();

                    //---------------- CHI TIẾT ----------------
                    foreach (var item in danhSach)
                    {
                        string sqlCT = @"INSERT INTO ct_phieuxuat
                        (MaPhieuXuat, MaSP, SoLuong, DonGia)
                        VALUES
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

        //-------------------------------------------------
        // TẠO MỚI
        //-------------------------------------------------
        private void bt_taoMoi_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        //-------------------------------------------------
        // THOÁT
        //-------------------------------------------------
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