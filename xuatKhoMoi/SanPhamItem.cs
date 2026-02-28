using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xuatKhoMoi
{
    public class SanPhamItem
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }

        public override string ToString()
        {
            return TenSP;
        }
    }
}
