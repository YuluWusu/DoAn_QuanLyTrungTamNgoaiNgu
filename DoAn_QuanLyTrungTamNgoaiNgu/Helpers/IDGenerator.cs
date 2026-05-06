using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Helpers
{
    public static class IDGenerator
    {
        public static string GenerateNextID(string prefix, string lastID)
        {
            // Nếu bảng chưa có dữ liệu nào, bắt đầu từ 001
            if (string.IsNullOrEmpty(lastID))
            {
                return prefix + "001";
            }

            
            string numberPart = lastID.Replace(prefix, "");

            if (int.TryParse(numberPart, out int number))
            {
                number++; // Tăng giá trị lên 1
                return prefix + number.ToString("D3");
            }

            return prefix + "001";
        }
    }
}
