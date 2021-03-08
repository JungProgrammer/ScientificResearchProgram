using System;
using System.Globalization;
using System.Windows.Data;

namespace ResearchProgram.Forms
{
    public class NOCConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool _boolValue = System.Convert.ToBoolean(value);
            if (_boolValue)
            {
                return "Да";
            }
            else
            {
                return "Нет";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
