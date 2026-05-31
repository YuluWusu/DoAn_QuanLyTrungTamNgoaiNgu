using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using System;
using System.Linq;
using System.Windows;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class ThemPhieuChiWindow : Window
    {
        public ThemPhieuChiWindow()
        {
            InitializeComponent();
            dpNgayChi.SelectedDate = DateTime.Now;
            TudongTaoMaPC();
        }

        private void TudongTaoMaPC()
        {
            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    var lastPC = db.PHIEUCHIs.OrderByDescending(x => x.MAPC).FirstOrDefault();
                    if (lastPC != null && lastPC.MAPC.StartsWith("PC"))
                    {
                        if (int.TryParse(lastPC.MAPC.Substring(2), out int num))
                        {
                            txtMaPC.Text = "PC" + (num + 1).ToString("D6");
                        }
                        else
                        {
                            txtMaPC.Text = "PC000001";
                        }
                    }
                    else
                    {
                        txtMaPC.Text = "PC000001";
                    }
                }
            }
            catch
            {
                txtMaPC.Text = "PC000001";
            }
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPC.Text))
            {
                MessageBox.Show("Vui lòng nhập mã phiếu chi!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpNgayChi.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày chi!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtSoTien.Text, out decimal soTien) || soTien <= 0)
            {
                MessageBox.Show("Số tiền phải là số lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new QL_TRUNGTAM_TIENGANH())
                {
                    if (db.PHIEUCHIs.Any(x => x.MAPC == txtMaPC.Text.Trim()))
                    {
                        MessageBox.Show("Mã phiếu chi đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newPC = new PHIEUCHI
                    {
                        MAPC = txtMaPC.Text.Trim(),
                        NGAYCHI = dpNgayChi.SelectedDate.Value,
                        SOTIEN = soTien,
                        NOIDUNG = txtNoiDung.Text.Trim(),
                        // Lấy mã nhân viên đang đăng nhập. Nếu không có, gán tạm hoặc báo lỗi.
                        // Trong UserSession.ChucVu == "Quan ly", phải có MaNV. 
                        // Nếu không lấy được thì dùng mã cứng để test.
                        MANV_LAP = "NV0001" 
                    };

                    // Nếu UserSession có lưu MaNV thì dùng:
                    // newPC.MANV_LAP = UserSession.MaNV ?? "NV0001";
                    
                    db.PHIEUCHIs.Add(newPC);
                    db.SaveChanges();
                }

                MessageBox.Show("Thêm phiếu chi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm phiếu chi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
