using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram.Classes
{
    class PersonsFilters
    {

        public static bool IsActive()
        {
            Type thisType = typeof(PersonsFilters);
            FieldInfo[] Field = thisType.GetFields();
            for (int i = 0; i < Field.Length; i++)
            {
                if (Field[i].GetValue(null) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
