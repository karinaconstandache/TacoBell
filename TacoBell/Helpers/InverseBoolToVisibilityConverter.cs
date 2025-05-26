using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace TacoBell.Helpers
{
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Handle boolean values
            if (value is bool boolValue)
                return boolValue ? Visibility.Collapsed : Visibility.Visible;

            // Handle integer values (for collection counts)
            if (value is int count)
                return count > 0 ? Visibility.Collapsed : Visibility.Visible;

            // Default case
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}