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
        private ObservableCollection<DANGKYLOP> _listDangKy;
        public ObservableCollection<DANGKYLOP> ListDangKy 
        { 
            get => _listDangKy; 
            set { _listDangKy = value; OnPropertyChanged(); } 
        }
        public Action OnRequestAddRegistration { get; set; }

        private DANGKYLOP _selectedItem;
        public DANGKYLOP SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        // Commands để thực hiện các thao tác
        public RelayCommand FilterCommand { get;}
        public RelayCommand AddCommand { get; }
        public RelayCommand  EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public DangKyViewModel()
        {
            ListDangKy = new ObservableCollection<DANGKYLOP>();
            FilterCommand = new RelayCommand(_ => new DANGKYLOP());
            AddCommand = new RelayCommand((p) => {
                OnRequestAddRegistration?.Invoke();
            });
             EditCommand = new RelayCommand(_=>new DANGKYLOP());
            DeleteCommand = new RelayCommand(_ => new DANGKYLOP());
        }
    }
        }
