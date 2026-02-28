using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace group
{
    public partial class TrangChu : Window
    {
        public TrangChu()
        {
            InitializeComponent();
        }

        private void BtnSanPham_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnTonKho_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnNhap_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnXuat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnQuanLySanPham_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnThemSP_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm");
                return;
            }

            string imagePath = string.IsNullOrWhiteSpace(txtHinhAnh.Text)
                ? "/Images/default.png"
                : txtHinhAnh.Text;

            // Tạo card sản phẩm mới
            Border border = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                Width = 160
            };

            StackPanel stack = new StackPanel();

            Image img = new Image
            {
                Height = 100,
                Stretch = System.Windows.Media.Stretch.Uniform
            };

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                if (System.IO.File.Exists(imagePath))
                {
                    // Ảnh từ ổ đĩa (jpg, png, jpeg đều được)
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                }
                else
                {
                    // Ảnh trong project (Resource)
                    bitmap.UriSource = new Uri(
                        $"pack://application:,,,/{imagePath.TrimStart('/')}",
                        UriKind.Absolute);
                }

                bitmap.EndInit();
                img.Source = bitmap;
            }
            catch
            {
                img.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Images/default.png", UriKind.Absolute));
                //}

                TextBlock txt = new TextBlock
                {
                    Text = txtTenSP.Text,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                stack.Children.Add(img);
                stack.Children.Add(txt);
                border.Child = stack;

                WrapPanelProducts.Children.Add(border);

                txtTenSP.Clear();
                txtHinhAnh.Clear();
            }
        }
        private void TxtHinhAnh_TextChanged(object sender, TextChangedEventArgs e)
        {
               
        }
    }
}