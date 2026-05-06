using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class ThemHocPhiKhoaHoc : BaseViewModel
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

        //private LoaiKhoaHoc _selectedLoaiKH;
        //public LoaiKhoaHoc SelectedLoaiKH
        //{
        //    get => _selectedLoaiKH;
        //    set { _selectedLoaiKH = value; OnPropertyChanged(); }
        //}

        //private ObservableCollection<LoaiKhoaHoc> _listLoaiKhoaHoc;
        //public ObservableCollection<LoaiKhoaHoc> ListLoaiKhoaHoc
        //{
        //    get => _listLoaiKhoaHoc;
        //    set { _listLoaiKhoaHoc = value; OnPropertyChanged(); }
        //}

        //private ObservableCollection<KhoaHoc> _listKhoaHoc;
        //public ObservableCollection<KhoaHoc> ListKhoaHoc
        //{
        //    get => _listKhoaHoc;
        //    set { _listKhoaHoc = value; OnPropertyChanged(); }
        //}

        public ICommand CancelCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        // Gọi về TrangChuViewModel để quay lại màn hình danh sách
        public Action RequestNavigateBack { get; set; }

        public ThemHocPhiKhoaHoc()
        {
            LoadData();

            CancelCommand = new RelayCommand(p => RequestNavigateBack?.Invoke());
            RefreshCommand = new RelayCommand(p => ExecuteRefresh());
            SaveCommand = new RelayCommand(p => ExecuteSave());
        }

        private void LoadData()
        {
            //ListLoaiKhoaHoc = new ObservableCollection<LoaiKhoaHoc>();
            //ListKhoaHoc = new ObservableCollection<KhoaHoc>();
        }

        private void ExecuteRefresh()
        {
            MaKH = string.Empty;
            TenKH = string.Empty;
            //SelectedLoaiKH = null;
            SoBuoi = 0;
            HocPhi = 0;
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrWhiteSpace(MaKH) || string.IsNullOrWhiteSpace(TenKH))/*|| SelectedLoaiKH == null)*/
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc (*).", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SoBuoi <= 0 || HocPhi < 0)
            {
                MessageBox.Show("Số buổi phải lớn hơn 0 và Học phí không được âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Logic gọi Stored Procedure: SP_ThemKhoaHoc
            try
            {
                // Thực thi DataProvider.Ins.DB.SP_ThemKhoaHoc(MaKH, TenKH, SoBuoi, HocPhi, SelectedLoaiKH.MALOAI_KH)

                MessageBox.Show($"Đã thêm thành công khóa học: {TenKH}!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Có thể làm mới danh sách tham chiếu sau khi lưu
                // LoadData(); 
                RequestNavigateBack?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
