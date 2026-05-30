using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xaml;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class DiemDanhItem
    {
        public string HoTenHS { get; set; }
        public int VangKP { get; set; }
        public int VangCP { get; set; }
        public int Tre {  get; set; }
    }
    internal class VM_DiemDanh : BaseViewModel
    {
        private ObservableCollection<VW_DanhSachDiemDanh> _listDiemDanh;
        public ObservableCollection<VW_DanhSachDiemDanh> ListDiemDanh
        {
            get => _listDiemDanh;
            set
            {
                _listDiemDanh = value; OnPropertyChanged();
            }
        }
        private ObservableCollection<LOPHOC> _listLop;
        public ObservableCollection<LOPHOC> ListLop
        {
            get => _listLop;
            set
            {
                _listLop = value; OnPropertyChanged();
            }
        }
        private LOPHOC _selectedLop;
        public LOPHOC SelectedLop
        {
            get => _selectedLop;
            set
            {
                _selectedLop = value; OnPropertyChanged(); LoadHocVien();
            }
        }
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value; OnPropertyChanged(); LoadDanhSachLop();
            }
        }
        private int _tongVangKP;
        public int TongVangKP
        {
            get => _tongVangKP;
            set { _tongVangKP = value; OnPropertyChanged(); }
        }
        private int _tongVangCP;
        public int TongVangCP
        {
            get => _tongVangCP;
            set
            {
                _tongVangCP = value; OnPropertyChanged();
            }
        }
        private int _tongTre;
        public int TongTre
        {
            get => _tongTre;
            set
            {
                _tongTre = value; OnPropertyChanged();
            }
        }
        private ObservableCollection<DiemDanhItem> _diemDanhList;
        public ObservableCollection<DiemDanhItem> DiemDanhList
        {
            get => _diemDanhList;
            set {  _diemDanhList = value; OnPropertyChanged(); }
        }
        public ICommand SaveCommand { get; set; }
        public VM_DiemDanh()
        {
            LoadDanhSachLop();
            SaveCommand = new RelayCommand<object>((p) => SelectedLop != null, (p) =>
            {
                try
                {
                    using (var db = new QL_TRUNGTAM_TIENGANH())
                    {
                        foreach (var item in ListDiemDanh)
                        {
                            var idHV = item.MaHV?.Trim();
                            var idLop = SelectedLop.MALOP.Trim();
                            var date = SelectedDate.Date;

                            string sql = "IF EXISTS (SELECT 1 FROM DIEMDANH WHERE MaHV = {0} AND MALOP = {1} AND NGAYDD = {2}) " +
                                         "UPDATE DIEMDANH SET TRANGTHAI = {3}, GHICHU = {4} WHERE MaHV = {0} AND MALOP = {1} AND NGAYDD = {2} " +
                                         "ELSE INSERT INTO DIEMDANH (MaHV, MALOP, NGAYDD, TRANGTHAI, GHICHU) VALUES ({0}, {1}, {2}, {3}, {4})";

                            db.Database.ExecuteSqlCommand(sql, idHV, idLop, date, item.TRANGTHAI ?? "Co mat", item.GHICHU ?? "");
                        }
                    }

                    
                    LoadHocVien();

                    MessageBox.Show("Lưu thông tin điểm danh thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi điểm danh: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
        void LoadDanhSachLop()
        {
            var date = SelectedDate.Date;
            var LopCoLichHoc = DataProvider.Ins.DB.LICHOCs
                .Where(x => x.NGAYHOC == date)
                .Select(x => x.MALOP)
                .ToList();
            var rawData = DataProvider.Ins.DB.LOPHOCs
                .Where(x => x.TRANGTHAI == "Dang mo"
                    && x.NGAYBATDAU <= date
                    && x.NGAYKETTHUC >= date
                    && LopCoLichHoc.Contains(x.MALOP))
                .OrderBy(x => x.TENLOP)
                .ToList();
            ListLop = new ObservableCollection<LOPHOC>(rawData);
            if (SelectedLop != null && !rawData.Any(x=>x.MALOP == SelectedLop.MALOP))
            {
                SelectedLop = null;
                ListDiemDanh = null;
                DiemDanhList = null;
            }
            else if(SelectedLop!= null)
            {
                LoadHocVien();
            }
        }
        void LoadHocVien()
        {
            if (SelectedLop == null) return;

            var rawDangKy = DataProvider.Ins.DB.DANGKYLOPs
                .AsNoTracking()
                .Include("HOCVIEN")
                .Where(x => x.MALOP == SelectedLop.MALOP)
                .ToList();
            var rawDiemDanh = DataProvider.Ins.DB.DIEMDANHs
                .AsNoTracking()
                .Where(x => x.MALOP == SelectedLop.MALOP
                && x.NGAYDD == SelectedDate.Date)
                .ToList();

            var result = rawDangKy.Select(x => {
                var dd = rawDiemDanh.FirstOrDefault(d => d.MaHV == x.MaHV);
                return new VW_DanhSachDiemDanh
                {
                    MaHV = x.MaHV,
                    HoTen = x.HOCVIEN.HoTen,
                    MALOP = x.MALOP,
                    NGAYDD = SelectedDate.Date,
                    TRANGTHAI = dd?.TRANGTHAI ?? "Co mat",
                    GHICHU = dd?.GHICHU
                };
            }).ToList();
            ListDiemDanh = new ObservableCollection<VW_DanhSachDiemDanh>(result);
            RefreshDiemDanh();
        }
        void RefreshDiemDanh()
        {
            if (SelectedLop == null) return;

            var allRecords = DataProvider.Ins.DB.DIEMDANHs
                .AsNoTracking()
                .Include("DANGKYLOP.HOCVIEN")
                .Where(x => x.MALOP == SelectedLop.MALOP)
                .ToList();

            var diemdanh = allRecords
                .GroupBy(x=> new {x.MaHV, HoTen = x.DANGKYLOP.HOCVIEN.HoTen})
                .Select(g=>new DiemDanhItem
                { 
                    HoTenHS = g.Key.HoTen,
                    VangKP = g.Count(x=> x.TRANGTHAI == "Vang KP"),
                    VangCP = g.Count(x => x.TRANGTHAI == "Vang CP"),
                    Tre = g.Count(x => x.TRANGTHAI == "Tre"),
                })
                .OrderByDescending(x=>x.VangKP + x.VangCP + x.Tre)
                .ToList();
            DiemDanhList = new ObservableCollection<DiemDanhItem>(diemdanh);
            TongVangKP = diemdanh.Sum(x => x.VangKP);
            TongVangCP = diemdanh.Sum(x=>x.VangCP);
            TongTre = diemdanh.Sum(x=> x.Tre);
        }
    }
}
