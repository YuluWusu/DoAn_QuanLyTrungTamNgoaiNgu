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
 
         public ICommand CancelCommand { get; set; }
         public ICommand SaveCommand { get; set; }
         public ICommand EditCommand { get; set; }
         public ICommand RefreshFormCommand { get; set; }
         public ICommand RefreshTableCommand { get; set; }
         public Action RequestNavigateBack { get; set; }
 
         public VM_DangKyMoi()
         {
             LoadData();
             TrangThai = "Cho dong tien"; // Default status according to SQL
             
             CancelCommand = new RelayCommand(p => RequestNavigateBack?.Invoke());
             SaveCommand = new RelayCommand(p => ExecuteSave());
             EditCommand = new RelayCommand(p => ExecuteEdit());
             RefreshFormCommand = new RelayCommand(p => ExecuteRefreshForm());
             RefreshTableCommand = new RelayCommand(p => ExecuteRefreshTable());
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

         private void ExecuteRefreshForm()
         {
             SelectedHocVien = null;
             SelectedLopHoc = null;
             HocPhi = 0;
             TrangThai = "Cho dong tien";
         }

         private void ExecuteRefreshTable()
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
             if (SelectedItem == null && !IsEditing)
             {
                 MessageBox.Show("Vui lòng chọn đăng ký cần sửa trong bảng bên phải!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
 
         private void ExecuteSave()
         {
             if (IsEditing) return; // Không cho phép thêm mới khi đang sửa bảng
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
                 
                 // Làm mới danh sách sau khi thêm thành công
                 LoadData();
                 ExecuteRefreshForm();
             }
             catch (Exception ex)
             {
                 MessageBox.Show("Lỗi khi lưu vào cơ sở dữ liệu (Có thể học viên đã đăng ký lớp này rồi): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
             }
         }
    }
}
