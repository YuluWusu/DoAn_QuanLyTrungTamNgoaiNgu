using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class TrangChuViewModel : BaseViewModel
    {
        private TaiKhoan _taiKhoanHienTai;
        public TaiKhoan TaiKhoanHienTai
        {
            get => _taiKhoanHienTai;
            set
            {
                SetProperty(ref _taiKhoanHienTai, value);
                OnPropertyChanged(nameof(HoTenHienThi));
                OnPropertyChanged(nameof(VaiTroHienThi));
            }
        }

        public string HoTenHienThi => TaiKhoanHienTai?.HoTenNguoiDung ?? "Người dùng";
        public string VaiTroHienThi => TaiKhoanHienTai?.VaiTro == "GiaoVien" ? "Giáo viên" : "Nhân viên";

        private UserControl _noiDungHienTai;
        public UserControl NoiDungHienTai
        {
            get => _noiDungHienTai;
            set => SetProperty(ref _noiDungHienTai, value);
        }

        public RelayCommand NavTrangChu { get; set; }
        public RelayCommand NavHocVien { get; set; }
        public RelayCommand NavLopHoc { get; set; }
        public RelayCommand NavDangKyLop { get; set; }
        public RelayCommand NavDiemDanh { get; set; }
        public RelayCommand NavHocPhi { get; set; }
        public RelayCommand NavBaoCao { get; set; }
        public RelayCommand NavTaiKhoan { get; set; }
        public RelayCommand DangXuatCommand { get; set; }

        public TrangChuViewModel()
        {
            
            NavTrangChu = new RelayCommand(o => { NoiDungHienTai = new UC_TrangChu(); });
            NavHocVien = new RelayCommand(o => { /* NoiDungHienTai = new UC_HocVien(); */ });
            NavLopHoc = new RelayCommand(o => { /* NoiDungHienTai = new UC_LopHoc(); */ });
            NavDangKyLop = new RelayCommand(o => { /* NoiDungHienTai = new UC_DangKy(); */ });
            NavDiemDanh = new RelayCommand(o => { /* NoiDungHienTai = new UC_DiemDanh(); */ });
            NavHocPhi = new RelayCommand(o => { /* NoiDungHienTai = new UC_HocPhi(); */ });
            NavBaoCao = new RelayCommand(o => { /* NoiDungHienTai = new UC_BaoCao(); */ });
            NavTaiKhoan = new RelayCommand(o => { /* NoiDungHienTai = new UC_TaiKhoan(); */ });

            DangXuatCommand = new RelayCommand(ExecuteDangXuat);

            
            NoiDungHienTai = new UC_TrangChu();
        }

        private void ExecuteDangXuat(object parameter)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var dn = new Views.DangNhap();
                dn.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != dn) window.Close();
                }
            }
        }
    }
}