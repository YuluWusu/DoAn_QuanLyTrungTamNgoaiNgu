using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Linq;
using System.Windows;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class ThemPhongHocWindow : Window
    {
        public ThemPhongHocWindow()
        {
            InitializeComponent();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            string maPhong = txtMaPhong.Text.Trim();
            string tenPhong = txtTenPhong.Text.Trim();

            if (string.IsNullOrWhiteSpace(maPhong) || string.IsNullOrWhiteSpace(tenPhong))
            {
                MessageBox.Show("Mã và Tên phòng không được để trống!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSoGhe.Text, out int soGhe) || soGhe <= 0)
            {
                MessageBox.Show("Số ghế phải là số nguyên dương lớn hơn 0!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    if (db.PHONGHOCs.Any(x => x.MAPHONG == maPhong))
                    {
                        MessageBox.Show("Mã phòng học đã tồn tại!", "Cảnh báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (db.PHONGHOCs.Any(x => x.TENPHONG == tenPhong))
                    {
                        MessageBox.Show("Tên phòng học đã tồn tại!", "Cảnh báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var phongHoc = new PHONGHOC
                    {
                        MAPHONG = maPhong,
                        TENPHONG = tenPhong,
                        SOGHENGOI = soGhe
                    };
                    db.PHONGHOCs.Add(phongHoc);
                    db.SaveChanges();
                }
                MessageBox.Show("Thêm phòng học thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
