using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ResearchProgram
{
    public static class Utilities
    {
        public static bool ComparativeOperator(string sign, string x, string y)
        {
            switch (sign)
            {
                case "<":
                    return Parser.ConvertToRightFloat(x) < Parser.ConvertToRightFloat(y);
                case ">":
                    return Parser.ConvertToRightFloat(x) > Parser.ConvertToRightFloat(y);
                case "<=":
                    return Parser.ConvertToRightFloat(x) <= Parser.ConvertToRightFloat(y);
                case ">=":
                    return Parser.ConvertToRightFloat(x) >= Parser.ConvertToRightFloat(y);
                case "=":
                    return Math.Abs(Parser.ConvertToRightFloat(x) - Parser.ConvertToRightFloat(y)) <= 0.00000001f;
                default:
                    return false;
            }


            
        }

        //Функция для ввода в текст бокс только чисел с одним разделителем
        public static void TextBoxNumbersPreviewInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (DS_Count(((TextBox)sender).Text) < 1)));
        }

        // функция подсчета разделителя
        public static int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }

    }
}
