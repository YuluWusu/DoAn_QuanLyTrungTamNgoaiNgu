using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Windows;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class SuaPhongHocWindow : Window
    {
        private readonly string _maPhong;

        public SuaPhongHocWindow(string maPhong, string tenPhong, int soGhe)
        {
            InitializeComponent();
            _maPhong = maPhong;
            txtTenPhong.Text = tenPhong;
            txtSoGhe.Text = soGhe.ToString();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenPhong.Text))
            {
                MessageBox.Show("Tên phòng không được để trống!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtSoGhe.Text, out int soGhe) || soGhe <= 0)
            {
                MessageBox.Show("Số ghế phải là số nguyên dương!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    var entity = db.PHONGHOCs.Find(_maPhong);
                    if (entity != null)
                    {
                        entity.TENPHONG = txtTenPhong.Text.Trim();
                        entity.SOGHENGOI = soGhe;
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Cập nhật phòng học thành công!", "Thông báo",
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
