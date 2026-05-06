using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using DoAn_QuanLyTrungTamNgoaiNgu.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
        public class DangKyViewModel : BaseViewModel
    {
        private ObservableCollection<DangKyLop> _listDangKy;
        public ObservableCollection<DangKyLop> ListDangKy 
        { 
            get => _listDangKy; 
            set { _listDangKy = value; OnPropertyChanged(); } 
        }
        public Action OnRequestAddRegistration { get; set; }

        private DangKyLop _selectedItem;
        public DangKyLop SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        // Commands để thực hiện các thao tác
        public ICommand FilterCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public DangKyViewModel()
        {
            ListDangKy = new ObservableCollection<DangKyLop>();
            FilterCommand = new RelayCommand(_ => new DangKyLop());
            AddCommand = new RelayCommand((p) => {
                OnRequestAddRegistration?.Invoke();
            });
             EditCommand = new RelayCommand(_=>new DangKyLop());
            DeleteCommand = new RelayCommand(_ => new DangKyLop());
        }
    }
        }
