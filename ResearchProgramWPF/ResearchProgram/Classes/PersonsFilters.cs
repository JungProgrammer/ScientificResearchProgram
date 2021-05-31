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

        public static string GetPersonQuarry()
        {
            string quarry = "SELECT persons.id pid FROM persons " +
                "LEFT JOIN work_degree wd ON persons.degree_id = wd.id " +
                "LEFT JOIN work_rank wr on persons.rank_id = wr.id " +
                "JOIN persons_work_places ON persons.id = persons_work_places.person_id " +
                "LEFT JOIN work_categories wc on persons_work_places.category_id = wc.id " ;

            List<string> quarryList = new List<string>();
            string tempQuarry;

            if (FIO != null)
            {
                quarryList.Add("( FIO LIKE '%" + FIO + "%' )");
            }

            if (BirthDate != null)
            {
                if (BirthDate.LeftDate != null)
                {
                    quarryList.Add("( birthdate " + FilterRange.GetSignLiteral(BirthDate.LeftDateSign) + " \'" + BirthDate.LeftDate?.ToString("yyyy-MM-dd") + "\') ");
                }
                if (BirthDate.RightDate != null)
                {
                    quarryList.Add("( birthdate " + FilterRange.GetSignLiteral(BirthDate.RightDateSign) + " \'" + BirthDate.RightDate?.ToString("yyyy-MM-dd") + "\') ");
                }
            }

            if (Age != null)
            {
                if (Age.LeftValue != null)
                {
                    quarryList.Add("( age(persons.birthdate) " + FilterRange.GetSignLiteral(Age.LeftSign) + " interval \'" + Age.LeftValue + "years\' ) ");
                }
                if (Age.RightValue != null)
                {
                    quarryList.Add("( age(persons.birthdate) " + FilterRange.GetSignLiteral(Age.RightSign) + " interval \'" + Age.RightValue + "years\' ) ");
                }
            }

            if (Sex != null)
            {
                if ((bool)Sex)
                {
                    quarryList.Add("( sex = true )");
                }
                else
                {
                    quarryList.Add("( sex = false )");
                }

            }

            if (workDegrees != null)
            {
                tempQuarry = "( ";
                tempQuarry += " degree_id = " + string.Join(" OR degree_id = ", workDegrees.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (workRanks != null)
            {
                tempQuarry = "( ";
                tempQuarry += " rank_id = " + string.Join(" OR rank_id = ", workRanks.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if(IsMainWorkPlace!= null)
            {
                if ((bool)IsMainWorkPlace)
                {
                    quarryList.Add("( is_main_work_place = true )");
                }
            }

            if(FirstNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " first_node_id = " + string.Join(" OR first_node_id = ", FirstNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (SecondNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " second_node_id = " + string.Join(" OR second_node_id = ", SecondNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (ThirdNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " third_node_id = " + string.Join(" OR third_node_id = ", ThirdNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (FourthNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " fourth_node_id = " + string.Join(" OR fourth_node_id = ", FourthNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (WorkCategories != null)
            {
                tempQuarry = "( ";
                tempQuarry += " wc.id = " + string.Join(" OR wc.id = ", WorkCategories.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (quarryList.Count > 0)
            {
                quarry += " WHERE " + string.Join(" AND ", quarryList);
            }
            quarry += " ORDER BY persons.id; ";

            Console.WriteLine(quarry);
            return quarry;
        }

    }
}
