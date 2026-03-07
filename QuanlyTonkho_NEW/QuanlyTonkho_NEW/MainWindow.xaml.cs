using QuanlyTonkho_NEW.Repositories;
using QuanlyTonkho_NEW.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QuanlyTonkho_NEW
{
    public partial class MainWindow : Window
    {
        TonKhoRepository repo = new TonKhoRepository();

        List<TonKhoModel> danhSach;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        //--------------------------------
        // LOAD DATA DATABASE
        //--------------------------------
        void LoadData()
        {
            danhSach = repo.GetAll();

            dgvTonKho.ItemsSource = danhSach;
        }

        //--------------------------------
        // TÌM KIẾM
        //--------------------------------
        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtTimKiem.Text.ToLower();

            var ketQua = danhSach
                .Where(x => x.MaSP.ToLower().Contains(keyword)
                         || x.TenSP.ToLower().Contains(keyword))
                .ToList();

            dgvTonKho.ItemsSource = ketQua;
        }

        //--------------------------------
        // THÊM
        //--------------------------------
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng thêm sản phẩm đang phát triển");
        }
    }
}