using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System.Data.Entity;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity.Validation;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_HocVien : BaseViewModel
    {

        public static DANGKYLOP SelectedDKStatic;

        public static HOCVIEN NewHVStatic = new HOCVIEN();

        public static LOPHOC SelectedLopStatic;


        public ObservableCollection<DANGKYLOP> _dsHV;
        public ObservableCollection<DANGKYLOP> dsHV
        {
            get => _dsHV;
            set
            {
                _dsHV = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LOPHOC> _dsLop;
        public ObservableCollection<LOPHOC> dsLop
        {
            get => _dsLop;
            set
            {
                _dsLop = value;
                OnPropertyChanged();
            }
        }

        public List<string> dsGioiTinh { get; set; }

        public HOCVIEN NewHV
        {
            get => NewHVStatic;
            set
            {
                NewHVStatic = value;
                OnPropertyChanged();
            }
        }

        public LOPHOC SelectedLop
        {
            get => SelectedLopStatic;
            set
            {
                SelectedLopStatic = value;
                OnPropertyChanged();
            }
        }

        public DANGKYLOP SelectedDK
        {
            get => SelectedDKStatic;
            set
            {
                SelectedDKStatic = value;
                OnPropertyChanged();

                if (value != null)
                {
                    NewHV = new HOCVIEN()
                    {
                        MaHV = value.HOCVIEN.MaHV,
                        HoTen = value.HOCVIEN.HoTen,
                        NgaySinh = value.HOCVIEN.NgaySinh,
                        GioiTinh = value.HOCVIEN.GioiTinh,
                        DiaChi = value.HOCVIEN.DiaChi,
                        SDT = value.HOCVIEN.SDT,
                        Email = value.HOCVIEN.Email,
                        TrangThai = value.HOCVIEN.TrangThai
                    };

                    SelectedLop = value.LOPHOC;
                }
            }
        }

        private ICollectionView _dsHVView;
        public ICollectionView dsHVView
        {
            get => _dsHVView;
            set { _dsHVView = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterList(); 
            }
        }
        void FilterList()
        {
            if (dsHVView == null) return;

            dsHVView.Filter = (obj) =>
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return true;

                var dk = obj as DANGKYLOP;
                if (dk == null) return false;

                string search = SearchText.ToLower();
               
                return dk.HOCVIEN.MaHV.ToLower().Contains(search) ||
                       dk.HOCVIEN.HoTen.ToLower().Contains(search);
            };
        }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public VM_HocVien()
        {
            dsGioiTinh = new List<string>()
            {
                "Nam",
                "Nu"
            };

            LoadData();

            AddCommand = new RelayCommand<object>((p) => true,(p) => Add());

            UpdateCommand = new RelayCommand<object>((p) => true,(p) => Update());

            DeleteCommand = new RelayCommand<DANGKYLOP>((p) => p != null,(p) => Delete(p));

            CancelCommand = new RelayCommand<object>((p) => true,(p) => Cancel());

            BackCommand = new RelayCommand<object>((p) => true,(p) => Back());
        }

        void LoadData()
        {
            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    var allDk = db.DANGKYLOPs
                        .Include(x => x.HOCVIEN)
                        .Include(x => x.LOPHOC)
                        .Include(x => x.LOPHOC.KHOA_HOC)
                        .ToList();

                    var uniqueDk = allDk.GroupBy(x => x.MaHV)
                        .Select(g => g.OrderByDescending(x => x.NGAYDK).FirstOrDefault())
                        .ToList();

                    dsHV = new ObservableCollection<DANGKYLOP>(uniqueDk);
                    dsHVView = CollectionViewSource.GetDefaultView(dsHV);
                    dsLop = new ObservableCollection<LOPHOC>(db.LOPHOCs.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        string TaoMaHV(QL_TRUNGTAM_TIENGANH db)
        {
            var lastHV = db.HOCVIENs.OrderByDescending(x => x.MaHV).FirstOrDefault();

            if (lastHV == null)
                return "HV0001";

            int number = int.Parse(lastHV.MaHV.Substring(2));

            number++;

            return "HV" + number.ToString("D4");
        }

        void Add()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewHV.HoTen))
                {
                    MessageBox.Show("Vui lòng nhập họ tên!");
                    return;
                }

                if (NewHV.NgaySinh==null)
                {
                    MessageBox.Show("Vui lòng chọn ngày sinh!");
                    return;
                }

                if (NewHV.NgaySinh.Value.Date >= DateTime.Today)
                {
                    MessageBox.Show("Ngày sinh không được là hôm nay hoặc tương lai!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewHV.GioiTinh))
                {
                    MessageBox.Show("Vui lòng chọn giới tính!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewHV.SDT))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!");
                    return;
                }

                if (!NewHV.SDT.All(char.IsDigit))
                {
                    MessageBox.Show("Số điện thoại chỉ được chứa số!");
                    return;
                }

                if (NewHV.SDT.Length != 10)
                {
                    MessageBox.Show("Số điện thoại phải đủ 10 số!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewHV.Email))
                {
                    MessageBox.Show("Vui lòng nhập email!");
                    return;
                }
                if (!NewHV.Email.EndsWith("@gmail.com"))
                {
                    MessageBox.Show("Email phải có dạng ten@gmail.com!");
                    return;
                }

                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    if (db.HOCVIENs.Any(x => x.SDT == NewHV.SDT))
                    {
                        MessageBox.Show("Số điện thoại đã tồn tại!");
                        return;
                    }

                    if (SelectedLop == null)
                    {
                        MessageBox.Show("Vui lòng chọn lớp học!");
                        return;
                    }

                    string ma = TaoMaHV(db);

                    try
                    {
                        db.Database.ExecuteSqlCommand(
                            "EXEC SP_ThemHocVien {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                            ma, NewHV.HoTen, NewHV.NgaySinh, NewHV.GioiTinh, NewHV.DiaChi, NewHV.SDT, NewHV.Email
                        );
                        
                        db.Database.ExecuteSqlCommand(
                            "INSERT INTO DANGKYLOP(MaHV, MALOP, NGAYDK, HOCPHI, TRANGTHAI) VALUES({0}, {1}, {2}, {3}, {4})",
                            ma, SelectedLop.MALOP, DateTime.Now, 0, "Dang hoc"
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi Database: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                LoadData();

                MessageBox.Show("Thêm học viên thành công!","Thông báo",MessageBoxButton.OK,MessageBoxImage.Information);

                Cancel();

                Back();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("\n", errorMessages);

                var exceptionMessage = string.Concat("Dữ liệu không hợp lệ:\n", fullErrorMessage);

                MessageBox.Show(exceptionMessage, "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thêm học viên!\n" + (ex.InnerException?.Message ?? ex.Message), "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void Update()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewHV.HoTen))
                {
                    MessageBox.Show("Vui lòng nhập họ tên!");
                    return;
                }

                if (NewHV.NgaySinh == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày sinh!");
                    return;
                }

                if (NewHV.NgaySinh.Value.Date >= DateTime.Today)
                {
                    MessageBox.Show("Ngày sinh không được là hôm nay hoặc tương lai!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewHV.GioiTinh))
                {
                    MessageBox.Show("Vui lòng chọn giới tính!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewHV.SDT))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!");
                    return;
                }

                if (!NewHV.SDT.All(char.IsDigit))
                {
                    MessageBox.Show("Số điện thoại chỉ được chứa số!");
                    return;
                }

                if (NewHV.SDT.Length != 10)
                {
                    MessageBox.Show("Số điện thoại phải đủ 10 số!");
                    return;
                }

                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    var hv = db.HOCVIENs.FirstOrDefault(x => x.MaHV == NewHV.MaHV);

                    if (hv == null)
                    {
                        MessageBox.Show("Không tìm thấy học viên!");
                        return;
                    }

                    if (db.HOCVIENs.Any(x => x.SDT == NewHV.SDT && x.MaHV != NewHV.MaHV))
                    {
                        MessageBox.Show("Số điện thoại đã tồn tại!");
                        return;
                    }

                    if (db.HOCVIENs.Any(x => x.Email == NewHV.Email && x.MaHV != NewHV.MaHV))
                    {
                        MessageBox.Show("Email đã tồn tại!");
                        return;
                    }

                    try
                    {
                        db.Database.ExecuteSqlCommand(
                            "UPDATE HOCVIEN SET HoTen = {0}, NgaySinh = {1}, GioiTinh = {2}, DiaChi = {3}, SDT = {4}, Email = {5} WHERE MaHV = {6}",
                            NewHV.HoTen, NewHV.NgaySinh, NewHV.GioiTinh, NewHV.DiaChi, NewHV.SDT, NewHV.Email, NewHV.MaHV
                        );

                        if (SelectedLop != null)
                        {
                            db.Database.ExecuteSqlCommand(
                                "UPDATE DANGKYLOP SET MALOP = {0} WHERE MaHV = {1} AND MALOP = {2}",
                                SelectedLop.MALOP, NewHV.MaHV, SelectedDK.MALOP
                            );
                        }

                        db.Database.ExecuteSqlCommand(
                            "EXEC SP_CapNhatTrangThaiHocVien {0}, {1}",
                            NewHV.MaHV, "Dang hoc"
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                LoadData();

                MessageBox.Show("Cập nhật thông tin học viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                Back();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("\n", errorMessages);

                var exceptionMessage = string.Concat("Dữ liệu không hợp lệ:\n", fullErrorMessage);

                MessageBox.Show(exceptionMessage, "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã xảy ra lỗi khi cập nhật học viên!\n" +
                    (ex.InnerException?.Message ?? ex.Message),
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        void Delete(DANGKYLOP dk)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa học viên này không?","Xác nhận xóa",MessageBoxButton.YesNo,MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    var dkDB = db.DANGKYLOPs.FirstOrDefault(x => x.MaHV == dk.MaHV);

                    if (dkDB != null)
                    {
                        db.DANGKYLOPs.Remove(dkDB);
                    }

                    var hv = db.HOCVIENs.FirstOrDefault(x => x.MaHV == dk.MaHV);

                    if (hv != null)
                    {
                        db.HOCVIENs.Remove(hv);
                    }

                    db.SaveChanges();
                }

                LoadData();

                MessageBox.Show("Xóa học viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã xảy ra lỗi khi xóa học viên!\n" +(ex.InnerException?.Message ?? ex.Message),"Thông báo",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        public void Cancel()
        {
            NewHV = new HOCVIEN();

            SelectedLop = null;

            SelectedDK = null;
        }

        void Back()
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item.DataContext is VM_TrangChu vm)
                {
                    vm.NoiDungHienTai = new UC_HocVien();
                }
            }
        }
    }
}