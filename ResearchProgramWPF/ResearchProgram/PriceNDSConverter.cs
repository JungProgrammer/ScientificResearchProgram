using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ResearchProgram
{
    class PriceNDSConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string textBoxValue = value.ToString();
            if (textBoxValue.Length > 0)
                return (Math.Round(System.Convert.ToDouble(textBoxValue) * 1 / Settings.Default.NDSValue, 2)).ToString();
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
