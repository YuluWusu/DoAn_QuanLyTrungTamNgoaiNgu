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

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set 
            { 
                _isEditing = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(IsReadOnly));
                OnPropertyChanged(nameof(IsOtherButtonsEnabled));
                OnPropertyChanged(nameof(EditButtonText));
            }
        }

        public bool IsReadOnly => !IsEditing;
        public bool IsOtherButtonsEnabled => !IsEditing;

        public string EditButtonText => IsEditing ? "Xác nhận sửa" : "Sửa thông tin";

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
        public RelayCommand ClearFilterCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }

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
            ClearFilterCommand = new RelayCommand(p => ExecuteClearFilter());
            RefreshCommand = new RelayCommand(p => ExecuteRefresh());
        }

        public void LoadData()
        {
            ListLoaiKhoaHoc = new ObservableCollection<LOAI_KHOAHOC>(DataProvider.Ins.DB.LOAI_KHOAHOC.ToList());
            var query = DataProvider.Ins.DB.KHOA_HOC.Include("LOAI_KHOAHOC").ToList();
            ListKhoaHoc = new ObservableCollection<KHOA_HOC>(query);
        }

        private void ExecuteFilter()
        {
            if (IsEditing) return;
            var query = DataProvider.Ins.DB.KHOA_HOC.AsQueryable();
            
            if (FilterLoaiKH != null) 
                query = query.Where(x => x.MALOAI_KH == FilterLoaiKH.MALOAI_KH);
                
            if (int.TryParse(FilterSoBuoi, out int sb)) 
                query = query.Where(x => x.SOBUOI <= sb);
                
            if (decimal.TryParse(FilterHocPhi, out decimal hp)) 
                query = query.Where(x => x.HOCPHI_GD <= hp);
                
            ListKhoaHoc = new ObservableCollection<KHOA_HOC>(query.Include("LOAI_KHOAHOC").ToList());
        }

        private void ExecuteClearFilter()
        {
            if (IsEditing) return;
            FilterLoaiKH = null;
            FilterSoBuoi = null;
            FilterHocPhi = null;
            ExecuteFilter();
        }

        private void ExecuteRefresh()
        {
            if (IsEditing)
            {
                var result = MessageBox.Show("Bạn đang trong chế độ chỉnh sửa. Nếu làm mới, các thay đổi chưa lưu sẽ bị hủy. Bạn có muốn tiếp tục?", "Xác nhận làm mới", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;
            }
            CancelEdits();
        }

        private void ExecuteEdit()
        {
            if (SelectedKhoaHoc == null && !IsEditing)
            {
                MessageBox.Show("Vui lòng chọn khóa học cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsEditing)
            {
                IsEditing = true;
            }
            else
            {
                if (SaveChangesToDatabase())
                {
                    IsEditing = false;
                    MessageBox.Show("Đã lưu các chỉnh sửa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public bool SaveChangesToDatabase()
        {
            try
            {
                DataProvider.Ins.DB.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void CancelEdits()
        {
            IsEditing = false;
            // Hủy các thay đổi trong DbContext cho các thực thể đang theo dõi
            var changedEntries = DataProvider.Ins.DB.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified);
            foreach (var entry in changedEntries)
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
            }
            LoadData();
        }

        private void ExecuteDelete()
        {
            if (IsEditing) return;
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
                    // Lấy chính xác đối tượng đang được track từ DbContext để tránh lỗi detached entity
                    var dbItem = DataProvider.Ins.DB.KHOA_HOC.Find(SelectedKhoaHoc.MAKH);
                    if (dbItem != null)
                    {
                        DataProvider.Ins.DB.KHOA_HOC.Remove(dbItem);
                        DataProvider.Ins.DB.SaveChanges();
                        ListKhoaHoc.Remove(SelectedKhoaHoc);
                        MessageBox.Show("Đã xóa thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khóa học này trong CSDL. Vui lòng làm mới danh sách.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa (Có thể khóa học này đã có lớp học): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadData(); // Đồng bộ lại UI nếu có lỗi xảy ra
                }
            }
        }
    }
}
