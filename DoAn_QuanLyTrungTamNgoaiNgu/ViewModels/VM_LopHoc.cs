using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_LopHoc : BaseViewModel
    {
        public event Action<LOPHOC> OnEditRequested;
        public event Action<LOPHOC> OnCancelRequested;
        private ObservableCollection<LOPHOC> _listLopHoc;
        public ObservableCollection<LOPHOC> ListLopHoc
        {
            get => _listLopHoc;
            set { _listLopHoc = value; OnPropertyChanged(); ApplyFilter(); OnPropertyChanged(nameof(ListLopHocDangMo)); }
        }

        private ObservableCollection<LOPHOC> _filteredList;
        public ObservableCollection<LOPHOC> FilteredList
        {
            get => _filteredList;
            set { _filteredList = value; OnPropertyChanged(); }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilter(); OnPropertyChanged(nameof(ListLopHocDangMo)); }
        }
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        private string _filterTrangThai = "Dang mo";
        public string FilterTrangThai
        {
            get => _filterTrangThai;
            set { _filterTrangThai = value; OnPropertyChanged(); ApplyFilter(); OnPropertyChanged(nameof(ListLopHocDangMo)); }
        }

        private LOPHOC _selectedItem;
        public LOPHOC SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }
        public int TotalClasses => ListLopHoc?.Count ?? 0;
        private int _totalStudents;
        public int TotalStudents
        {
            get => _totalStudents;
            set
            {
                _totalStudents = value; OnPropertyChanged();
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand HienTatCa { get; set; }
        public ICommand LocTheoTrangThai { get; set; }

        public ICommand XoaLocNgay { get; set; }

        public VM_LopHoc()
        {
            LoadData();
            OnPropertyChanged(nameof(ListLopHocDangMo));
            InitCommands();
        }

        public async void LoadData()
        {
            var query = DataProvider.Ins.DB.LOPHOCs
                       .Include("GIAOVIEN")
                       .Include("DANGKYLOPs.HOCVIEN")
                       .Include("LICHOCs")
                       .AsNoTracking()
                       .AsQueryable();

            if (UserSession.VaiTro != null && (UserSession.VaiTro.Contains("Giáo viên") || UserSession.VaiTro.Contains("Giao vien")))
            {
                query = query.Where(x => x.MaGV == UserSession.MaGV);
            }

            var data = query.ToList();

            ListLopHoc = new ObservableCollection<LOPHOC>(data);
            TotalStudents = DataProvider.Ins.DB.HOCVIENs.Count();
        }
        public IEnumerable<LOPHOC> ListLopHocDangMo =>
            ListLopHoc?.Where(x => x.TRANGTHAI == "Dang mo");
        public void ApplyFilter()
        {
            if (ListLopHoc == null) return;

            var query = ListLopHoc.AsEnumerable();

            if (!string.IsNullOrEmpty(FilterTrangThai))
                query = query.Where(x => x.TRANGTHAI == FilterTrangThai);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLower();
                query = query.Where(x =>
                    (x.MALOP?.ToLower().Contains(keyword) == true) ||
                    (x.TENLOP?.ToLower().Contains(keyword) == true) ||
                    (x.GIAOVIEN?.TenGV?.ToLower().Contains(keyword) == true));
            }

            if (SelectedDate.HasValue)
            {
                var ngay = SelectedDate.Value.Date;
                query = query.Where(x =>
                    x.LICHOCs != null && x.LICHOCs.Any(l => l.NGAYHOC == ngay));
            }

            FilteredList = new ObservableCollection<LOPHOC>(query);
        }

        void InitCommands()
        {
            XoaLocNgay = new RelayCommand<object>(
            (p) => true,
            (p) => SelectedDate = null);
            AddCommand = new RelayCommand<object>(
            (p) => true,
            (p) =>
            {
                OnEditRequested?.Invoke(null);
            });

            EditCommand = new RelayCommand<object>(
                (p) => p != null,
                (p) =>
                {
                    var item = p as LOPHOC;
                    if (item == null) return;
                    OnEditRequested?.Invoke(item);
                });

            CancelCommand = new RelayCommand<object>(
            (p) =>
            {
                if (p is LOPHOC lop)
                    return lop.TRANGTHAI == "Dang mo";
                return false;
            },
            (p) =>
            {
                var item = p as LOPHOC;
                if (item == null) return;
                OnCancelRequested?.Invoke(item);
            });
            HienTatCa = new RelayCommand<object>(
                (p) => true,
                (p) => FilterTrangThai = null);

            LocTheoTrangThai = new RelayCommand<string>(
                (p) => true,
                (p) => FilterTrangThai = p?.ToString());
        }
    }
}
