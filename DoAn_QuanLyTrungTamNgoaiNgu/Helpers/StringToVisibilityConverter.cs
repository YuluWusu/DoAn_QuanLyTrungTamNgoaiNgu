using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DoAn_QuanLyTrungTamNgoaiNgu.Helpers
{
    /// <summary>
    /// Trả về Visible nếu chuỗi không rỗng, Collapsed nếu rỗng/null.
    /// Dùng cho TextBlock thông báo lỗi.
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
