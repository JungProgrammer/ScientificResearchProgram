using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ResearchProgram.Converters
{
    class AggregationCountConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int selectedIndex = (int)value;

            if (selectedIndex == 1)
                return Visibility.Visible;
            else
                return
                    Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
