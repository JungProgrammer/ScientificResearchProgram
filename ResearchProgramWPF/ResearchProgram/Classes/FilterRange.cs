using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram.Classes
{
    /// <summary>
    /// Класс олицетворяющий диапазон значений в фильтрах
    /// хранит в себе начальное значение, конечное значение и знаки больше/меньше/равно
    /// </summary>
    public class FilterRange
    {
        public enum Signs{
            More,
            Less,
            MoreEqual,
            LessEqual,
            Equal,
            None
        }
        public string LeftValue { get; set; }
        public string RightValue { get; set; }
        public Signs LeftSign { get; set; }
        public Signs RightSign { get; set; }
        public Signs LeftDateSign { get; set; }
        public Signs RightDateSign { get; set; }
        public DateTime? LeftDate { get; set; }
        public DateTime? RightDate { get; set; }


        public FilterRange()
        {
            LeftValue = null;
            RightValue = null;
            LeftSign = Signs.None;
            RightSign = Signs.None;
            LeftDateSign = Signs.None;
            RightDateSign = Signs.None;
            LeftDate = null;
            RightDate = null;
        }

        public bool isActive()
        {
            if (LeftValue != null || LeftDate != null || RightDate != null || RightValue != null)
            {
                return true;
            }
            return false;
        }
    }
}
