using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram.Classes
{
    class PersonsFilters
    {
        public static string FIO;

        public static bool? Sex;

        public static ObservableCollection<WorkDegree> workDegrees;

        public static ObservableCollection<WorkRank> workRanks;

        public static ObservableCollection<WorkCategories> WorkCategories;

        public static FilterRange Age;

        public static FilterRange BirthDate;

        public static bool? IsMainWorkPlace;

        public static ObservableCollection<UniversityStructureNode> FirstNode;

        public static ObservableCollection<UniversityStructureNode> SecondNode;

        public static ObservableCollection<UniversityStructureNode> ThirdNode;

        public static ObservableCollection<UniversityStructureNode> FourthNode;


        public static void ResetFilters()
        {
            FIO = null;
            Sex = null;
            workDegrees = null;
            workRanks = null;
            WorkCategories = null;
            Age = null;
            BirthDate = null;
            IsMainWorkPlace = null;
            FirstNode = null;
            SecondNode = null;
            ThirdNode = null;
            FourthNode = null;
        }
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
