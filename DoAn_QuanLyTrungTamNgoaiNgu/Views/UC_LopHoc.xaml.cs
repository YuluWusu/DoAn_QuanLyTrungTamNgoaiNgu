using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    public partial class UC_LopHoc : UserControl
    {
        private VM_LopHoc _vm;
        public UC_LopHoc()
        {
            InitializeComponent();
            _vm = new VM_LopHoc();
            this.DataContext = _vm;

            _vm.OnEditRequested += OnEditRequested;
            _vm.OnCancelRequested += OnCancelRequested;
        }

        private void OnEditRequested(LOPHOC item)
        {
            var malop = item?.MALOP; 

            var dlg = new EditLopHoc(malop)
            {
                Owner = Window.GetWindow(this)
            };
            if (dlg.ShowDialog() == true)
                _vm.LoadData();
        }

        private void OnCancelRequested(LOPHOC item)
        {
            var confirm = MessageBox.Show(
                $"Hủy lớp {item.TENLOP}?\nLịch học tương lai sẽ bị xóa.",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            DataProvider.Ins.DB.Database.ExecuteSqlCommand(
                "EXEC SP_HuyLop @MaLop",
                new System.Data.SqlClient.SqlParameter("@MaLop", item.MALOP));

            item.TRANGTHAI = "Huy";
            CommandManager.InvalidateRequerySuggested();
            _vm.ApplyFilter(); 
        }

    }
}
namespace DoAn_QuanLyTrungTamNgoaiNgu.CustomControls
{
    public class NoAutomationCalendar : System.Windows.Controls.Calendar
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return null;
        }
    }
    public class SearchBox : Control
    {

    }
}
