using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    static class FormsManager
    {
        public static createPersonWindow CreatePersonWindow;
        public static CreateGrantWindow CreateGrantWindow;


        public static void UpdateOpenedWindows()
        {
            // Обновление окна создания/редактирования человека
            if (CreatePersonWindow != null)
            {
                CreatePersonWindow.UpdateDataAsync();
            }

            // Обновление окна создания/редактирования договора
            if(CreateGrantWindow != null)
            {

            } 
        }
    }
}
