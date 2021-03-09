using System;
using System.Globalization;
using System.Windows.Data;
namespace ResearchProgram
{
    public class JobConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Job)value != null)
            {
                return ((Job)value).Salary.ToString();
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }
}
