using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemoryGame.Services
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string stringParam && stringParam.Equals("Inverse", StringComparison.OrdinalIgnoreCase))
                {
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibilityValue)
            {
                if (parameter is string stringParam && stringParam.Equals("Inverse", StringComparison.OrdinalIgnoreCase))
                {
                    return visibilityValue != Visibility.Visible;
                }
                return visibilityValue == Visibility.Visible;
            }
            return false;
        }
    }
}