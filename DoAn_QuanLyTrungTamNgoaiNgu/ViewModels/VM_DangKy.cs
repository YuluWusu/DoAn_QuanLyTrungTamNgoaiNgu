using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System; 
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Data.Entity;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_DangKy : BaseViewModel
    {
        private ObservableCollection<DANGKYLOP> _listDangKy;
        public ObservableCollection<DANGKYLOP> ListDangKy 
        { 
            get => _listDangKy; 
            set { _listDangKy = value; OnPropertyChanged(); } 
        }

        private string _filterSearchQuery;
        public string FilterSearchQuery
        {
            get => _filterSearchQuery;
            set { _filterSearchQuery = value; OnPropertyChanged(); ExecuteFilter(); }
        }

        private string _filterTrangThai = "-- Tất cả --";
        public string FilterTrangThai
        {
            get => _filterTrangThai;
            set { _filterTrangThai = value; OnPropertyChanged(); ExecuteFilter(); }
        }

        public Action OnRequestAddRegistration { get; set; }

        private DANGKYLOP _selectedItem;
        public DANGKYLOP SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
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

        // Commands để thực hiện các thao tác
        public RelayCommand FilterCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public VM_DangKy()
        {
            try
            {
                LoadData();
                FilterCommand = new RelayCommand(_ => ExecuteFilter());
                AddCommand = new RelayCommand((p) => { OnRequestAddRegistration?.Invoke(); });
                EditCommand = new RelayCommand(p => ExecuteEdit(p));
                DeleteCommand = new RelayCommand(p => ExecuteDelete(p));
                RefreshCommand = new RelayCommand(_ => ExecuteRefresh());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi khởi tạo VM_DangKy");
            }
        }

        public void LoadData()
        {
            // Load từ DB với Include để hiển thị thông tin liên quan nếu cần
            try
            {
                // Phải Include rõ ràng 2 bảng liên kết vì LazyLoading đã bị tắt
                var list = DataProvider.Ins.DB.DANGKYLOPs
                                .Include(x => x.HOCVIEN)
                                .Include(x => x.LOPHOC)
                                 .ToList();

                ListDangKy = new ObservableCollection<DANGKYLOP>(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu Đăng Ký: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteFilter()
        {
            if (IsEditing) return; // Do not filter while editing
            try
            {
                var query = DataProvider.Ins.DB.DANGKYLOPs
                                .Include(x => x.HOCVIEN)
                                .Include(x => x.LOPHOC)
                                .AsQueryable();

                if (!string.IsNullOrWhiteSpace(FilterSearchQuery))
                {
                    string search = FilterSearchQuery.ToLower();
                    query = query.Where(x => x.MaHV.ToLower().Contains(search)
                                          || x.MALOP.ToLower().Contains(search)
                                          || x.HOCVIEN.HoTen.ToLower().Contains(search)
                                          || x.LOPHOC.TENLOP.ToLower().Contains(search));
                }

                if (!string.IsNullOrEmpty(FilterTrangThai) && FilterTrangThai != "-- Tất cả --")
                {
                    query = query.Where(x => x.TRANGTHAI == FilterTrangThai);
                }

                ListDangKy = new ObservableCollection<DANGKYLOP>(query.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lọc: " + ex.Message);
            }
        }

        private void ExecuteEdit(object parameter = null)
        {
            if (parameter is DANGKYLOP dk)
            {
                SelectedItem = dk;
            }

            if (SelectedItem == null && !IsEditing)
            {
                MessageBox.Show("Vui lòng chọn đăng ký cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsEditing)
            {
                IsEditing = true;
            }
            else
            {
                SaveChangesToDatabase();
                IsEditing = false;
                MessageBox.Show("Đã lưu các chỉnh sửa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void SaveChangesToDatabase()
        {
            try
            {
                DataProvider.Ins.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ExecuteRefresh()
        {
            if (IsEditing)
            {
                var result = MessageBox.Show("Bạn đang trong chế độ chỉnh sửa. Nếu làm mới, các thay đổi chưa lưu sẽ bị hủy. Bạn có muốn tiếp tục?", "Xác nhận làm mới", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;
            }
            FilterSearchQuery = string.Empty;
            FilterTrangThai = "-- Tất cả --";
            CancelEdits();
        }

        private void ExecuteDelete(object parameter = null)
        {
            if (parameter is DANGKYLOP dk)
            {
                SelectedItem = dk;
            }

            if (IsEditing) return; // Do not delete while editing
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn Hủy/Xóa đăng ký của HV {SelectedItem.MaHV} cho Lớp {SelectedItem.MALOP}?",
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Check for related DIEMDANH records
                    var relatedDiemDanhs = DataProvider.Ins.DB.DIEMDANHs.Where(x => x.MaHV == SelectedItem.MaHV && x.MALOP == SelectedItem.MALOP).ToList();
                    if (relatedDiemDanhs.Any())
                    {
                        // Has dependencies, perform soft-delete by setting status to 'Huy'
                        SelectedItem.TRANGTHAI = "Huy";
                        DataProvider.Ins.DB.SaveChanges();
                        MessageBox.Show("Học viên đã có dữ liệu điểm danh nên không thể xóa hoàn toàn khỏi cơ sở dữ liệu.\nTrạng thái đăng ký đã được chuyển thành 'Hủy'.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        // Safe to hard-delete
                        DataProvider.Ins.DB.DANGKYLOPs.Remove(SelectedItem);
                        DataProvider.Ins.DB.SaveChanges();
                        ListDangKy.Remove(SelectedItem);
                        MessageBox.Show("Đã xóa đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa/hủy (Có thể do dữ liệu ràng buộc): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
