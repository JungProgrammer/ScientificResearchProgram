using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ResearchProgram
{
    class DepositsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;

            var rand = new Random(seed);

            int xuy = rand.Next();


            return new SolidColorBrush(xuy % 2 == 0 ? Colors.Red : Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
