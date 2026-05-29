using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_ThemLoaiKhoaHoc : BaseViewModel
    {
        private string _tenLoai;
        public string TenLoai
        {
            get => _tenLoai;
            set { _tenLoai = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public VM_ThemLoaiKhoaHoc()
        {
            SaveCommand = new RelayCommand<Window>((p) => !string.IsNullOrWhiteSpace(TenLoai), (p) =>
            {
                try
                {
                    using (var db = new QL_TRUNGTAM_TIENGANH())
                    {
                        var last = db.LOAI_KHOAHOC.OrderByDescending(x => x.MALOAI_KH).FirstOrDefault();
                        string ma = "LKH01";
                        if (last != null)
                        {
                            string numStr = new string(last.MALOAI_KH.Where(char.IsDigit).ToArray());
                            if (int.TryParse(numStr, out int num))
                            {
                                ma = "LKH" + (num + 1).ToString("D2");
                            }
                        }

                        db.LOAI_KHOAHOC.Add(new LOAI_KHOAHOC
                        {
                            MALOAI_KH = ma,
                            TENLOAI = TenLoai
                        });
                        db.SaveChanges();
                    }
                    MessageBox.Show("Thêm loại khóa học thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    p?.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            CancelCommand = new RelayCommand<Window>((p) => true, (p) => p?.Close());
        }
    }
}
