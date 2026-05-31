using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Models
{
    // MOCK EXTENSION: Thêm các property và function bị thiếu tạm thời.
    // Lớp này sẽ không bị ghi đè khi cập nhật EDMX do là một file rời,
    // nhưng nếu Visual Studio báo lỗi "Type already contains definition...", bạn hãy xóa file này.
    public partial class QL_TRUNGTAM_TIENGANH : DbContext
    {
        public virtual DbSet<PHIEUCHI> PHIEUCHIs { get; set; }

        public virtual ObjectResult<SP_BaoCaoTaiChinhThang_Result> SP_BaoCaoTaiChinhThang(Nullable<int> thang, Nullable<int> nam)
        {
            var thangParameter = thang.HasValue ?
                new ObjectParameter("Thang", thang) :
                new ObjectParameter("Thang", typeof(int));
    
            var namParameter = nam.HasValue ?
                new ObjectParameter("Nam", nam) :
                new ObjectParameter("Nam", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_BaoCaoTaiChinhThang_Result>("SP_BaoCaoTaiChinhThang", thangParameter, namParameter);
        }
    }
}
