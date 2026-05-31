using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Linq;
using System.Windows;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class ThemLoaiKhoaHocWindow : Window
    {
        public ThemLoaiKhoaHocWindow()
        {
            InitializeComponent();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            string maLoai = txtMaLoai.Text.Trim();
            string tenLoai = txtTenLoai.Text.Trim();

            if (string.IsNullOrWhiteSpace(maLoai) || string.IsNullOrWhiteSpace(tenLoai))
            {
                MessageBox.Show("Mã và Tên loại khóa học không được để trống!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    if (db.LOAI_KHOAHOC.Any(x => x.MALOAI_KH == maLoai))
                    {
                        MessageBox.Show("Mã loại khóa học đã tồn tại!", "Cảnh báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (db.LOAI_KHOAHOC.Any(x => x.TENLOAI == tenLoai))
                    {
                        MessageBox.Show("Tên loại khóa học đã tồn tại!", "Cảnh báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var loaiKh = new LOAI_KHOAHOC
                    {
                        MALOAI_KH = maLoai,
                        TENLOAI = tenLoai
                    };
                    db.LOAI_KHOAHOC.Add(loaiKh);
                    db.SaveChanges();
                }
                MessageBox.Show("Thêm loại khóa học thành công!", "Thông báo",
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
