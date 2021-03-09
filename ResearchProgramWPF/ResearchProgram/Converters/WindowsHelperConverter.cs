using ResearchProgram.Classes;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ResearchProgram
{
    class WindowsHelperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string windowType = System.Convert.ToString(value);
            if (windowType == WindowsArgumentsTranfer.IsJobsWindow)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
