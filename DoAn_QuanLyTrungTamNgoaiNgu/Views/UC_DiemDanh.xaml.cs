using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DoAn_QuanLyTrungTamNgoaiNgu.ViewModels;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Views
{
    /// <summary>
    /// Interaction logic for UC_DiemDanh.xaml
    /// </summary>
    public partial class UC_DiemDanh : UserControl
    {
        public UC_DiemDanh()
        {
            InitializeComponent();
            this.DataContext = new VM_DiemDanh();
        }
    }
}
