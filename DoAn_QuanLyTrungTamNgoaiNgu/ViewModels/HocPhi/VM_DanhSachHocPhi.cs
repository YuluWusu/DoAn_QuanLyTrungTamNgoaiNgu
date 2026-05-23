using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data.Entity;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{ 
    public class VM_DanhSachHocPhi : BaseViewModel
    {
        private ObservableCollection<KHOA_HOC> _listKhoaHoc;
        public ObservableCollection<KHOA_HOC> ListKhoaHoc
        {
            get => _listKhoaHoc;
            set { _listKhoaHoc = value; OnPropertyChanged(); }
        }

        private ObservableCollection<LOAI_KHOAHOC> _listLoaiKhoaHoc;
        public ObservableCollection<LOAI_KHOAHOC> ListLoaiKhoaHoc
        {
            get => _listLoaiKhoaHoc;
            set { _listLoaiKhoaHoc = value; OnPropertyChanged(); }
        }

        private KHOA_HOC _selectedKhoaHoc;
        public KHOA_HOC SelectedKhoaHoc
        {
            get => _selectedKhoaHoc;
            set { _selectedKhoaHoc = value; OnPropertyChanged(); }
        }

        // Bộ lọc
        private LOAI_KHOAHOC _filterLoaiKH;
        public LOAI_KHOAHOC FilterLoaiKH
        {
            get => _filterLoaiKH;
            set { _filterLoaiKH = value; OnPropertyChanged(); }
        }

        private string _filterSoBuoi;
        public string FilterSoBuoi
        {
            get => _filterSoBuoi;
            set { _filterSoBuoi = value; OnPropertyChanged(); }
        }

        private string _filterHocPhi;
        public string FilterHocPhi
        {
            get => _filterHocPhi;
            set { _filterHocPhi = value; OnPropertyChanged(); }
        }

        public RelayCommand FilterCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        // Action điều hướng (Sẽ được gán tại TrangChuViewModel)
        public Action OnRequestAddKhoaHoc { get; set; }

        public VM_DanhSachHocPhi()
        {
            LoadData();

            FilterCommand = new RelayCommand(p => ExecuteFilter());

            AddCommand = new RelayCommand(p =>
            {
                OnRequestAddKhoaHoc?.Invoke();
            });

            EditCommand = new RelayCommand(p => ExecuteEdit());
            DeleteCommand = new RelayCommand(p => ExecuteDelete());
        }

        private void LoadData()
        {
            ListLoaiKhoaHoc = new ObservableCollection<LOAI_KHOAHOC>(DataProvider.Ins.DB.LOAI_KHOAHOC.ToList());
            var query = DataProvider.Ins.DB.KHOA_HOC.Include("LOAI_KHOAHOC").ToList();
            ListKhoaHoc = new ObservableCollection<KHOA_HOC>(query);
        }

        private void ExecuteFilter()
        {
            var query = DataProvider.Ins.DB.KHOA_HOC.AsQueryable();
            
            if (FilterLoaiKH != null) 
                query = query.Where(x => x.MALOAI_KH == FilterLoaiKH.MALOAI_KH);
                
            if (int.TryParse(FilterSoBuoi, out int sb)) 
                query = query.Where(x => x.SOBUOI <= sb);
                
            if (decimal.TryParse(FilterHocPhi, out decimal hp)) 
                query = query.Where(x => x.HOCPHI_GD <= hp);
                
            ListKhoaHoc = new ObservableCollection<KHOA_HOC>(query.Include("LOAI_KHOAHOC").ToList());
        }

        private void ExecuteEdit()
        {
            if (SelectedKhoaHoc == null)
            {
                MessageBox.Show("Vui lòng chọn khóa học cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Add edit logic or navigation here
        }

        private void ExecuteDelete()
        {
            if (SelectedKhoaHoc == null)
            {
                MessageBox.Show("Vui lòng chọn khóa học cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa khóa học {SelectedKhoaHoc.TENKH}?",
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataProvider.Ins.DB.KHOA_HOC.Remove(SelectedKhoaHoc);
                    DataProvider.Ins.DB.SaveChanges();
                    ListKhoaHoc.Remove(SelectedKhoaHoc);
                    MessageBox.Show("Đã xóa thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa (Có thể khóa học này đã có lớp học): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
