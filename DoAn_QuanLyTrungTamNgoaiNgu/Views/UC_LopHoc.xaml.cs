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
        public UC_LopHoc()
        {
            InitializeComponent();
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
}
