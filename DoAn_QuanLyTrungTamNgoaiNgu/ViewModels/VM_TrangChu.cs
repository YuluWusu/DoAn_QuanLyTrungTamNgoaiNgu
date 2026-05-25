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
                if (!CanNavigateAway()) return;

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
            NavTrangChu = new RelayCommand<object>((p) => true, (p) => { NavigateTo(new UC_TrangChu()); });

            // 3. Nút Đăng ký lớp
            NavDangKyLop = new RelayCommand<object>((p) => true, (p) => 
            { 
                var vmDangKy = new VM_DangKy();
        
                // Bắt sự kiện khi bấm nút "+ THÊM ĐĂNG KÝ" trong UC_DanhSachDangKy
                vmDangKy.OnRequestAddRegistration = () => 
                {
                    var vmDangKyMoi = new VM_DangKyMoi();
                    // Bắt sự kiện khi bấm nút "Quay lại" hoặc "Hủy" trong form thêm mới
                    vmDangKyMoi.RequestNavigateBack = () => NavigateTo(vmDangKy);
                    NavigateTo(vmDangKyMoi);
                };
        
                NavigateTo(vmDangKy); 
            });
            NavHocPhi = new RelayCommand<object>((p) => true, (p) => 
            { 
                var vmDanhSachHocPhi = new VM_DanhSachHocPhi();
        
                // Bắt sự kiện khi bấm nút "+ THÊM KHÓA HỌC" trong UC_DanhSachHocPhi
                vmDanhSachHocPhi.OnRequestAddKhoaHoc = () =>
                {
                    var vmThemHocPhi = new VM_ThemHocPhi();
                    // Bắt sự kiện khi bấm nút "Quay lại" trong form thêm khóa học
                    vmThemHocPhi.RequestNavigateBack = () => 
                    {
                        vmDanhSachHocPhi.LoadData();
                        NavigateTo(vmDanhSachHocPhi);
                    };
                    NavigateTo(vmThemHocPhi); 
                };
        
                NavigateTo(vmDanhSachHocPhi); 
            });
        }

        private void NavigateTo(object newContent)
        {
            if (CanNavigateAway())
            {
                NoiDungHienTai = newContent;
            }
        }

        private bool CanNavigateAway()
        {
            if (NoiDungHienTai is VM_DangKy vmDangKy && vmDangKy.IsEditing)
            {
                var result = MessageBox.Show("Bạn đang trong chế độ chỉnh sửa Đăng ký. Bạn có muốn LƯU các thay đổi trước khi chuyển trang không?\n\n- Chọn 'Yes' để lưu và chuyển trang.\n- Chọn 'No' để hủy thay đổi và chuyển trang.\n- Chọn 'Cancel' để ở lại.", "Xác nhận chuyển trang", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    vmDangKy.SaveChangesToDatabase();
                    return true;
                }
                else if (result == MessageBoxResult.No)
                {
                    vmDangKy.CancelEdits();
                    return true;
                }
                return false; // Cancel
            }

            if (NoiDungHienTai is VM_DanhSachHocPhi vmHocPhi && vmHocPhi.IsEditing)
            {
                var result = MessageBox.Show("Bạn đang trong chế độ chỉnh sửa Khóa học. Bạn có muốn LƯU các thay đổi trước khi chuyển trang không?\n\n- Chọn 'Yes' để lưu và chuyển trang.\n- Chọn 'No' để hủy thay đổi và chuyển trang.\n- Chọn 'Cancel' để ở lại.", "Xác nhận chuyển trang", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    vmHocPhi.SaveChangesToDatabase();
                    return true;
                }
                else if (result == MessageBoxResult.No)
                {
                    vmHocPhi.CancelEdits();
                    return true;
                }
                return false; // Cancel
            }

            if (NoiDungHienTai is VM_DangKyMoi vmDangKyMoi && vmDangKyMoi.IsEditing)
            {
                var result = MessageBox.Show("Bạn đang trong chế độ chỉnh sửa bảng Đăng ký của form thêm mới. Bạn có muốn LƯU các thay đổi trước khi chuyển trang không?\n\n- Chọn 'Yes' để lưu và chuyển trang.\n- Chọn 'No' để hủy thay đổi và chuyển trang.\n- Chọn 'Cancel' để ở lại.", "Xác nhận chuyển trang", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    vmDangKyMoi.SaveChangesToDatabase();
                    return true;
                }
                else if (result == MessageBoxResult.No)
                {
                    vmDangKyMoi.CancelEdits();
                    return true;
                }
                return false; // Cancel
            }

            return true;
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
