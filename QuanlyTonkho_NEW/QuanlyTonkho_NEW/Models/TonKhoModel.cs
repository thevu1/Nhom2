namespace QuanlyTonkho_NEW.Models
{
    public class TonKhoModel
    {
        public string MaSP { get; set; }

        public string TenSP { get; set; }

        public string LoaiHang { get; set; }

        public int TongNhap { get; set; }

        public int TongXuat { get; set; }

        public int TonHienTai
        {
            get { return TongNhap - TongXuat; }
        }
    }
}