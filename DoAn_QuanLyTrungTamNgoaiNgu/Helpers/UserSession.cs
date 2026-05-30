namespace DoAn_QuanLyTrungTamNgoaiNgu.Helpers
{
    public static class UserSession
    {
        public static string MaTK { get; set; }
        public static string HoTen { get; set; }
        public static string VaiTro { get; set; }
        public static string ChucVu 
        { 
            get => VaiTro; 
            set => VaiTro = value; 
        }
        public static string MaNV { get; set; }
        public static string MaGV { get; set; }
    }
}
