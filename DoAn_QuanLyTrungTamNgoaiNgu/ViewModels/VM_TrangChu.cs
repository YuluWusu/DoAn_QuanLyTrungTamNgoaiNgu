using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    internal class VM_TrangChu:BaseViewModel
    {
        QL_TRUNGTAM_TIENGANH data;
        private string _hoTenHienThi;
        public string HoTenHienThi
        {
            get => _hoTenHienThi;
            set
            {
                _hoTenHienThi = value;
                OnPropertyChanged();
            }
        }
        private string _vaiTroHienThi;
        public string VaiTroHienThi
        {
            get => _vaiTroHienThi;
            set
            {
                _vaiTroHienThi= value;
                OnPropertyChanged();
            }
        }
        private object _noiDungHienTai;
        public object NoiDungHienTai
        {
            get => _noiDungHienTai;
            set
            {
                _noiDungHienTai = value;
                OnPropertyChanged();
            }
        }
        public ICommand NavTrangChu {  get; set; }
        public ICommand NavHocVien { get; set; }
        public ICommand NavLopHoc { get; set; }
        public ICommand NavDangKyLop { get; set; }
        public ICommand NavDiemDanh { get; set; }
        public ICommand NavHocPhi { get; set; }
        public ICommand NavBaoCao { get; set; }
        public ICommand NavTaiKhoan { get; set; }
        public ICommand DangXuatCommand { get; set; }
        public VM_TrangChu()
        {
            DangXuatCommand = new RelayCommand<object>((p) => true, (p) =>
            {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn đăng xuất tài khoản này không?",
        "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DangNhap loginWin = new DangNhap();
                    loginWin.Show();
                    foreach (Window item in Application.Current.Windows)
                    {
                        if (item.DataContext == this)
                        {
                            item.Tag = "IsLoggingOut";
                            item.Close();
                            break;
                        }
                    }
                }
            });
            data = new QL_TRUNGTAM_TIENGANH();
            NoiDungHienTai = new UC_TrangChu();
            NavTrangChu = new RelayCommand<object>((p) => true, (p) => { NoiDungHienTai = new UC_TrangChu(); });

            NavTrangChu = new RelayCommand<object>((p) => true, (p) => { NoiDungHienTai = new UC_TrangChu(); });

            // 3. Nút Đăng ký lớp
            NavDangKyLop = new RelayCommand<object>((p) => true, (p) => 
            { 
                var vmDangKy = new VM_DangKy();
        
                // Bắt sự kiện khi bấm nút "+ THÊM ĐĂNG KÝ" trong UC_DanhSachDangKy
                vmDangKy.OnRequestAddRegistration = () => 
                {
                    var vmDangKyMoi = new VM_DangKyMoi();
                    // Bắt sự kiện khi bấm nút "Quay lại" hoặc "Hủy" trong form thêm mới
                    vmDangKyMoi.RequestNavigateBack = () => NoiDungHienTai = vmDangKy;
                    NoiDungHienTai = vmDangKyMoi;
                };
        
                NoiDungHienTai = vmDangKy; 
            });
            NavHocPhi = new RelayCommand<object>((p) => true, (p) => 
            { 
                var vmDanhSachHocPhi = new VM_DanhSachHocPhi();
        
                // Bắt sự kiện khi bấm nút "+ THÊM KHÓA HỌC" trong UC_DanhSachHocPhi
                vmDanhSachHocPhi.OnRequestAddKhoaHoc = () =>
                {
                    var vmThemHocPhi = new VM_ThemHocPhi();
                    // Bắt sự kiện khi bấm nút "Quay lại" trong form thêm khóa học
                    vmThemHocPhi.RequestNavigateBack = () => NoiDungHienTai = vmDanhSachHocPhi;
                    NoiDungHienTai = vmThemHocPhi; 
                };
        
                NoiDungHienTai = vmDanhSachHocPhi; 
            });
        }
        public void LoadThongTinNguoiDung(string maTK)
        {
            var tk = data.TAIKHOANs.FirstOrDefault(x => x.MATK == maTK);
            if(tk!=null)
            {
                if(tk.MaNV != null)
                {
                    HoTenHienThi = tk.NHANVIEN.HoTen;
                    VaiTroHienThi = tk.NHANVIEN.ChucVu;
                }
                else if(tk.MaGV != null)
                {
                    HoTenHienThi = tk.GIAOVIEN.TenGV;
                    VaiTroHienThi = "Giáo viên";
                }
            }
        }
    }
}
