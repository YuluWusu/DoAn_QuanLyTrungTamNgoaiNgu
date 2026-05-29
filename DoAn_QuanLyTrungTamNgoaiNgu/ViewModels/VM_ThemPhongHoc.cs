using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_ThemPhongHoc : BaseViewModel
    {
        private string _tenPhong;
        public string TenPhong
        {
            get => _tenPhong;
            set { _tenPhong = value; OnPropertyChanged(); }
        }

        private int _soGheNgoi = 30;
        public int SoGheNgoi
        {
            get => _soGheNgoi;
            set { _soGheNgoi = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public VM_ThemPhongHoc()
        {
            SaveCommand = new RelayCommand<Window>((p) => !string.IsNullOrWhiteSpace(TenPhong) && SoGheNgoi > 0, (p) =>
            {
                try
                {
                    using (var db = new QL_TRUNGTAM_TIENGANH())
                    {
                        var last = db.PHONGHOCs.OrderByDescending(x => x.MAPHONG).FirstOrDefault();
                        string ma = "PH01";
                        if (last != null)
                        {
                            string numStr = new string(last.MAPHONG.Where(char.IsDigit).ToArray());
                            if (int.TryParse(numStr, out int num))
                            {
                                ma = "PH" + (num + 1).ToString("D2");
                            }
                        }

                        db.PHONGHOCs.Add(new PHONGHOC
                        {
                            MAPHONG = ma,
                            TENPHONG = TenPhong,
                            SOGHENGOI = SoGheNgoi
                        });
                        db.SaveChanges();
                    }
                    MessageBox.Show("Thêm phòng học thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
