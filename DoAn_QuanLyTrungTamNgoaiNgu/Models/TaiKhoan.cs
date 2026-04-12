namespace DoAn_QuanLyTrungTamNgoaiNgu.Models
{
    public class TaiKhoan
    {
        public string MATK { get; set; }
        public string TENDANGNHAP { get; set; }
        public string MATKHAU { get; set; }
        public string MaNV { get; set; }
        public string MaGV { get; set; }
        public bool TRANGTHAI { get; set; }
        public string HoTenNguoiDung { get; set; }
        public string VaiTro { get; set; } // "NhanVien" hoac "GiaoVien"
    }
}
