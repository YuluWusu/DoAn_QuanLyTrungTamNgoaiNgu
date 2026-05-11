using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
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
    public class VM_TaiKhoan : BaseViewModel
    {
        private QL_TRUNGTAM_TIENGANH data;

        // Collections for DataGrid and ComboBoxes
        private ObservableCollection<VW_TaiKhoan_Quyen> _danhSachTaiKhoan;
        public ObservableCollection<VW_TaiKhoan_Quyen> DanhSachTaiKhoan
        {
            get => _danhSachTaiKhoan;
            set { _danhSachTaiKhoan = value; OnPropertyChanged(); }
        }

        // Removed DanhSachNhanVien and DanhSachGiaoVien as per user request

        public List<string> LoaiTaiKhoanList { get; set; } = new List<string> { "Admin", "Nhân viên", "Giáo viên" };

        // DB stores unaccented values per CHECK constraint: 'Quan ly', 'Le tan', 'Ke toan'
        // Display shows accented Vietnamese in UI via mapping
        public List<string> DanhSachChucVuDisplay { get; set; } = new List<string> { "Quản lý", "Lễ tân", "Kế toán" };
        private readonly Dictionary<string, string> _chucVuDisplayToDb = new Dictionary<string, string>
        {
            { "Quản lý", "Quan ly" },
            { "Lễ tân",   "Le tan"  },
            { "Kế toán",  "Ke toan" }
        };
        private readonly Dictionary<string, string> _chucVuDbToDisplay = new Dictionary<string, string>
        {
            { "Quan ly", "Quản lý" },
            { "Le tan",  "Lễ tân"   },
            { "Ke toan", "Kế toán"  }
        };

        // DB stores: 'Cu nhan', 'Thac si', 'Tien si'
        public List<string> DanhSachTrinhDoDisplay { get; set; } = new List<string> { "Cử nhân", "Thạc sĩ", "Tiến sĩ" };
        private readonly Dictionary<string, string> _trinhDoDisplayToDb = new Dictionary<string, string>
        {
            { "Cử nhân", "Cu nhan"  },
            { "Thạc sĩ",  "Thac si"  },
            { "Tiến sĩ",  "Tien si"  }
        };
        private readonly Dictionary<string, string> _trinhDoDbToDisplay = new Dictionary<string, string>
        {
            { "Cu nhan", "Cử nhân" },
            { "Thac si", "Thạc sĩ"  },
            { "Tien si", "Tiến sĩ"  }
        };

        // Form Fields
        private string _hoTen;
        public string HoTen
        {
            get => _hoTen;
            set { _hoTen = value; OnPropertyChanged(); }
        }

        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set { _tenDangNhap = value; OnPropertyChanged(); }
        }

        private string _matKhau;
        public string MatKhau
        {
            get => _matKhau;
            set { _matKhau = value; OnPropertyChanged(); }
        }

        private bool _trangThai;
        public bool TrangThai
        {
            get => _trangThai;
            set { _trangThai = value; OnPropertyChanged(); }
        }

        private string _selectedLoaiTaiKhoan;
        public string SelectedLoaiTaiKhoan
        {
            get => _selectedLoaiTaiKhoan;
            set
            {
                _selectedLoaiTaiKhoan = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ChucVuVisibility));
                OnPropertyChanged(nameof(NhanVienFieldsVisibility));
                OnPropertyChanged(nameof(GiaoVienFieldsVisibility));
                // Admin mặc định Chức vụ là Quản lý
                if (value == "Admin") SelectedChucVuDisplay = "Quản lý";
                else if (value != "Nhân viên") SelectedChucVuDisplay = null;
            }
        }

        private string _selectedChucVuDisplay;
        public string SelectedChucVuDisplay
        {
            get => _selectedChucVuDisplay;
            set { _selectedChucVuDisplay = value; OnPropertyChanged(); }
        }

        private string _selectedTrinhDoDisplay;
        public string SelectedTrinhDoDisplay
        {
            get => _selectedTrinhDoDisplay;
            set { _selectedTrinhDoDisplay = value; OnPropertyChanged(); }
        }

        public Visibility ChucVuVisibility => SelectedLoaiTaiKhoan == "Nhân viên" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NhanVienFieldsVisibility => SelectedLoaiTaiKhoan == "Nhân viên" || SelectedLoaiTaiKhoan == "Admin" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility GiaoVienFieldsVisibility => SelectedLoaiTaiKhoan == "Giáo viên" ? Visibility.Visible : Visibility.Collapsed;

        // Extra NHANVIEN / GIAOVIEN fields
        private string _sdt;
        public string SDT { get => _sdt; set { _sdt = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _diaChi;
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }

        private VW_TaiKhoan_Quyen _selectedTaiKhoan;
        public VW_TaiKhoan_Quyen SelectedTaiKhoan
        {
            get => _selectedTaiKhoan;
            set
            {
                _selectedTaiKhoan = value;
                OnPropertyChanged();
                if (_selectedTaiKhoan != null && !IsAddMode && IsFormVisible)
                {
                    LoadDetailForm(_selectedTaiKhoan);
                }
            }
        }

        // Search
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                FilterData();
            }
        }

        // Visibility & Mode
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set { _isFormVisible = value; OnPropertyChanged(); }
        }

        private bool _isAddMode;
        public bool IsAddMode
        {
            get => _isAddMode;
            set { _isAddMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEditMode)); }
        }
        public bool IsEditMode => !IsAddMode;

        // Commands
        public ICommand ThemCommand { get; set; }
        public ICommand SuaCommand { get; set; }
        public ICommand XoaCommand { get; set; }
        public ICommand LuuCommand { get; set; }
        public ICommand HuyCommand { get; set; }

        public VM_TaiKhoan()
        {
            data = new QL_TRUNGTAM_TIENGANH();
            LoadData();

            ThemCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                IsAddMode = true;
                IsFormVisible = true;
                ClearForm();
            });

            SuaCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                if (SelectedTaiKhoan == null)
                {
                    MessageBox.Show("Vui lòng chọn một tài khoản từ danh sách để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                IsAddMode = false;
                IsFormVisible = true;
                LoadDetailForm(SelectedTaiKhoan);
            });

            HuyCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                IsFormVisible = false;
                ClearForm();
            });

            LuuCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                if (string.IsNullOrWhiteSpace(TenDangNhap))
                {
                    MessageBox.Show("Vui lòng nhập Tên đăng nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(HoTen))
                {
                    MessageBox.Show("Vui lòng nhập Họ và tên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (SelectedLoaiTaiKhoan == "Nhân viên" && string.IsNullOrWhiteSpace(SelectedChucVuDisplay))
                {
                    MessageBox.Show("Vui lòng chọn Chức vụ cho Nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    if (IsAddMode)
                    {
                        if (string.IsNullOrWhiteSpace(MatKhau))
                        {
                            MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (data.TAIKHOANs.Any(x => x.TENDANGNHAP == TenDangNhap.Trim()))
                        {
                            MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        string newMaTK = GenerateNewId();
                        TAIKHOAN tk = new TAIKHOAN
                        {
                            MATK = newMaTK,
                            TENDANGNHAP = TenDangNhap.Trim(),
                            MATKHAU = MatKhau,
                            TRANGTHAI = TrangThai,
                            NGAYTAO = DateTime.Now
                        };

                        if (SelectedLoaiTaiKhoan == "Admin")
                        {
                            string newMaNV = GenerateNewMaNV();
                            data.NHANVIENs.Add(new NHANVIEN
                            {
                                MaNV = newMaNV,
                                HoTen = HoTen.Trim(),
                                ChucVu = "Quan ly",
                                SDT = string.IsNullOrWhiteSpace(SDT) ? null : SDT.Trim(),
                                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                                NgayVaoLam = DateTime.Now
                            });
                            tk.MaNV = newMaNV;
                        }
                        else if (SelectedLoaiTaiKhoan == "Nhân viên")
                        {
                            string chucVuDb = _chucVuDisplayToDb.ContainsKey(SelectedChucVuDisplay ?? "") ? _chucVuDisplayToDb[SelectedChucVuDisplay] : SelectedChucVuDisplay;
                            string newMaNV = GenerateNewMaNV();
                            data.NHANVIENs.Add(new NHANVIEN
                            {
                                MaNV = newMaNV,
                                HoTen = HoTen.Trim(),
                                ChucVu = chucVuDb,
                                SDT = string.IsNullOrWhiteSpace(SDT) ? null : SDT.Trim(),
                                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                                NgayVaoLam = DateTime.Now
                            });
                            tk.MaNV = newMaNV;
                        }
                        else if (SelectedLoaiTaiKhoan == "Giáo viên")
                        {
                            string trinhDoDb = _trinhDoDisplayToDb.ContainsKey(SelectedTrinhDoDisplay ?? "") ? _trinhDoDisplayToDb[SelectedTrinhDoDisplay] : null;
                            string newMaGV = GenerateNewMaGV();
                            data.GIAOVIENs.Add(new GIAOVIEN
                            {
                                MaGV = newMaGV,
                                TenGV = HoTen.Trim(),
                                SDT = string.IsNullOrWhiteSpace(SDT) ? null : SDT.Trim(),
                                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                                DiaChi = string.IsNullOrWhiteSpace(DiaChi) ? null : DiaChi.Trim(),
                                TrinhDo = trinhDoDb,
                                NgayVaoLam = DateTime.Now
                            });
                            tk.MaGV = newMaGV;
                        }

                        data.TAIKHOANs.Add(tk);
                        data.SaveChanges();

                        // Nếu không làm phần quyền, có thể bỏ qua bước thêm QUYEN.
                        // Nhưng thông thường Admin/NV/GV cần 1 QUYEN cơ bản để đăng nhập được.
                        // Ở đây cứ thêm bình thường, quyền có thể fix sau nếu cần.

                        MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsAddMode = false;
                        LoadData();
                    }
                    else
                    {
                        var tk = data.TAIKHOANs.FirstOrDefault(x => x.MATK == SelectedTaiKhoan.MATK);
                        if (tk != null)
                        {
                            tk.TENDANGNHAP = TenDangNhap.Trim();
                            if (!string.IsNullOrWhiteSpace(MatKhau))
                            {
                                tk.MATKHAU = MatKhau;
                            }
                            tk.TRANGTHAI = TrangThai;

                            if (SelectedLoaiTaiKhoan == "Admin" || SelectedLoaiTaiKhoan == "Nhân viên")
                            {
                                string chucVuDb = SelectedLoaiTaiKhoan == "Admin" ? "Quan ly" : (_chucVuDisplayToDb.ContainsKey(SelectedChucVuDisplay ?? "") ? _chucVuDisplayToDb[SelectedChucVuDisplay] : SelectedChucVuDisplay);
                                if (tk.MaNV != null)
                                {
                                    tk.NHANVIEN.HoTen = HoTen.Trim();
                                    tk.NHANVIEN.ChucVu = chucVuDb;
                                    tk.NHANVIEN.SDT = string.IsNullOrWhiteSpace(SDT) ? null : SDT.Trim();
                                    tk.NHANVIEN.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
                                    if (tk.NHANVIEN.NgayVaoLam == null) tk.NHANVIEN.NgayVaoLam = DateTime.Now;
                                }
                                else
                                {
                                    string newMaNV = GenerateNewMaNV();
                                    data.NHANVIENs.Add(new NHANVIEN
                                    {
                                        MaNV = newMaNV,
                                        HoTen = HoTen.Trim(),
                                        ChucVu = chucVuDb,
                                        SDT = string.IsNullOrWhiteSpace(SDT) ? null : SDT.Trim(),
                                        Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                                        NgayVaoLam = DateTime.Now
                                    });
                                    tk.MaNV = newMaNV;
                                }
                                tk.MaGV = null;
                            }
                            else if (SelectedLoaiTaiKhoan == "Giáo viên")
                            {
                                string trinhDoDb = _trinhDoDisplayToDb.ContainsKey(SelectedTrinhDoDisplay ?? "") ? _trinhDoDisplayToDb[SelectedTrinhDoDisplay] : null;
                                if (tk.MaGV != null)
                                {
                                    tk.GIAOVIEN.TenGV = HoTen.Trim();
                                    tk.GIAOVIEN.SDT = string.IsNullOrWhiteSpace(SDT) ? null : SDT.Trim();
                                    tk.GIAOVIEN.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
                                    tk.GIAOVIEN.DiaChi = string.IsNullOrWhiteSpace(DiaChi) ? null : DiaChi.Trim();
                                    tk.GIAOVIEN.TrinhDo = trinhDoDb;
                                    if (tk.GIAOVIEN.NgayVaoLam == null) tk.GIAOVIEN.NgayVaoLam = DateTime.Now;
                                }
                                else
                                {
                                    string newMaGV = GenerateNewMaGV();
                                    data.GIAOVIENs.Add(new GIAOVIEN
                                    {
                                        MaGV = newMaGV,
                                        TenGV = HoTen.Trim(),
                                        SDT = string.IsNullOrWhiteSpace(SDT) ? null : SDT.Trim(),
                                        Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                                        DiaChi = string.IsNullOrWhiteSpace(DiaChi) ? null : DiaChi.Trim(),
                                        TrinhDo = trinhDoDb,
                                        NgayVaoLam = DateTime.Now
                                    });
                                    tk.MaGV = newMaGV;
                                }
                                tk.MaNV = null;
                            }

                            data.SaveChanges();
                            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData();
                        }
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    string errorMsg = "";
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            errorMsg += string.Format("Property: {0} Error: {1}\n", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    MessageBox.Show("Lỗi Data: " + errorMsg, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    if (ex.InnerException != null)
                    {
                        msg += "\nChi tiết: " + ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            msg += "\n" + ex.InnerException.InnerException.Message;
                        }
                    }
                    MessageBox.Show("Lỗi: " + msg, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            XoaCommand = new RelayCommand<object>((p) => SelectedTaiKhoan != null, (p) =>
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này không?\nThao tác này không thể hoàn tác!", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string matk = SelectedTaiKhoan.MATK.Trim();
                        var tk = data.TAIKHOANs.FirstOrDefault(x => x.MATK == SelectedTaiKhoan.MATK);
                        if (tk == null) return;

                        string maNV = tk.MaNV?.Trim();
                        string maGV = tk.MaGV?.Trim();

                        // 1. Xóa quyền liên quan trực tiếp bằng SQL
                        data.Database.ExecuteSqlCommand(
                            "DELETE FROM TAIKHOAN_QUYEN WHERE MATK = @p0", matk);

                        // 2. Xóa tài khoản
                        data.TAIKHOANs.Remove(tk);
                        data.SaveChanges();

                        // 3. Xóa NHANVIEN nếu không còn tài khoản nào liên kết
                        if (!string.IsNullOrEmpty(maNV))
                        {
                            bool conLienKet = data.TAIKHOANs.Any(x => x.MaNV == maNV);
                            if (!conLienKet)
                            {
                                data.Database.ExecuteSqlCommand(
                                    "DELETE FROM NHANVIEN WHERE MaNV = @p0", maNV);
                            }
                        }
                        else if (!string.IsNullOrEmpty(maGV))
                        {
                            bool conLienKet = data.TAIKHOANs.Any(x => x.MaGV == maGV);
                            if (!conLienKet)
                            {
                                data.Database.ExecuteSqlCommand(
                                    "DELETE FROM GIAOVIEN WHERE MaGV = @p0", maGV);
                            }
                        }

                        MessageBox.Show("Đã xóa tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsFormVisible = false;
                        ClearForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        string errMsg = ex.Message;
                        if (ex.InnerException != null) errMsg += "\nChi tiết: " + ex.InnerException.Message;
                        MessageBox.Show("Lỗi khi xóa: " + errMsg, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });

            ClearForm();
            IsAddMode = false;
            IsFormVisible = false;
        }

        private void LoadData()
        {
            data = new QL_TRUNGTAM_TIENGANH(); // Làm mới context để cập nhật dữ liệu từ View
            FilterData();
        }

        private void FilterData()
        {
            var query = data.VW_TaiKhoan_Quyen.AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(x => x.TENDANGNHAP.Contains(SearchKeyword) || x.HoTenNguoiDung.Contains(SearchKeyword));
            }
            DanhSachTaiKhoan = new ObservableCollection<VW_TaiKhoan_Quyen>(query.ToList());
        }

        private void LoadDetailForm(VW_TaiKhoan_Quyen tkView)
        {
            if (tkView == null) return;
            
            var tk = data.TAIKHOANs.FirstOrDefault(x => x.MATK == tkView.MATK);
            if (tk != null)
            {
                TenDangNhap = tk.TENDANGNHAP;
                MatKhau = ""; // Don't show old password
                TrangThai = tk.TRANGTHAI;

                if (!string.IsNullOrEmpty(tk.MaNV))
                {
                    string chucVuDb = tk.NHANVIEN?.ChucVu?.Trim() ?? "";
                    if (chucVuDb == "Quan ly")
                    {
                        SelectedLoaiTaiKhoan = "Admin";
                        SelectedChucVuDisplay = null;
                    }
                    else
                    {
                        SelectedLoaiTaiKhoan = "Nhân viên";
                        SelectedChucVuDisplay = _chucVuDbToDisplay.ContainsKey(chucVuDb) ? _chucVuDbToDisplay[chucVuDb] : chucVuDb;
                    }
                    HoTen = tk.NHANVIEN?.HoTen;
                    SDT = tk.NHANVIEN?.SDT;
                    Email = tk.NHANVIEN?.Email;
                }
                else if (!string.IsNullOrEmpty(tk.MaGV))
                {
                    SelectedLoaiTaiKhoan = "Giáo viên";
                    HoTen = tk.GIAOVIEN?.TenGV;
                    SDT = tk.GIAOVIEN?.SDT;
                    Email = tk.GIAOVIEN?.Email;
                    DiaChi = tk.GIAOVIEN?.DiaChi;
                    string trinhDoDb = tk.GIAOVIEN?.TrinhDo?.Trim() ?? "";
                    SelectedTrinhDoDisplay = _trinhDoDbToDisplay.ContainsKey(trinhDoDb) ? _trinhDoDbToDisplay[trinhDoDb] : trinhDoDb;
                }
                else
                {
                    SelectedLoaiTaiKhoan = "Admin";
                    HoTen = "";
                }
            }
        }

        private void ClearForm()
        {
            HoTen = "";
            TenDangNhap = "";
            MatKhau = "";
            TrangThai = true;
            SDT = "";
            Email = "";
            DiaChi = "";
            SelectedTrinhDoDisplay = null;
            SelectedLoaiTaiKhoan = "Admin";
            SelectedChucVuDisplay = null;
        }

        // CanSave removed as validation is now inside LuuCommand

        private string GenerateNewId()
        {
            var allTk = data.TAIKHOANs.ToList();
            if (!allTk.Any()) return "TK0001";

            int maxId = 0;
            foreach (var tk in allTk)
            {
                string id = tk.MATK.Trim();
                if (id.StartsWith("TK") && int.TryParse(id.Substring(2), out int num))
                {
                    if (num > maxId) maxId = num;
                }
            }
            return "TK" + (maxId + 1).ToString("D4");
        }

        private string GenerateNewMaNV()
        {
            var allNv = data.NHANVIENs.ToList();
            if (!allNv.Any()) return "NV0001";

            int maxId = 0;
            foreach (var nv in allNv)
            {
                string id = nv.MaNV.Trim();
                if (id.StartsWith("NV") && int.TryParse(id.Substring(2), out int num))
                {
                    if (num > maxId) maxId = num;
                }
            }
            return "NV" + (maxId + 1).ToString("D4");
        }

        private string GenerateNewMaGV()
        {
            var allGv = data.GIAOVIENs.ToList();
            if (!allGv.Any()) return "GV0001";

            int maxId = 0;
            foreach (var gv in allGv)
            {
                string id = gv.MaGV.Trim();
                if (id.StartsWith("GV") && int.TryParse(id.Substring(2), out int num))
                {
                    if (num > maxId) maxId = num;
                }
            }
            return "GV" + (maxId + 1).ToString("D4");
        }
    }
}
