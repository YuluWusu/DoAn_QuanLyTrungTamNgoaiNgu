using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_DangKyMoi : BaseViewModel
    {
        private ObservableCollection<HOCVIEN> _listHocVien;
        public ObservableCollection<HOCVIEN> ListHocVien
        {
            get => _listHocVien;
            set { _listHocVien = value; OnPropertyChanged(); }
        }

        private ObservableCollection<LOPHOC> _listLopHoc;
        public ObservableCollection<LOPHOC> ListLopHoc
        {
            get => _listLopHoc;
            set { _listLopHoc = value; OnPropertyChanged(); }
        }

        private HOCVIEN _selectedHocVien;
        public HOCVIEN SelectedHocVien
        {
            get => _selectedHocVien;
            set { _selectedHocVien = value; OnPropertyChanged(); }
        }

        private LOPHOC _selectedLopHoc;
        public LOPHOC SelectedLopHoc
        {
            get => _selectedLopHoc;
            set { _selectedLopHoc = value; OnPropertyChanged(); }
        }

        private decimal _hocPhi;
        public decimal HocPhi
        {
            get => _hocPhi;
            set { _hocPhi = value; OnPropertyChanged(); }
        }

        private string _trangThai;
        public string TrangThai
        {
            get => _trangThai;
            set { _trangThai = value; OnPropertyChanged(); }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public Action RequestNavigateBack { get; set; }

        public VM_DangKyMoi()
        {
            LoadData();
            TrangThai = "Cho dong tien"; // Default status according to SQL
            
            CancelCommand = new RelayCommand(p => RequestNavigateBack?.Invoke());
            SaveCommand = new RelayCommand(p => ExecuteSave());
        }

        private void LoadData()
        {
            ListHocVien = new ObservableCollection<HOCVIEN>(DataProvider.Ins.DB.HOCVIENs.ToList());
            ListLopHoc = new ObservableCollection<LOPHOC>(DataProvider.Ins.DB.LOPHOCs.ToList());
        }

        private void ExecuteSave()
        {
            if (SelectedHocVien == null || SelectedLopHoc == null)
            {
                MessageBox.Show("Vui lòng chọn Học viên và Lớp học.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (HocPhi < 0)
            {
                MessageBox.Show("Học phí không được âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dangKyLop = new DANGKYLOP
                {
                    MaHV = SelectedHocVien.MaHV,
                    MALOP = SelectedLopHoc.MALOP,
                    NGAYDK = DateTime.Now,
                    HOCPHI = HocPhi,
                    TRANGTHAI = TrangThai
                };

                DataProvider.Ins.DB.DANGKYLOPs.Add(dangKyLop);
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Đã thêm thành công đăng ký mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestNavigateBack?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào cơ sở dữ liệu (Có thể học viên đã đăng ký lớp này rồi): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
