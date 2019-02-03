using System;
using System.Globalization;
using System.Windows.Data;

namespace HearthStoneSimGui.View.Extensions
{
    public class OpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enabled = value != null && (bool)value;
            if (enabled) return 1;
            return 0.3;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
