using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class EditLopHoc : Window
    {
        private readonly string _maLop;
        private LOPHOC _lop;

        public EditLopHoc(string maLop = null)
        {
            InitializeComponent();
            _maLop = maLop;
            LoadData();
        }

        void LoadData()
        {
            if (_maLop == null)
                LoadTaoMoi();
            else
                LoadChinhSua();
        }

        void LoadTaoMoi()
        {
            txbHeader.Text = "Tạo lớp mới";
            txbMaLop.Text = "";
            pnlTaoMoi.Visibility = Visibility.Visible;
            pnlChinhSua.Visibility = Visibility.Collapsed;
            btnSave.Content = "Tạo lớp";

            var lastMa = DataProvider.Ins.DB.LOPHOCs
                .OrderByDescending(x => x.MALOP)
                .Select(x => x.MALOP)
                .FirstOrDefault();
            if (lastMa != null && lastMa.StartsWith("L") &&
                int.TryParse(lastMa.Trim().Substring(1), out int num))
                txtMaLop.Text = "L" + (num + 1).ToString("D4");
            else
                txtMaLop.Text = "L0001";

            cboKhoaHoc.ItemsSource = DataProvider.Ins.DB.KHOA_HOC.OrderBy(x => x.TENKH).ToList();
            cboGiaoVienMoi.ItemsSource = DataProvider.Ins.DB.GIAOVIENs.OrderBy(x => x.TenGV).ToList();
            cboPhong.ItemsSource = DataProvider.Ins.DB.PHONGHOCs.OrderBy(x => x.TENPHONG).ToList();
            cboCa.ItemsSource = DataProvider.Ins.DB.CAHOCs.OrderBy(x => x.TENCA).ToList();
            dpNgayBatDau.SelectedDate = DateTime.Today;
        }

        void LoadChinhSua()
        {
            txbHeader.Text = "Chỉnh sửa lớp học";
            pnlTaoMoi.Visibility = Visibility.Collapsed;
            pnlChinhSua.Visibility = Visibility.Visible;
            btnSave.Content = "Lưu thay đổi";

            _lop = DataProvider.Ins.DB.LOPHOCs.FirstOrDefault(x => x.MALOP == _maLop);
            if (_lop == null)
            {
                MessageBox.Show("Không tìm thấy lớp học.", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); return;
            }

            txbMaLop.Text = $"Mã lớp: {_lop.MALOP}";
            txtTenLop.Text = _lop.TENLOP;

            var ngayHocCuaLop = DataProvider.Ins.DB.LICHOCs
                .Where(l => l.MALOP == _maLop).Select(l => l.NGAYHOC).ToList();
            var maGVBan = DataProvider.Ins.DB.LICHOCs
                .Where(l => l.MALOP != _maLop && ngayHocCuaLop.Contains(l.NGAYHOC))
                .Select(l => l.LOPHOC.MaGV).Distinct().ToList();

            cboGiaoVien.ItemsSource = DataProvider.Ins.DB.GIAOVIENs
                .Where(g => !maGVBan.Contains(g.MaGV) || g.MaGV == _lop.MaGV)
                .OrderBy(g => g.TenGV).ToList();
            cboGiaoVien.SelectedValue = _lop.MaGV;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_maLop == null)
                SaveTaoMoi();
            else
                SaveChinhSua();
        }

        void SaveTaoMoi()
        {
            if (string.IsNullOrWhiteSpace(txtMaLop.Text) ||
                string.IsNullOrWhiteSpace(txtTenLopMoi.Text))
            { MessageBox.Show("Vui lòng nhập đầy đủ Mã lớp và Tên lớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (cboKhoaHoc.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn khóa học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (cboGiaoVienMoi.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn giáo viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (dpNgayBatDau.SelectedDate == null)
            { MessageBox.Show("Vui lòng chọn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (cboPhong.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn phòng học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (cboCa.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn ca học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            bool coNgayHoc = chkT2.IsChecked == true || chkT3.IsChecked == true ||
                             chkT4.IsChecked == true || chkT5.IsChecked == true ||
                             chkT6.IsChecked == true || chkT7.IsChecked == true ||
                             chkCN.IsChecked == true;
            if (!coNgayHoc)
            { MessageBox.Show("Vui lòng chọn ít nhất một ngày học trong tuần.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (!int.TryParse(txtSiSo.Text, out int siSo) || siSo <= 0)
            { MessageBox.Show("Sĩ số tối đa phải là số nguyên dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

  
            if (DataProvider.Ins.DB.LOPHOCs.Any(x => x.MALOP == txtMaLop.Text.Trim()))
            { MessageBox.Show("Mã lớp đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            try
            {
                var conn = DataProvider.Ins.DB.Database.Connection;
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SP_MoLopMoi";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@MaLop", txtMaLop.Text.Trim()));
                    cmd.Parameters.Add(new SqlParameter("@TenLop", txtTenLopMoi.Text.Trim()));
                    cmd.Parameters.Add(new SqlParameter("@MaKH", cboKhoaHoc.SelectedValue.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@MaGV", cboGiaoVienMoi.SelectedValue.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@NgayBatDau", dpNgayBatDau.SelectedDate.Value.Date));
                    cmd.Parameters.Add(new SqlParameter("@MaPhong", cboPhong.SelectedValue.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@MaCa", cboCa.SelectedValue.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@T2", chkT2.IsChecked == true ? 1 : 0));
                    cmd.Parameters.Add(new SqlParameter("@T3", chkT3.IsChecked == true ? 1 : 0));
                    cmd.Parameters.Add(new SqlParameter("@T4", chkT4.IsChecked == true ? 1 : 0));
                    cmd.Parameters.Add(new SqlParameter("@T5", chkT5.IsChecked == true ? 1 : 0));
                    cmd.Parameters.Add(new SqlParameter("@T6", chkT6.IsChecked == true ? 1 : 0));
                    cmd.Parameters.Add(new SqlParameter("@T7", chkT7.IsChecked == true ? 1 : 0));
                    cmd.Parameters.Add(new SqlParameter("@CN", chkCN.IsChecked == true ? 1 : 0));

                    int? soBuoi = null;
                    if (int.TryParse(txtSoBuoi.Text, out int sb) && sb > 0)
                        soBuoi = sb;
                    cmd.Parameters.Add(new SqlParameter("@SoBuoi", soBuoi.HasValue ? (object)soBuoi.Value : DBNull.Value));
                    cmd.ExecuteNonQuery();
                }

                
                var lopMoi = DataProvider.Ins.DB.LOPHOCs.Find(txtMaLop.Text.Trim());
                if (lopMoi != null) { lopMoi.SISO_TOIDA = siSo; DataProvider.Ins.DB.SaveChanges(); }

                MessageBox.Show("Tạo lớp thành công! Lịch học đã được sinh tự động.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void SaveChinhSua()
        {
            if (string.IsNullOrWhiteSpace(txtTenLop.Text))
            { MessageBox.Show("Tên lớp không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (cboGiaoVien.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn giáo viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            _lop.TENLOP = txtTenLop.Text.Trim();
            _lop.MaGV = cboGiaoVien.SelectedValue.ToString();
            DataProvider.Ins.DB.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}