using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ResearchProgram
{
    public class LaboratoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((Kafedra)value == null)
            {
                // Значит, что нам не нужно этот комбобокс видеть
                if(parameter.ToString() == "KafedraSelected")
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }
            }
            else
            {
                if (parameter.ToString() == "KafedraSelected")
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

