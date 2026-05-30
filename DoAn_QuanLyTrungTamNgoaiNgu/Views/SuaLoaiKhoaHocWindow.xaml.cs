using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Windows;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class SuaLoaiKhoaHocWindow : Window
    {
        private readonly string _maLoai;

        public SuaLoaiKhoaHocWindow(string maLoai, string tenLoaiHienTai)
        {
            InitializeComponent();
            _maLoai = maLoai;
            txtTenLoai.Text = tenLoaiHienTai;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenLoai.Text))
            {
                MessageBox.Show("Tên loại không được để trống!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    var entity = db.LOAI_KHOAHOC.Find(_maLoai);
                    if (entity != null)
                    {
                        entity.TENLOAI = txtTenLoai.Text.Trim();
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Cập nhật thành công!", "Thông báo",
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
