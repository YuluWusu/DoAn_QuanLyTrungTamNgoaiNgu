using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    internal class VM_DangNhap : BaseViewModel
    {
        private string _tenDangNhap;
        public string TenDangNhap { get => _tenDangNhap; set { _tenDangNhap = value; OnPropertyChanged(); } }

        private string _thongBaoLoi;
        public string ThongBaoLoi { get => _thongBaoLoi; set { _thongBaoLoi = value; OnPropertyChanged(); } }

        public ICommand DangNhapCommand { get; set; }

        public VM_DangNhap()
        {
            DangNhapCommand = new RelayCommand<PasswordBox>((p) => true, (p) => {
                ThucHienDangNhap(p);
            });
        }

        private void ThucHienDangNhap(PasswordBox pb)
        {
            if (string.IsNullOrEmpty(TenDangNhap) || pb == null || string.IsNullOrEmpty(pb.Password))
            {
                ThongBaoLoi = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            using (var db = new QL_TRUNGTAM_TIENGANH())
            {
                var acc = db.TAIKHOANs.FirstOrDefault(x => x.TENDANGNHAP == TenDangNhap && x.MATKHAU == pb.Password);

                if (acc != null)
                {
                    if (acc.TRANGTHAI == false)
                    {
                        ThongBaoLoi = "Tài khoản đang bị khóa!";
                        return;
                    } 
                    UserSession.MaTK = acc.MATK;
                    if (acc.MaNV != null)
                    {
                        UserSession.MaNV = acc.MaNV;
                        UserSession.HoTen = acc.NHANVIEN.HoTen;
                        UserSession.VaiTro = acc.NHANVIEN.ChucVu;
                    }
                    else if (acc.MaGV != null)
                    {
                        UserSession.MaGV = acc.MaGV;
                        UserSession.HoTen = acc.GIAOVIEN.TenGV;
                        UserSession.VaiTro = "Giáo viên";
                    }

                    TrangChu main = new TrangChu();
                    
                    var vm = main.DataContext as VM_TrangChu;
                    if (vm != null) vm.LoadThongTinNguoiDung(acc.MATK);

                    main.Show();

                    foreach (Window item in Application.Current.Windows)
                    {
                        if (item.DataContext == this) item.Close();
                    }
                }
                else
                {
                    ThongBaoLoi = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }
        }
    }
}
