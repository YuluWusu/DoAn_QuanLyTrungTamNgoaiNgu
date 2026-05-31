using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_BaoCaoTaiChinh : BaseViewModel
    {
        private ObservableCollection<PHIEUTHU> _dsPhieuThu;
        public ObservableCollection<PHIEUTHU> DsPhieuThu
        {
            get => _dsPhieuThu;
            set { _dsPhieuThu = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PHIEUCHI> _dsPhieuChi;
        public ObservableCollection<PHIEUCHI> DsPhieuChi
        {
            get => _dsPhieuChi;
            set { _dsPhieuChi = value; OnPropertyChanged(); }
        }

        private PHIEUCHI _selectedPhieuChi;
        public PHIEUCHI SelectedPhieuChi
        {
            get => _selectedPhieuChi;
            set { _selectedPhieuChi = value; OnPropertyChanged(); }
        }

        private decimal _tongThu;
        public decimal TongThu
        {
            get => _tongThu;
            set { _tongThu = value; OnPropertyChanged(); }
        }

        private decimal _tongChi;
        public decimal TongChi
        {
            get => _tongChi;
            set { _tongChi = value; OnPropertyChanged(); }
        }

        private decimal _loiNhuan;
        public decimal LoiNhuan
        {
            get => _loiNhuan;
            set { _loiNhuan = value; OnPropertyChanged(); }
        }

        private int _thangHienTai;
        public int ThangHienTai
        {
            get => _thangHienTai;
            set { _thangHienTai = value; OnPropertyChanged(); LoadData(); }
        }

        private int _namHienTai;
        public int NamHienTai
        {
            get => _namHienTai;
            set { _namHienTai = value; OnPropertyChanged(); LoadData(); }
        }

        public ICommand ThemPhieuChiCommand { get; set; }
        public ICommand XoaPhieuChiCommand { get; set; }

        public ObservableCollection<int> DanhSachThang { get; set; }
        public ObservableCollection<int> DanhSachNam { get; set; }

        public VM_BaoCaoTaiChinh()
        {
            DanhSachThang = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            DanhSachNam = new ObservableCollection<int> { 2023, 2024, 2025, 2026, 2027 };

            ThangHienTai = DateTime.Now.Month;
            NamHienTai = DateTime.Now.Year;

            ThemPhieuChiCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                var win = new ThemPhieuChiWindow();
                if (win.ShowDialog() == true)
                {
                    LoadData();
                }
            });

            XoaPhieuChiCommand = new RelayCommand<object>(
                (p) => SelectedPhieuChi != null,
                (p) =>
                {
                    var result = MessageBox.Show($"Bạn có chắc muốn xóa phiếu chi {SelectedPhieuChi.MAPC}?", 
                        "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            using (var db = new QL_TRUNGTAM_TIENGANH())
                            {
                                var pc = db.PHIEUCHIs.Find(SelectedPhieuChi.MAPC);
                                if (pc != null)
                                {
                                    db.PHIEUCHIs.Remove(pc);
                                    db.SaveChanges();
                                    LoadData();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            );
        }

        public void LoadData()
        {
            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    // Lấy DS Thu trong tháng
                    var lstThu = db.PHIEUTHUs
                        .Where(x => x.NGAYTHU.Month == ThangHienTai && x.NGAYTHU.Year == NamHienTai)
                        .OrderByDescending(x => x.NGAYTHU).ToList();
                    DsPhieuThu = new ObservableCollection<PHIEUTHU>(lstThu);

                    // Lấy DS Chi trong tháng
                    var lstChi = db.PHIEUCHIs
                        .Where(x => x.NGAYCHI.Month == ThangHienTai && x.NGAYCHI.Year == NamHienTai)
                        .OrderByDescending(x => x.NGAYCHI).ToList();
                    DsPhieuChi = new ObservableCollection<PHIEUCHI>(lstChi);

                    // Tính tổng từ SP (Hoặc có thể tính luôn từ List cho nhanh, nhưng yêu cầu dùng SP)
                    var res = db.SP_BaoCaoTaiChinhThang(ThangHienTai, NamHienTai).FirstOrDefault();
                    if (res != null)
                    {
                        TongThu = res.TongThu ?? 0;
                        TongChi = res.TongChi ?? 0;
                        LoiNhuan = res.LoiNhuanRong ?? 0;
                    }
                    else
                    {
                        TongThu = 0; TongChi = 0; LoiNhuan = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu tài chính: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class IsNegativeConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal num)
                return num < 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

