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
    public class VM_LoaiKhoaHoc : BaseViewModel
    {
        private ObservableCollection<LOAI_KHOAHOC> _dsLoai;
        public ObservableCollection<LOAI_KHOAHOC> DsLoai
        {
            get => _dsLoai;
            set { _dsLoai = value; OnPropertyChanged(); }
        }

        private LOAI_KHOAHOC _selectedLoai;
        public LOAI_KHOAHOC SelectedLoai
        {
            get => _selectedLoai;
            set { _selectedLoai = value; OnPropertyChanged(); }
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

        public VM_LoaiKhoaHoc()
        {
            LoadData();

            ThemCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                var win = new ThemLoaiKhoaHocWindow();
                if (win.ShowDialog() == true)
                    LoadData();
            });

            SuaCommand = new RelayCommand<object>(
                (p) => SelectedLoai != null,
                (p) =>
                {
                    var win = new SuaLoaiKhoaHocWindow(SelectedLoai.MALOAI_KH, SelectedLoai.TENLOAI);
                    if (win.ShowDialog() == true)
                        LoadData();
                });

            XoaCommand = new RelayCommand<object>(
                (p) => SelectedLoai != null,
                (p) =>
                {
                    var result = MessageBox.Show(
                        $"Bạn có chắc muốn xóa loại khóa học \"{SelectedLoai.TENLOAI}\"?",
                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes) return;

                    try
                    {
                        using (var db = new QL_TRUNGTAM_TIENGANH())
                        {
                            // Kiểm tra có khóa học con không
                            if (db.KHOA_HOC.Any(x => x.MALOAI_KH == SelectedLoai.MALOAI_KH))
                            {
                                MessageBox.Show("Không thể xóa vì còn khóa học thuộc loại này!",
                                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            var entity = db.LOAI_KHOAHOC.Find(SelectedLoai.MALOAI_KH);
                            if (entity != null)
                            {
                                db.LOAI_KHOAHOC.Remove(entity);
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
                    var query = db.LOAI_KHOAHOC.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(TuKhoa))
                        query = query.Where(x => x.TENLOAI.Contains(TuKhoa));
                    DsLoai = new ObservableCollection<LOAI_KHOAHOC>(query.OrderBy(x => x.MALOAI_KH).ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
    }
}
