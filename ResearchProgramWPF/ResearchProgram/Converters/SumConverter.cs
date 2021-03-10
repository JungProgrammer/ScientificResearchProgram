using System;
using System.Globalization;
using System.Windows.Data;

namespace ResearchProgram.Forms
{
    public class SumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() != "Сумма:")
            {
                return value.ToString() + " руб. ";
            }
            else return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
