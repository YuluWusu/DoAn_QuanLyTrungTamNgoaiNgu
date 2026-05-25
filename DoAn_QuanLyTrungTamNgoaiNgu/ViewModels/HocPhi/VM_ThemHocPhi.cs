using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_ThemHocPhi : BaseViewModel
    {
        private string _maKH;
        public string MaKH
        {
            get => _maKH;
            set { _maKH = value; OnPropertyChanged(); }
        }

        private string _tenKH;
        public string TenKH
        {
            get => _tenKH;
            set { _tenKH = value; OnPropertyChanged(); }
        }

        private int _soBuoi;
        public int SoBuoi
        {
            get => _soBuoi;
            set { _soBuoi = value; OnPropertyChanged(); }
        }

        private decimal _hocPhi;
        public decimal HocPhi
        {
            get => _hocPhi;
            set { _hocPhi = value; OnPropertyChanged(); }
        }

        private LOAI_KHOAHOC _selectedLoaiKH;
        public LOAI_KHOAHOC SelectedLoaiKH
        {
            get => _selectedLoaiKH;
            set { _selectedLoaiKH = value; OnPropertyChanged(); }
        }

        private ObservableCollection<LOAI_KHOAHOC> _listLoaiKhoaHoc;
        public ObservableCollection<LOAI_KHOAHOC> ListLoaiKhoaHoc
        {
            get => _listLoaiKhoaHoc;
            set { _listLoaiKhoaHoc = value; OnPropertyChanged(); }
        }

        private ObservableCollection<KHOA_HOC> _listKhoaHoc;
        public ObservableCollection<KHOA_HOC> ListKhoaHoc
        {
            get => _listKhoaHoc;
            set { _listKhoaHoc = value; OnPropertyChanged(); }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        // Gọi về TrangChuViewModel để quay lại màn hình danh sách
        public Action RequestNavigateBack { get; set; }

        public VM_ThemHocPhi()
        {
            LoadData();

            CancelCommand = new RelayCommand(p => RequestNavigateBack?.Invoke());
            RefreshCommand = new RelayCommand(p => ExecuteRefresh());
            SaveCommand = new RelayCommand(p => ExecuteSave());
        }

        private void LoadData()
        {
            ListLoaiKhoaHoc = new ObservableCollection<LOAI_KHOAHOC>(DataProvider.Ins.DB.LOAI_KHOAHOC.ToList());
            ListKhoaHoc = new ObservableCollection<KHOA_HOC>(DataProvider.Ins.DB.KHOA_HOC.ToList());
            GenerateMaKH();
        }

        private void GenerateMaKH()
        {
            var maxMaKH = DataProvider.Ins.DB.KHOA_HOC.OrderByDescending(x => x.MAKH).Select(x => x.MAKH).FirstOrDefault();
            if (string.IsNullOrEmpty(maxMaKH))
            {
                MaKH = "KH001";
            }
            else
            {
                if (maxMaKH.StartsWith("KH") && int.TryParse(maxMaKH.Substring(2), out int num))
                {
                    MaKH = $"KH{(num + 1):D3}";
                }
                else
                {
                    MaKH = "KH001";
                }
            }
        }

        private void ExecuteRefresh()
        {
            GenerateMaKH();
            TenKH = string.Empty;
            SelectedLoaiKH = null;
            SoBuoi = 0;
            HocPhi = 0;
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrWhiteSpace(MaKH) || string.IsNullOrWhiteSpace(TenKH) || SelectedLoaiKH == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc (*).", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SoBuoi <= 0 || HocPhi < 0)
            {
                MessageBox.Show("Số buổi phải lớn hơn 0 và Học phí không được âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newKhoaHoc = new KHOA_HOC
                {
                    MAKH = MaKH,
                    TENKH = TenKH,
                    SOBUOI = SoBuoi,
                    HOCPHI_GD = HocPhi,
                    MALOAI_KH = SelectedLoaiKH.MALOAI_KH,
                    LOAI_KHOAHOC = SelectedLoaiKH
                };

                DataProvider.Ins.DB.KHOA_HOC.Add(newKhoaHoc);
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show($"Đã thêm thành công khóa học: {TenKH}!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                RequestNavigateBack?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
