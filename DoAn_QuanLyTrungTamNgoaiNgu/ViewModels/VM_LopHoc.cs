using DoAn_QuanLyTrungTamNgoaiNgu.Helpers;
using DoAn_QuanLyTrungTamNgoaiNgu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data.Entity;

namespace DoAn_QuanLyTrungTamNgoaiNgu.ViewModels
{
    public class VM_LopHoc : BaseViewModel
    {
        private ObservableCollection<LOPHOC> _listLopHoc;
        public ObservableCollection<LOPHOC> ListLopHoc
        {
            get => _listLopHoc;
            set { _listLopHoc = value; OnPropertyChanged(); }
        }

        private LOPHOC _selectedItem;
        public LOPHOC SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }
        public int TotalClasses => ListLopHoc?.Count ?? 0;

        public int TotalStudents => DataProvider.Ins.DB.HOCVIENs.Count();

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public VM_LopHoc()
        {
            LoadData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => {
                var lastLop = DataProvider.Ins.DB.LOPHOCs.OrderByDescending(x => x.MALOP).FirstOrDefault();
                string lastId = lastLop?.MALOP;

                string nextId = IDGenerator.GenerateNextID("LH", lastId);

                var newLop = new LOPHOC()
                {
                    MALOP = nextId,
                    TENLOP = "Lớp mới " + nextId,
                    TRANGTHAI = "Dang mo",
                    SISO_TOIDA = 25
                };

                DataProvider.Ins.DB.LOPHOCs.Add(newLop);
                DataProvider.Ins.DB.SaveChanges();
                ListLopHoc.Add(newLop);
                OnPropertyChanged(nameof(TotalClasses));
            });

            DeleteCommand = new RelayCommand<object>((p) => SelectedItem != null, (p) => {
                var lop = DataProvider.Ins.DB.LOPHOCs.Where(x => x.MALOP == SelectedItem.MALOP).FirstOrDefault();
                if (lop != null)
                {
                    DataProvider.Ins.DB.LOPHOCs.Remove(lop);
                    DataProvider.Ins.DB.SaveChanges();
                    ListLopHoc.Remove(SelectedItem);
                    OnPropertyChanged(nameof(TotalClasses));
                    OnPropertyChanged(nameof(TotalStudents));
                }
            });
        }

        void LoadData()
        {
            var data = DataProvider.Ins.DB.LOPHOCs
                       .Include(x => x.GIAOVIEN)
                       .AsNoTracking()
                       .ToList();

            ListLopHoc = new ObservableCollection<LOPHOC>(data);
        }
    }
}
