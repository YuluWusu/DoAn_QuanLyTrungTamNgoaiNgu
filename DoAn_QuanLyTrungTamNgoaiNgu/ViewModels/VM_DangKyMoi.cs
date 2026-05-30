using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data.Entity;
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

        private ObservableCollection<DANGKYLOP> _listDangKy;
        public ObservableCollection<DANGKYLOP> ListDangKy
        {
            get => _listDangKy;
            set { _listDangKy = value; OnPropertyChanged(); }
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

        private DANGKYLOP _selectedItem;
        public DANGKYLOP SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
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

        private DateTime _ngayDK = DateTime.Today;
        public DateTime NgayDK
        {
            get => _ngayDK;
            set { _ngayDK = value; OnPropertyChanged(); }
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
            try
            {
                ListHocVien = new ObservableCollection<HOCVIEN>(DataProvider.Ins.DB.HOCVIENs.ToList());
                ListLopHoc = new ObservableCollection<LOPHOC>(DataProvider.Ins.DB.LOPHOCs.ToList());
                
                var registrations = DataProvider.Ins.DB.DANGKYLOPs
                                                .Include(x => x.HOCVIEN)
                                                .Include(x => x.LOPHOC)
                                                .ToList();
                ListDangKy = new ObservableCollection<DANGKYLOP>(registrations);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu đăng ký mới: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            SelectedHocVien = null;
            SelectedLopHoc = null;
            HocPhi = 0;
            TrangThai = "Cho dong tien";
            NgayDK = DateTime.Today;
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

            if (NgayDK.Date > DateTime.Today)
            {
                MessageBox.Show("Ngày đăng ký không được lớn hơn ngày hiện tại.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
 
            try
            {
                var dangKyLop = new DANGKYLOP
                {
                    MaHV = SelectedHocVien.MaHV,
                    MALOP = SelectedLopHoc.MALOP,
                    NGAYDK = NgayDK,
                    HOCPHI = HocPhi,
                    TRANGTHAI = TrangThai,
                    HOCVIEN = SelectedHocVien,
                    LOPHOC = SelectedLopHoc
                };
 
                DataProvider.Ins.DB.DANGKYLOPs.Add(dangKyLop);
                DataProvider.Ins.DB.SaveChanges();
 
                MessageBox.Show("Đã thêm thành công đăng ký mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Làm mới danh sách sau khi thêm thành công
                LoadData();
                SelectedItem = ListDangKy.FirstOrDefault(x => x.MaHV == dangKyLop.MaHV && x.MALOP == dangKyLop.MALOP);
                ClearForm();

                // Điều hướng quay lại trang danh sách đăng ký
                RequestNavigateBack?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào cơ sở dữ liệu (Có thể học viên đã đăng ký lớp này rồi): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
