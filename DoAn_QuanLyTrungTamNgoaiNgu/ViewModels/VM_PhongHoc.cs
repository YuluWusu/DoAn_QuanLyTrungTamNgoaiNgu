using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_PhongHoc : BaseViewModel
    {
        private ObservableCollection<PHONGHOC> _dsPhong;
        public ObservableCollection<PHONGHOC> DsPhong
        {
            get => _dsPhong;
            set { _dsPhong = value; OnPropertyChanged(); }
        }

        private PHONGHOC _selectedPhong;
        public PHONGHOC SelectedPhong
        {
            get => _selectedPhong;
            set { _selectedPhong = value; OnPropertyChanged(); }
        }

        private string _tuKhoa = "";
        public string TuKhoa
        {
            get => _tuKhoa;
            set
            {
                _tuKhoa = value;
                OnPropertyChanged();
                LoadData();
            }
        }

        public ICommand ThemCommand { get; set; }
        public ICommand SuaCommand  { get; set; }
        public ICommand XoaCommand  { get; set; }

        public VM_PhongHoc()
        {
            LoadData();

            ThemCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                var win = new ThemPhongHocWindow();
                if (win.ShowDialog() == true)
                    LoadData();
            });

            SuaCommand = new RelayCommand<object>(
                (p) => SelectedPhong != null,
                (p) =>
                {
                    var win = new SuaPhongHocWindow(SelectedPhong.MAPHONG, SelectedPhong.TENPHONG, SelectedPhong.SOGHENGOI);
                    if (win.ShowDialog() == true)
                        LoadData();
                });

            XoaCommand = new RelayCommand<object>(
                (p) => SelectedPhong != null,
                (p) =>
                {
                    var result = MessageBox.Show(
                        $"Bạn có chắc muốn xóa phòng \"{SelectedPhong.TENPHONG}\"?",
                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes) return;

                    try
                    {
                        using (var db = new QL_TRUNGTAM_TIENGANH())
                        {
                            if (db.LICHOCs.Any(x => x.MAPHONG == SelectedPhong.MAPHONG))
                            {
                                MessageBox.Show("Không thể xóa vì phòng này đang có lịch học!",
                                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            var entity = db.PHONGHOCs.Find(SelectedPhong.MAPHONG);
                            if (entity != null)
                            {
                                db.PHONGHOCs.Remove(entity);
                                db.SaveChanges();
                            }
                        }
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
        }

        public void LoadData()
        {
            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    var query = db.PHONGHOCs.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(TuKhoa))
                        query = query.Where(x => x.TENPHONG.Contains(TuKhoa));
                    DsPhong = new ObservableCollection<PHONGHOC>(query.OrderBy(x => x.MAPHONG).ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
    }
}
