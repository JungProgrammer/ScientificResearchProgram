using System;
using System.Windows;
using System.Windows.Data;

namespace ResearchProgram
{
    public class VisibilityToColumnLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Visibility vis = (System.Windows.Visibility)value;

            if (vis == System.Windows.Visibility.Visible)
                return new System.Windows.GridLength(35, System.Windows.GridUnitType.Star);
            else
                return new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
