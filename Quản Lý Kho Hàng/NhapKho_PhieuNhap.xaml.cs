
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace Quản_Lý_Kho_Hàng
{
    public partial class NhapKho_PhieuNhap : Window
    {
        // Danh sách chi tiết
        private ObservableCollection<CTPhieuNhapItem> _chiTiet = new ObservableCollection<CTPhieuNhapItem>();

        public NhapKho_PhieuNhap()
        {
            InitializeComponent();

            // Tự tính thành tiền khi nhập
            txtSoLuong.TextChanged += (_, __) => TinhThanhTienTam();
            txtDonGia.TextChanged += (_, __) => TinhThanhTienTam();
        }

        // ================== THÊM ==================
        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (cboSanPham.SelectedItem == null && string.IsNullOrWhiteSpace(cboSanPham.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm.");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.");
                return;
            }

            if (!decimal.TryParse(txtDonGia.Text.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out decimal donGia) || donGia <= 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ.");
                return;
            }

            string tenSP = cboSanPham.Text.Trim();
            decimal thanhTien = soLuong * donGia;

            // Nếu thêm trùng sản phẩm + đơn giá => cộng dồn
            var existed = _chiTiet.FirstOrDefault(x => x.TenSanPham == tenSP && x.DonGia == donGia);
            if (existed != null)
            {
                existed.SoLuong += soLuong;
                existed.ThanhTien = existed.SoLuong * existed.DonGia;
            }
            else
            {
                _chiTiet.Add(new CTPhieuNhapItem
                {
                    TenSanPham = tenSP,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    ThanhTien = thanhTien
                });
            }

            txtThanhTien.Text = thanhTien.ToString("N0", CultureInfo.CurrentCulture);

            CapNhatTongTien();
            ResetNhapChiTiet();
        }

        // ================== XÓA ==================
        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (_chiTiet.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm để xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm cuối cùng?",
                "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _chiTiet.RemoveAt(_chiTiet.Count - 1);
                CapNhatTongTien();
            }
        }

        // ================== LƯU PHIẾU ==================
        private void btnLuuPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboNCC.Text))
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp.");
                return;
            }

            if (string.IsNullOrWhiteSpace(cboNguoiNhap.Text))
            {
                MessageBox.Show("Vui lòng chọn Người nhập.");
                return;
            }

            if (dpNgayNhap.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn Ngày nhập.");
                return;
            }

            if (_chiTiet.Count == 0)
            {
                MessageBox.Show("Phiếu nhập phải có ít nhất 1 dòng chi tiết.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMaPN.Text))
                txtMaPN.Text = TaoMaPhieu();

            decimal tongTien = _chiTiet.Sum(x => x.ThanhTien);

            var phieu = new PhieuNhapDto
            {
                MaPN = txtMaPN.Text.Trim(),
                NhaCungCap = cboNCC.Text.Trim(),
                NguoiNhap = cboNguoiNhap.Text.Trim(),
                NgayNhap = dpNgayNhap.SelectedDate.Value,
                GhiChu = txtGhiChu.Text.Trim(),
                TongTien = tongTien,
                ChiTiet = _chiTiet.ToList()
            };

            LuuPhieuJson(phieu);
            MessageBox.Show("Lưu phiếu thành công!");
        }

        // ================== HÀM PHỤ ==================
        private void CapNhatTongTien()
        {
            decimal tong = _chiTiet.Sum(x => x.ThanhTien);
            txtTongTien.Text = tong.ToString("N0", CultureInfo.CurrentCulture);
        }

        private void ResetNhapChiTiet()
        {
            cboSanPham.SelectedIndex = -1;
            txtSoLuong.Clear();
            txtDonGia.Clear();
            txtThanhTien.Clear();
            cboSanPham.Focus();
        }

        private void TinhThanhTienTam()
        {
            if (int.TryParse(txtSoLuong.Text.Trim(), out int sl) &&
                decimal.TryParse(txtDonGia.Text.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out decimal dg))
            {
                txtThanhTien.Text = (sl * dg).ToString("N0", CultureInfo.CurrentCulture);
            }
            else
            {
                txtThanhTien.Clear();
            }
        }

        private string TaoMaPhieu()
        {
            return "PN" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        // ================== MODEL ==================
        public class CTPhieuNhapItem
        {
            public string TenSanPham { get; set; } = "";
            public int SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public class PhieuNhapDto
        {
            public string MaPN { get; set; } = "";
            public string NhaCungCap { get; set; } = "";
            public string NguoiNhap { get; set; } = "";
            public DateTime NgayNhap { get; set; }
            public string GhiChu { get; set; } = "";
            public decimal TongTien { get; set; }
            public List<CTPhieuNhapItem> ChiTiet { get; set; } = new List<CTPhieuNhapItem>();
        }

        // ================== LƯU JSON (Newtonsoft) ==================
        private void LuuPhieuJson(PhieuNhapDto phieu)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PhieuNhap");
            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, $"{phieu.MaPN}.txt");

            // ghi đơn giản để khỏi cần Json library
            File.WriteAllText(path, $"MaPN: {phieu.MaPN}\nTongTien: {phieu.TongTien}");
        }

        private void btnIn_Click(object sender, RoutedEventArgs e)
        {
            if (_chiTiet.Count == 0)
            {
                MessageBox.Show("Chưa có chi tiết để in.");
                return;
            }

            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() != true) return;

            FlowDocument doc = TaoPhieuIn();

            doc.PageHeight = dlg.PrintableAreaHeight;
            doc.PageWidth = dlg.PrintableAreaWidth;
            doc.PagePadding = new Thickness(40);
            doc.ColumnWidth = dlg.PrintableAreaWidth;

            IDocumentPaginatorSource idp = doc;
            dlg.PrintDocument(idp.DocumentPaginator, "Phiếu nhập kho");
        }
        private FlowDocument TaoPhieuIn()
        {
            FlowDocument doc = new FlowDocument();

            // ===== TIÊU ĐỀ =====
            doc.Blocks.Add(new Paragraph(new Run("PHIẾU NHẬP KHO"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            });

            // ===== THÔNG TIN =====
            doc.Blocks.Add(new Paragraph(new Run($"Mã phiếu: {txtMaPN.Text}")));
            doc.Blocks.Add(new Paragraph(new Run($"Nhà cung cấp: {cboNCC.Text}")));
            doc.Blocks.Add(new Paragraph(new Run($"Người nhập: {cboNguoiNhap.Text}")));
            doc.Blocks.Add(new Paragraph(new Run($"Ngày nhập: {dpNgayNhap.SelectedDate:dd/MM/yyyy}")));

            if (!string.IsNullOrWhiteSpace(txtGhiChu.Text))
                doc.Blocks.Add(new Paragraph(new Run($"Ghi chú: {txtGhiChu.Text}")));

            doc.Blocks.Add(new Paragraph(new Run(" ")));

            // ===== BẢNG =====
            Table table = new Table();
            table.CellSpacing = 0;

            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(60) });
            table.Columns.Add(new TableColumn { Width = new GridLength(120) });
            table.Columns.Add(new TableColumn { Width = new GridLength(140) });

            TableRowGroup group = new TableRowGroup();
            table.RowGroups.Add(group);

            // Header
            TableRow header = new TableRow();
            header.Cells.Add(MakeCell("Sản phẩm", true));
            header.Cells.Add(MakeCell("SL", true));
            header.Cells.Add(MakeCell("Đơn giá", true));
            header.Cells.Add(MakeCell("Thành tiền", true));
            group.Rows.Add(header);

            // Data
            foreach (var it in _chiTiet)
            {
                TableRow row = new TableRow();
                row.Cells.Add(MakeCell(it.TenSanPham));
                row.Cells.Add(MakeCell(it.SoLuong.ToString()));
                row.Cells.Add(MakeCell(it.DonGia.ToString("N0", CultureInfo.CurrentCulture)));
                row.Cells.Add(MakeCell(it.ThanhTien.ToString("N0", CultureInfo.CurrentCulture)));
                group.Rows.Add(row);
            }

            doc.Blocks.Add(table);

            // ===== TỔNG TIỀN =====
            decimal tong = _chiTiet.Sum(x => x.ThanhTien);

            doc.Blocks.Add(new Paragraph(new Run($"TỔNG TIỀN: {tong.ToString("N0", CultureInfo.CurrentCulture)}"))
            {
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            });

            return doc;
        }
        // local function tạo cell
        private TableCell MakeCell(string text, bool bold = false)
        {
            return new TableCell(new Paragraph(new Run(text)))
            {
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.5),
                Padding = new Thickness(6)
            };
        }
        private void btnTaoMoi_Click(object sender, RoutedEventArgs e)
        {
            // Reset form (tùy bạn muốn reset gì)
            cboNCC.SelectedIndex = -1;
            cboNguoiNhap.SelectedIndex = -1;
            dpNgayNhap.SelectedDate = DateTime.Today;
            txtGhiChu.Clear();

            cboSanPham.SelectedIndex = -1;
            txtSoLuong.Clear();
            txtDonGia.Clear();
            txtThanhTien.Clear();

            _chiTiet.Clear();
            txtTongTien.Text = "0";
        }
        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?",
                        "Xác nhận",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
    }
