using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quản_Lý_Kho_Hàng
{
    internal class ChiTietPhieuNhap
    {
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public decimal ThanhTien
        {
            get { return SoLuong * DonGia; }
        }
    }
}
