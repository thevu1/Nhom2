using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.VisualBasic;

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

            LoadSanPham();
            LoadNhanVien();

            txt_maPhieuXuat.Text = TaoMaPhieu(); // auto mã
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
                {
                    cb_nhanVien.Items.Add(rd["MaNV"].ToString());
                }
            }
        }

        //-------------------------------------------------
        // LOAD SẢN PHẨM
        //-------------------------------------------------
        void LoadSanPham()
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "select MaSP, TenSP from sanpham";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rd = cmd.ExecuteReader();

                comboBox_sanPham.Items.Clear();

                // placeholder
                comboBox_sanPham.Items.Add(new SanPhamItem()
                {
                    MaSP = "",
                    TenSP = "-- Lựa chọn sản phẩm --"
                });

                while (rd.Read())
                {
                    comboBox_sanPham.Items.Add(new SanPhamItem()
                    {
                        MaSP = rd["MaSP"].ToString(),
                        TenSP = rd["TenSP"].ToString()
                    });
                }

                comboBox_sanPham.DisplayMemberPath = "TenSP";
                comboBox_sanPham.SelectedIndex = 0;
            }
        }

        //-------------------------------------------------
        // KIỂM TRA KHÁCH HÀNG
        //-------------------------------------------------
        bool KiemTraKhachHang(string maKH)
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "select count(*) from khachhang where MaKH=@ma";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maKH);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        //-------------------------------------------------
        // THÊM KHÁCH HÀNG
        //-------------------------------------------------
        void ThemKhachHangMoi(string maKH, string tenKH)
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = @"insert into khachhang
                               (MaKH, TenKH)
                               values(@ma,@ten)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maKH);
                cmd.Parameters.AddWithValue("@ten", tenKH);

                cmd.ExecuteNonQuery();
            }
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
            txt_maPhieuXuat.Text = TaoMaPhieu();

            txt_khachHang.Text = "";
            txt_ghiChu.Text = "";

            txt_soLuong.Text = "Số lượng";
            txt_donGia.Text = "Đơn giá";

            txt_tongTien.Text = "";

            danhSach.Clear();
            comboBox_sanPham.SelectedIndex = 0;
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

            SanPhamItem sp = (SanPhamItem)comboBox_sanPham.SelectedItem;

            if (sp.MaSP == "")
            {
                MessageBox.Show("Chưa chọn sản phẩm");
                return;
            }

            if (txt_soLuong.Text == "Số lượng" ||
                !int.TryParse(txt_soLuong.Text, out int soLuong))
            {
                MessageBox.Show("Sai số lượng");
                return;
            }

            if (txt_donGia.Text == "Đơn giá" ||
                !decimal.TryParse(txt_donGia.Text, out decimal donGia))
            {
                MessageBox.Show("Sai đơn giá");
                return;
            }

            danhSach.Add(new ChiTietXuat
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                SoLuong = soLuong,
                DonGia = donGia
            });

            TinhTongTien();

            txt_soLuong.Text = "Số lượng";
            txt_donGia.Text = "Đơn giá";
            comboBox_sanPham.SelectedIndex = 0;
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

            if (cb_nhanVien.SelectedItem == null)
            {
                MessageBox.Show("Chọn nhân viên");
                return;
            }

            if (txt_khachHang.Text == "")
            {
                MessageBox.Show("Nhập khách hàng");
                return;
            }

            //---------------------------------------
            // tạo khách hàng nếu chưa tồn tại
            //---------------------------------------
            if (!KiemTraKhachHang(txt_khachHang.Text))
            {
                var result = MessageBox.Show(
                    "Khách hàng chưa tồn tại. Tạo mới?",
                    "Thông báo",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                    return;

                string tenKH = Interaction.InputBox(
                    "Nhập tên khách hàng:",
                    "Tạo khách hàng mới",
                    "");

                if (tenKH.Trim() == "")
                {
                    MessageBox.Show("Chưa nhập tên khách hàng");
                    return;
                }

                ThemKhachHangMoi(txt_khachHang.Text, tenKH);
            }

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                MySqlTransaction tran = conn.BeginTransaction();

                try
                {
                    //---------------------------------------
                    // insert phiếu
                    //---------------------------------------
                    string sqlPX = @"insert into phieuxuat
                    (MaPhieuXuat, NgayXuat, MaNV, MaKH, GhiChu)
                    values
                    (@Ma,@Ngay,@MaNV,@MaKH,@GhiChu)";

                    MySqlCommand cmdPX =
                        new MySqlCommand(sqlPX, conn, tran);

                    cmdPX.Parameters.AddWithValue("@Ma", txt_maPhieuXuat.Text);
                    cmdPX.Parameters.AddWithValue("@Ngay", dp_ngayXuat.SelectedDate);
                    cmdPX.Parameters.AddWithValue("@MaNV", cb_nhanVien.SelectedItem.ToString());
                    cmdPX.Parameters.AddWithValue("@MaKH", txt_khachHang.Text);
                    cmdPX.Parameters.AddWithValue("@GhiChu", txt_ghiChu.Text);

                    cmdPX.ExecuteNonQuery();

                    //---------------------------------------
                    // insert chi tiết
                    //---------------------------------------
                    foreach (var item in danhSach)
                    {
                        string sqlCT = @"insert into ct_phieuxuat
                        (MaPhieuXuat, MaSP, SoLuong, DonGia)
                        values
                        (@Ma,@MaSP,@SL,@DG)";

                        MySqlCommand cmdCT =
                            new MySqlCommand(sqlCT, conn, tran);

                        cmdCT.Parameters.AddWithValue("@Ma", txt_maPhieuXuat.Text);
                        cmdCT.Parameters.AddWithValue("@MaSP", item.MaSP);
                        cmdCT.Parameters.AddWithValue("@SL", item.SoLuong);
                        cmdCT.Parameters.AddWithValue("@DG", item.DonGia);

                        cmdCT.ExecuteNonQuery();
                    }

                    tran.Commit();

                    MessageBox.Show("Lưu thành công");

                    ResetForm();   // cực quan trọng
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
        // PLACEHOLDER
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