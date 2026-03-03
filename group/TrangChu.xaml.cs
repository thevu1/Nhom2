using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace group
{
    public partial class TrangChu : Window
    {
        string TenDangNhap;
        string Role;

        public TrangChu(string ten, string role)
        {
            InitializeComponent();

            TenDangNhap = ten;
            Role = role;

            LoadDanhMuc();
        }

        // ================= LOAD DANH MỤC =================
        void LoadDanhMuc()
        {
            btnBack.Visibility = Visibility.Collapsed;

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT * FROM danhmuc";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rd = cmd.ExecuteReader();

                    WrapPanelProducts.Children.Clear();

                    while (rd.Read())
                    {
                        string ma = rd["MaDanhMuc"].ToString();
                        string ten = rd["TenDanhMuc"].ToString();
                        string imgPath = rd["HinhAnh"].ToString();

                        Border card = new Border()
                        {
                            Width = 160,
                            Margin = new Thickness(10),
                            Padding = new Thickness(10),
                            Background = System.Windows.Media.Brushes.White,
                            CornerRadius = new CornerRadius(10)
                        };

                        StackPanel sp = new StackPanel();

                        Image img = new Image()
                        {
                            Height = 100
                        };

                        try
                        {
                            img.Source = new BitmapImage(
                                new Uri(imgPath, UriKind.RelativeOrAbsolute));
                        }
                        catch
                        {
                            img.Source = new BitmapImage(
                                new Uri("pack://application:,,,/Images/default.png"));
                        }

                        TextBlock txt = new TextBlock()
                        {
                            Text = ten,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            FontWeight = FontWeights.Bold
                        };

                        sp.Children.Add(img);
                        sp.Children.Add(txt);

                        card.Child = sp;

                        card.MouseDown += (s, e) =>
                        {
                            LoadSanPhamTheoDanhMuc(ma);
                        };

                        WrapPanelProducts.Children.Add(card);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= LOAD SẢN PHẨM =================
        void LoadSanPhamTheoDanhMuc(string maDanhMuc)
        {
            btnBack.Visibility = Visibility.Visible;

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();

                    string sql =
                        "SELECT * FROM sanpham WHERE MaDanhMuc=@ma";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ma", maDanhMuc);

                    MySqlDataReader rd = cmd.ExecuteReader();

                    WrapPanelProducts.Children.Clear();

                    while (rd.Read())
                    {
                        string ten = rd["TenSP"].ToString();
                        string imgPath = rd["HinhAnh"].ToString();

                        Border card = new Border()
                        {
                            Width = 160,
                            Margin = new Thickness(10),
                            Padding = new Thickness(10),
                            Background = System.Windows.Media.Brushes.White,
                            CornerRadius = new CornerRadius(10)
                        };

                        StackPanel sp = new StackPanel();

                        Image img = new Image()
                        {
                            Height = 100,
                            Source = new BitmapImage(
                                new Uri(imgPath, UriKind.RelativeOrAbsolute))
                        };

                        TextBlock txt = new TextBlock()
                        {
                            Text = ten,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            FontWeight = FontWeights.Bold
                        };

                        sp.Children.Add(img);
                        sp.Children.Add(txt);

                        card.Child = sp;

                        WrapPanelProducts.Children.Add(card);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= CHỌN ẢNH =================
        private void BtnChonAnh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Image Files|*.jpg;*.png;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                txtHinhAnh.Text = dlg.FileName;
            }
        }
        // ================= QUAY LẠI ===================
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            LoadDanhMuc();
            btnBack.Visibility = Visibility.Collapsed;
        }
        // ================= AUTO MÃ DANH MỤC =================
        string TaoMaDanhMuc(MySqlConnection conn)
        {
            string sql =
                "SELECT MaDanhMuc FROM danhmuc ORDER BY MaDanhMuc DESC LIMIT 1";

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            object result = cmd.ExecuteScalar();

            if (result == null)
                return "MD00";

            string last = result.ToString().Substring(2);

            int number = int.Parse(last) + 1;

            return "MD" + number.ToString("00");
        }

        // ================= THÊM DANH MỤC =================
        private void BtnThemSP_Click(object sender, RoutedEventArgs e)
        {
            string ten = txtTenHangMuc.Text.Trim();
            string img = txtHinhAnh.Text.Trim();

            if (ten == "")
            {
                MessageBox.Show("Nhập tên danh mục");
                return;
            }

            if (img == "")
            {
                img = "pack://application:,,,/Images/default.png";
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();

                    string ma = TaoMaDanhMuc(conn);

                    string sql =
                        @"INSERT INTO danhmuc(MaDanhMuc,TenDanhMuc,HinhAnh)
                          VALUES(@ma,@ten,@img)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@ma", ma);
                    cmd.Parameters.AddWithValue("@ten", ten);
                    cmd.Parameters.AddWithValue("@img", img);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thêm danh mục thành công");

                    LoadDanhMuc();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}