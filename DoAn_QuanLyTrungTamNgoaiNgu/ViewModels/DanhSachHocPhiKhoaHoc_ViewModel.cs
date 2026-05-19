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
    public class DanhSachHocPhiKhoaHoc:BaseViewModel
    {
        //private ObservableCollection<> _listKhoaHoc;
        //public ObservableCollection<KhoaHoc> ListKhoaHoc
        //{
        //    get => _listKhoaHoc;
        //    set { _listKhoaHoc = value; OnPropertyChanged(); }
        //}

        //private ObservableCollection<LoaiKhoaHoc> _listLoaiKhoaHoc;
        //public ObservableCollection<LoaiKhoaHoc> ListLoaiKhoaHoc
        //{
        //    get => _listLoaiKhoaHoc;
        //    set { _listLoaiKhoaHoc = value; OnPropertyChanged(); }
        //}

        //private KhoaHoc _selectedKhoaHoc;
        //public KhoaHoc SelectedKhoaHoc
        //{
        //    get => _selectedKhoaHoc;
        //    set { _selectedKhoaHoc = value; OnPropertyChanged(); }
        //}

        //// Bộ lọc
        //private LoaiKhoaHoc _filterLoaiKH;
        //public LoaiKhoaHoc FilterLoaiKH
        //{
        //    get => _filterLoaiKH;
        //    set { _filterLoaiKH = value; OnPropertyChanged(); }
        //}

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

        public ICommand FilterCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        // Action điều hướng (Sẽ được gán tại TrangChuViewModel)
        public Action OnRequestAddKhoaHoc { get; set; }

        public DanhSachHocPhiKhoaHoc()
        {
            LoadData();

            FilterCommand = new RelayCommand(p => ExecuteFilter());

            AddCommand = new RelayCommand(p =>
            {
                OnRequestAddKhoaHoc?.Invoke();
            });

            //EditCommand = new RelayCommand(p => ExecuteEdit());
            //DeleteCommand = new RelayCommand(p => ExecuteDelete());
        }

        private void LoadData()
        {

        }

        private void ExecuteFilter()
        {
            // var query = DataProvider.Ins.DB.KHOA_HOC.AsQueryable();
            // if (FilterLoaiKH != null) query = query.Where(x => x.MALOAI_KH == FilterLoaiKH.MALOAI_KH);
            // if (int.TryParse(FilterSoBuoi, out int sb)) query = query.Where(x => x.SOBUOI <= sb);
            // if (decimal.TryParse(FilterHocPhi, out decimal hp)) query = query.Where(x => x.HOCPHI_GD <= hp);
            // ListKhoaHoc = new ObservableCollection<KhoaHoc>(query);
        }

        //private void ExecuteEdit()
        //{
        //    if (SelectedKhoaHoc == null)
        //    {
        //        MessageBox.Show("Vui lòng chọn khóa học cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return;
        //    }
        //}

        //private void ExecuteDelete()
        //{
        //    if (SelectedKhoaHoc == null)
        //    {
        //        MessageBox.Show("Vui lòng chọn khóa học cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return;
        //    }

        //    var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa khóa học {SelectedKhoaHoc.TENKH}?",
        //                                 "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        // ListKhoaHoc.Remove(SelectedKhoaHoc);
        //        MessageBox.Show("Đã xóa thành công!");
        //    }
        //}
    }
}
