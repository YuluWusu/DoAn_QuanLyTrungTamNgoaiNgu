using System.Windows.Controls;
using DoAn_QuanLyTrungTamNgoaiNgu.ViewModels;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class UC_TaiKhoan : UserControl
    {
        public UC_TaiKhoan()
        {
            InitializeComponent();
            this.DataContext = new VM_TaiKhoan();
        }
    }
}
