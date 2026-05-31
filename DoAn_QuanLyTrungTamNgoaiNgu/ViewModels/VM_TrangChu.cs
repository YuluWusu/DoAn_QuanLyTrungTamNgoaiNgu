using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;

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
        private string _activeTab = "TrangChu";
        public string ActiveTab
        {
            get => _activeTab;
            set
            {
                _activeTab = value;
                OnPropertyChanged();
            }
        }

        private Visibility _menuHocVien = Visibility.Visible;
        public Visibility MenuHocVien { get => _menuHocVien; set { _menuHocVien = value; OnPropertyChanged(); } }

        private Visibility _menuLopHoc = Visibility.Visible;
        public Visibility MenuLopHoc { get => _menuLopHoc; set { _menuLopHoc = value; OnPropertyChanged(); } }

        private Visibility _menuDangKy = Visibility.Visible;
        public Visibility MenuDangKy { get => _menuDangKy; set { _menuDangKy = value; OnPropertyChanged(); } }

        private Visibility _menuDiemDanh = Visibility.Visible;
        public Visibility MenuDiemDanh { get => _menuDiemDanh; set { _menuDiemDanh = value; OnPropertyChanged(); } }

        private Visibility _menuHocPhi = Visibility.Visible;
        public Visibility MenuHocPhi { get => _menuHocPhi; set { _menuHocPhi = value; OnPropertyChanged(); } }

        private Visibility _menuBaoCao = Visibility.Visible;
        public Visibility MenuBaoCao { get => _menuBaoCao; set { _menuBaoCao = value; OnPropertyChanged(); } }

        private Visibility _menuTaiKhoan = Visibility.Visible;
        public Visibility MenuTaiKhoan { get => _menuTaiKhoan; set { _menuTaiKhoan = value; OnPropertyChanged(); } }

        private Visibility _menuLoaiKH = Visibility.Visible;
        public Visibility MenuLoaiKH { get => _menuLoaiKH; set { _menuLoaiKH = value; OnPropertyChanged(); } }

        private Visibility _menuPhongHoc = Visibility.Visible;
        public Visibility MenuPhongHoc { get => _menuPhongHoc; set { _menuPhongHoc = value; OnPropertyChanged(); } }

        private int _soHocVien;
        public int SoHocVien { get => _soHocVien; set { _soHocVien = value; OnPropertyChanged(); } }

        private int _soLop;
        public int SoLop { get => _soLop; set { _soLop = value; OnPropertyChanged(); } }

        private decimal _doanhThu;
        public decimal DoanhThu { get => _doanhThu; set { _doanhThu = value; OnPropertyChanged(); } }

        private int _soGiaoVien;
        public int SoGiaoVien { get => _soGiaoVien; set { _soGiaoVien = value; OnPropertyChanged(); } }

        private ObservableCollection<TodayScheduleItem> _todaySchedule;
        public ObservableCollection<TodayScheduleItem> TodaySchedule { get => _todaySchedule; set { _todaySchedule = value; OnPropertyChanged(); } }
        public ICommand NavTrangChu {  get; set; }
        
        public ICommand NavHocVien { get; set; }
        public ICommand NavThemHocVien { get; set; }
        public ICommand NavSuaHocVien { get; set; }

        public ICommand NavLopHoc { get; set; }
        public ICommand NavDangKyLop { get; set; }
        public ICommand NavDiemDanh { get; set; }
        public ICommand NavHocPhi { get; set; }
        public ICommand NavBaoCao { get; set; }
        public ICommand NavTaiKhoan { get; set; }
        public ICommand DangXuatCommand { get; set; }
        
        public ICommand NavThemLoaiKH { get; set; }
        public ICommand NavThemPhongHoc { get; set; }

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
            ActiveTab = "TrangChu";
            NavTrangChu = new RelayCommand<object>((p) => true, (p) => { NavigateTo(new UC_TrangChu(), "TrangChu"); });
            NavLopHoc = new RelayCommand<object>((p) => true, (p) => { NavigateTo(new UC_LopHoc(), "LopHoc"); });
            NavDiemDanh = new RelayCommand<object>((p) => true, (p) => { NavigateTo(new UC_DiemDanh(), "DiemDanh"); });
            NavTaiKhoan = new RelayCommand<object>((p) => 
            {
                string vaiTro = UserSession.VaiTro ?? VaiTroHienThi ?? "";
                return vaiTro == "Quản lý" || vaiTro == "Admin" || vaiTro == "Quan ly";
            }, (p) => { NavigateTo(new UC_TaiKhoan(), "TaiKhoan"); });
            
            
            NavHocVien = new RelayCommand<object>((p) => true, (p) => { NavigateTo(new UC_HocVien(), "HocVien"); });

           
            NavThemHocVien = new RelayCommand<object>((p) => true,(p) => { VM_HocVien.NewHVStatic = new HOCVIEN();VM_HocVien.SelectedLopStatic = null; NoiDungHienTai = new UC_ThemHocVien();});
            NavSuaHocVien = new RelayCommand<object>( (p) => p != null,(p) => { DANGKYLOP dk = p as DANGKYLOP;
                    if (dk != null)
                    {
                     
                        VM_HocVien.SelectedDKStatic = dk;

                        VM_HocVien.NewHVStatic = new HOCVIEN()
                        {
                            MaHV = dk.HOCVIEN.MaHV,
                            HoTen = dk.HOCVIEN.HoTen,
                            NgaySinh = dk.HOCVIEN.NgaySinh,
                            GioiTinh = dk.HOCVIEN.GioiTinh,
                            DiaChi = dk.HOCVIEN.DiaChi,
                            SDT = dk.HOCVIEN.SDT,
                            Email = dk.HOCVIEN.Email,
                            TrangThai = dk.HOCVIEN.TrangThai
                        };

                        VM_HocVien.SelectedLopStatic = dk.LOPHOC;

                        NoiDungHienTai = new UC_SuaThongTinHV();
                    }
                });


            NavBaoCao = new RelayCommand<object>((p) => true, (p) => { NavigateTo(new UC_BaoCaoTaiChinh(), "BaoCao"); });

            NavDangKyLop = new RelayCommand<object>((p) => true, (p) => 
            { 
                var vmDangKy = new VM_DangKy();
        

                vmDangKy.OnRequestAddRegistration = () => 
                {
                    var vmDangKyMoi = new VM_DangKyMoi();
                    
                    vmDangKyMoi.RequestNavigateBack = () => NavigateTo(vmDangKy);
                    NavigateTo(vmDangKyMoi);
                };
        
                NavigateTo(vmDangKy, "DangKy"); 
            });
            NavHocPhi = new RelayCommand<object>((p) => true, (p) => 
            { 
                var vmDanhSachHocPhi = new VM_DanhSachHocPhi();
        
                
                vmDanhSachHocPhi.OnRequestAddKhoaHoc = () =>
                {
                    var vmThemHocPhi = new VM_ThemHocPhi();
                    
                    vmThemHocPhi.RequestNavigateBack = () => 
                    {
                        vmDanhSachHocPhi.LoadData();
                        NavigateTo(vmDanhSachHocPhi);
                    };
                    NavigateTo(vmThemHocPhi); 
                };
        
                NavigateTo(vmDanhSachHocPhi, "HocPhi"); 
            });

            NavThemLoaiKH = new RelayCommand<object>((p) => true, (p) =>
            {
                NavigateTo(new UC_LoaiKhoaHoc(), "ThemLoaiKH");
            });
            NavThemPhongHoc = new RelayCommand<object>((p) => true, (p) =>
            {
                NavigateTo(new UC_PhongHoc(), "ThemPhongHoc");
            });
        }

        private void NavigateTo(object newContent, string tabName = null)
        {
            if (CanNavigateAway())
            {
                NoiDungHienTai = newContent;
                if (!string.IsNullOrEmpty(tabName))
                {
                    ActiveTab = tabName;
                }
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

                // Cài đặt phân quyền RBAC
                string vaiTro = VaiTroHienThi ?? "";
                if (vaiTro.Contains("Lễ tân") || vaiTro.Contains("Le tan"))
                {
                    MenuDiemDanh = Visibility.Collapsed;
                    MenuBaoCao = Visibility.Collapsed;
                    MenuTaiKhoan = Visibility.Collapsed;
                    // Lễ tân không quản lý danh mục
                    MenuLoaiKH = Visibility.Collapsed;
                    MenuPhongHoc = Visibility.Collapsed;
                }
                else if (vaiTro.Contains("Kế toán") || vaiTro.Contains("Ke toan"))
                {
                    MenuBaoCao = Visibility.Collapsed;
                    MenuHocVien = Visibility.Collapsed;
                    MenuLopHoc = Visibility.Collapsed;
                    MenuDangKy = Visibility.Collapsed;
                    MenuDiemDanh = Visibility.Collapsed;
                    MenuTaiKhoan = Visibility.Collapsed;
                    MenuLoaiKH = Visibility.Collapsed;
                    MenuPhongHoc = Visibility.Collapsed;
                }
                else if (vaiTro.Contains("Giáo viên") || vaiTro.Contains("Giao vien"))
                {
                    MenuHocVien = Visibility.Collapsed;
                    MenuDangKy = Visibility.Collapsed;
                    MenuHocPhi = Visibility.Collapsed;
                    MenuBaoCao = Visibility.Collapsed;
                    MenuTaiKhoan = Visibility.Collapsed;
                    MenuLoaiKH = Visibility.Collapsed;
                    MenuPhongHoc = Visibility.Collapsed;
                }
                // Admin / Manager: tất cả menu đều Visible (mặc định)
            }
            
            LoadDashboardData();
        }

        public void LoadDashboardData()
        {
            using (var db = new QL_TRUNGTAM_TIENGANH())
            {
                int tongHocVien = 0;
                if (UserSession.ChucVu == "Giáo viên")
                {
                    tongHocVien = db.DANGKYLOPs
                                    .Where(x => x.LOPHOC.MaGV == UserSession.MaGV && (x.TRANGTHAI == "Dang hoc" || x.TRANGTHAI == "Đang học"))
                                    .Select(x => x.MaHV).Distinct().Count();
                }
                else
                {
                    tongHocVien = db.DANGKYLOPs
                                    .Where(x => x.TRANGTHAI == "Dang hoc" || x.TRANGTHAI == "Đang học")
                                    .Select(x => x.MaHV).Distinct().Count();
                }
                SoHocVien = tongHocVien;

                SoLop = db.LOPHOCs.Count(x => x.TRANGTHAI == "Dang mo" || x.TRANGTHAI == "Đang mở");
                
                int currentMonth = System.DateTime.Now.Month;
                int currentYear = System.DateTime.Now.Year;
                var dt = db.PHIEUTHUs.Where(x => x.NGAYTHU.Month == currentMonth && x.NGAYTHU.Year == currentYear).Sum(x => (decimal?)x.SOTIEN);
                DoanhThu = dt ?? 0;

                SoGiaoVien = db.GIAOVIENs.Count();

                var today = System.DateTime.Today;
                var listLich = db.LICHOCs
                                 .Include(x => x.LOPHOC)
                                 .Include(x => x.LOPHOC.KHOA_HOC)
                                 .Include(x => x.CAHOC)
                                 .Include(x => x.PHONGHOC)
                                 .ToList();

                var scheduleQuery = listLich
                    .Where(x => x.NGAYHOC.Date == today)
                    .Select(x => new TodayScheduleItem
                    {
                        TenLop = x.LOPHOC.TENLOP,
                        TenKhoaHoc = x.LOPHOC.KHOA_HOC?.TENKH ?? "",
                        CaHoc = x.CAHOC != null ? $"{x.CAHOC.TENCA} ({x.CAHOC.GIOBATDAU:hh\\:mm} - {x.CAHOC.GIOKETTHUC:hh\\:mm})" : "Chưa xác định",
                        PhongHoc = x.PHONGHOC?.TENPHONG ?? "Chưa xếp",
                        TenGiaoVien = x.LOPHOC.GIAOVIEN?.TenGV ?? "Chưa xếp"
                    });

                if (UserSession.ChucVu == "Giáo viên")
                {
                    scheduleQuery = scheduleQuery.Where(x => x.TenGiaoVien == UserSession.HoTen);
                }

                TodaySchedule = new ObservableCollection<TodayScheduleItem>(scheduleQuery);
            }
        }
    }

    public class TodayScheduleItem
    {
        public string TenLop { get; set; }
        public string TenKhoaHoc { get; set; }
        public string CaHoc { get; set; }
        public string PhongHoc { get; set; }
        public string TenGiaoVien { get; set; }
    }
}
