using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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
        }

        //-------------------------------------------------
        // LOAD DANH SÁCH SẢN PHẨM
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

                while (rd.Read())
                {
                    comboBox_sanPham.Items.Add(new SanPhamItem()
                    {
                        MaSP = rd["MaSP"].ToString(),
                        TenSP = rd["TenSP"].ToString()
                    });
                }
            }
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
        // KIỂM TRA TRÙNG MÃ PHIẾU
        //-------------------------------------------------
        bool KiemTraMaPhieuTonTai(string ma)
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "select count(*) from phieuxuat where MaPhieuXuat=@ma";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", ma);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        //-------------------------------------------------
        // LOAD CHI TIẾT TỪ DATABASE
        //-------------------------------------------------
        void LoadChiTietFromDB(string maPhieu)
        {
            danhSach.Clear();

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                select ct.MaSP, sp.TenSP, ct.SoLuong, ct.DonGia
                from ct_phieuxuat ct
                join sanpham sp on ct.MaSP = sp.MaSP
                where ct.MaPhieuXuat=@ma";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maPhieu);

                MySqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    danhSach.Add(new ChiTietXuat
                    {
                        MaSP = rd["MaSP"].ToString(),
                        TenSP = rd["TenSP"].ToString(),
                        SoLuong = Convert.ToInt32(rd["SoLuong"]),
                        DonGia = Convert.ToDecimal(rd["DonGia"])
                    });
                }
            }

            TinhTongTien();
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

            SanPhamItem sp = (SanPhamItem)comboBox_sanPham.SelectedItem;

            danhSach.Add(new ChiTietXuat
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                SoLuong = soLuong,
                DonGia = donGia
            });

            TinhTongTien();
        }

        //-------------------------------------------------
        // LƯU PHIẾU
        //-------------------------------------------------
        private void bt_luu_Click(object sender, RoutedEventArgs e)
        {
            if (txt_maPhieuXuat.Text == "")
            {
                MessageBox.Show("Chưa nhập mã phiếu");
                return;
            }

            if (danhSach.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm");
                return;
            }

            if (KiemTraMaPhieuTonTai(txt_maPhieuXuat.Text))
            {
                MessageBox.Show("Mã phiếu đã tồn tại");
                return;
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
                    (MaPhieuXuat, NgayXuat, GhiChu)
                    values
                    (@Ma,@Ngay,@GhiChu)";

                    MySqlCommand cmdPX =
                        new MySqlCommand(sqlPX, conn, tran);

                    cmdPX.Parameters.AddWithValue("@Ma", txt_maPhieuXuat.Text);
                    cmdPX.Parameters.AddWithValue("@Ngay", dp_ngayXuat.SelectedDate);
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

                    LoadChiTietFromDB(txt_maPhieuXuat.Text);
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
            txt_maPhieuXuat.Text = "";
            txt_ghiChu.Text = "";
            dp_ngayXuat.SelectedDate = DateTime.Now;

            txt_soLuong.Text = "Số lượng";
            txt_donGia.Text = "Đơn giá";

            danhSach.Clear();
            txt_tongTien.Text = "";
        }

        //-------------------------------------------------
        // THOÁT
        //-------------------------------------------------
        private void bt_thoat_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //-------------------------------------------------
        // PLACEHOLDER EVENTS
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