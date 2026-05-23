using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System; 
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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
            set { _filterSearchQuery = value; OnPropertyChanged(); }
        }

        public Action OnRequestAddRegistration { get; set; }

        private DANGKYLOP _selectedItem;
        public DANGKYLOP SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        // Commands để thực hiện các thao tác
        public RelayCommand FilterCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public VM_DangKy()
        {
            LoadData();

            FilterCommand = new RelayCommand(_ => ExecuteFilter());
            
            AddCommand = new RelayCommand((p) => {
                OnRequestAddRegistration?.Invoke();
            });
            
            EditCommand = new RelayCommand(_ => { /* Logic Edit nếu cần */ });
            
            DeleteCommand = new RelayCommand(_ => ExecuteDelete());
        }

        public void LoadData()
        {
            // Load từ DB với Include để hiển thị thông tin liên quan nếu cần
            var list = DataProvider.Ins.DB.DANGKYLOPs
                .Include("HOCVIEN")
                .Include("LOPHOC")
                .ToList();
            ListDangKy = new ObservableCollection<DANGKYLOP>(list);
        }

        private void ExecuteFilter()
        {
            var query = DataProvider.Ins.DB.DANGKYLOPs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(FilterSearchQuery))
            {
                string search = FilterSearchQuery.ToLower();
                query = query.Where(x => x.MaHV.ToLower().Contains(search) 
                                      || x.MALOP.ToLower().Contains(search));
            }
            ListDangKy = new ObservableCollection<DANGKYLOP>(query.Include("HOCVIEN").Include("LOPHOC").ToList());
        }

        private void ExecuteDelete()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đăng ký để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đăng ký của HV {SelectedItem.MaHV} cho Lớp {SelectedItem.MALOP}?",
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataProvider.Ins.DB.DANGKYLOPs.Remove(SelectedItem);
                    DataProvider.Ins.DB.SaveChanges();
                    ListDangKy.Remove(SelectedItem);
                    MessageBox.Show("Đã xóa thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa (Có thể do dữ liệu ràng buộc): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
