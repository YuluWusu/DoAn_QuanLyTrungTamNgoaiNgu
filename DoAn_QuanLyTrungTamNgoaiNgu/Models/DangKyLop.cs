using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Models
{
    public class DangKyLop
    {
        public string MaHV { get; set; }
        public string MaLop { get; set; }

        public string HoTenHV { get; set; }
        public string TenLop { get; set; }
        public DateTime NgayDK { get; set; }
        public decimal HocPhi { get; set; }
        public string TrangThai { get; set; }
    }
}
