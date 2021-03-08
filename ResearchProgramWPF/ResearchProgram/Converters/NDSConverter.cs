using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ResearchProgram
{
    class NDSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "Отображение с НДС";
            }
            else
            {
                return "Отображение без НДС";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
