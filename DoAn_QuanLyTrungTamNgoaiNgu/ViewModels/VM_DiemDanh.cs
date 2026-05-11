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

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
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
                _selectedDate = value; OnPropertyChanged(); LoadHocVien();
            }
        }
        public ICommand SaveCommand { get; set; }
        public VM_DiemDanh()
        {
            ListLop = new ObservableCollection<LOPHOC>(DataProvider.Ins.DB.LOPHOCs.Where(x => x.TRANGTHAI == "Dang mo"));
            SaveCommand = new RelayCommand<object>((p) => SelectedLop != null, (p) =>
            {
                try
                {
                    foreach (var item in ListDiemDanh)
                    {
                        DataProvider.Ins.DB.SP_SaveDiemDanh(item.MaHV, SelectedLop.MALOP, SelectedDate, item.TRANGTHAI, item.GHICHU);
                    }
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Lưu thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi save:" + ex.Message);
                }
            });
        }
        void LoadHocVien()
        {
            if (SelectedLop == null) return;
            var data = DataProvider.Ins.DB.VW_DanhSachDiemDanh.Where(x => x.MALOP == SelectedLop.MALOP && (x.NGAYDD == SelectedDate))
                .ToList();
            if (data.Count ==0)
            {
                data = DataProvider.Ins.DB.VW_DanhSachDiemDanh
                    .Where(x => x.MALOP == SelectedLop.MALOP)
                    .GroupBy(x => x.MaHV)
                    .Select(g => g.FirstOrDefault())
                    .ToList();
                foreach(var item in data)
                {
                    item.TRANGTHAI = "Co mat";
                    item.NGAYDD = SelectedDate;
                }
            }
            ListDiemDanh = new ObservableCollection<VW_DanhSachDiemDanh>(data);
        }
    }
}
