using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ResearchProgram
{
    class DepositsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string deposits = (string)value;
            //string[] deposits_list = deposits.Split('\n');
            //if (deposits_list.Length > 0)
            //    return deposits_list[0];
            //else
            //    return "";
            string deposits = (string)value;
            string[] deposits_list = deposits.Split('\n');
            if (deposits_list.Length > 0)
                switch (deposits_list[0])
                {
                    case "ФЦП мин обра или иные источники госзаказа(бюджет)":
                        return Settings.Default.RowColor0;
                    case "Средства Российских фондов поддержки науки":
                        return Settings.Default.RowColor1;
                    case "Средства бюджета субъекта Федерации":
                        return Settings.Default.RowColor2;
                    case "Средства хозяйствующих субъектов":
                        return Settings.Default.RowColor3;
                    case "Собственные средства":
                        return Settings.Default.RowColor4;
                    case "Физ. лица":
                        return Settings.Default.RowColor5;
                    case "Иностранные стредства":
                        return Settings.Default.RowColor6;
                }
            return "#FFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
