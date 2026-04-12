using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class DangNhapViewModel:BaseViewModel
    {
        private static readonly TaiKhoan[] _taiKhoanMau = new[]
        {
            new TaiKhoan { MATK="TK0001", TENDANGNHAP="admin",   MATKHAU="123", MaNV="NV001", VaiTro="NhanVien", HoTenNguoiDung="Quản lý hệ thống", TRANGTHAI=true },
            new TaiKhoan { MATK="TK0002", TENDANGNHAP="letan",   MATKHAU="123", MaNV="NV002", VaiTro="NhanVien", HoTenNguoiDung="Nguyễn Lễ Tân",    TRANGTHAI=true },
            new TaiKhoan { MATK="TK0003", TENDANGNHAP="giaovien",MATKHAU="123", MaGV="GV001", VaiTro="GiaoVien", HoTenNguoiDung="Trần Giáo Viên",   TRANGTHAI=true },
        };

        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set => SetProperty(ref _tenDangNhap, value);
        }

        private string _thongBaoLoi;
        public string ThongBaoLoi
        {
            get => _thongBaoLoi;
            set => SetProperty(ref _thongBaoLoi, value);
        }

        private bool _dangXuLy;
        public bool DangXuLy
        {
            get => _dangXuLy;
            set => SetProperty(ref _dangXuLy, value);
        }

        
        public RelayCommand DangNhapCommand { get; }

        public DangNhapViewModel()
        {
            DangNhapCommand = new RelayCommand(ExecuteDangNhap);
        }

        private void ExecuteDangNhap(object parameter)
        {
            var matKhau = string.Empty;
            if (parameter is PasswordBox pb)
                matKhau = pb.Password;

            ThongBaoLoi = string.Empty;

            if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                ThongBaoLoi = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return;
            }

            var taiKhoan = _taiKhoanMau.FirstOrDefault(tk =>
                tk.TENDANGNHAP == TenDangNhap.Trim() && tk.MATKHAU == matKhau);

            if (taiKhoan == null)
            {
                ThongBaoLoi = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return;
            }

            var trangChu = new Views.TrangChu();

            if (trangChu.DataContext is TrangChuViewModel vm)
            {
                vm.TaiKhoanHienTai = taiKhoan;
            }

            trangChu.Show();

            foreach (Window item in Application.Current.Windows)
            {
                if (item is Views.DangNhap)
                {
                    item.Close();
                    break;
                }
            }
        }
    }
}
