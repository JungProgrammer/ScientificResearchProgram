using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
