using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Parser
    {
        /// <summary>
        /// Возвращает правильное вещественное значение. Если пользователь, например, ввел точку вместо запятой
        /// </summary>
        /// <param name="floatNum"></param>
        /// <returns></returns>
        public static float ConvertToRightFloat(string floatNum)
        {
            try
            {
                if (floatNum != null)
                {
                    return float.Parse(floatNum.Replace('.', ','));
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
