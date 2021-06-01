using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResearchProgram
{
    static class StaticData
    {
        //Хранение названий средств и их "номера"
        public static Dictionary<string, string> DepositsVerbose = new Dictionary<string, string>();

        public static ObservableCollection<UniversityStructureNode> universityStructureNodes = new ObservableCollection<UniversityStructureNode>();
        public static Dictionary<int, UniversityStructureNode> universityStructureNodesDict = new Dictionary<int, UniversityStructureNode>();

        /// <summary>
        /// Загружает в память данные о структуре универа в виде списка и в виде словаря
        /// </summary>
        public static void LoadUniversityStructNodesStatic()
        {
            universityStructureNodes = CRUDDataBase.GetStructureNodes("''");
            universityStructureNodesDict = universityStructureNodes.ToDictionary(x => x.Id, x => x);
        }

        /// <summary>
        /// Поиск в словаре структуры универа по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static UniversityStructureNode GetUniversityStructureNodeById(int Id)
        {
            if (universityStructureNodesDict.ContainsKey(Id))
                return universityStructureNodesDict[Id];
            else
                return null;
        }

        public static ObservableCollection<UniversityStructureNode> GetUniversityStructureNodeByRegex(string regex)
        {
            //TODO Триггер в БД
            return  new ObservableCollection<UniversityStructureNode>(universityStructureNodes.ToList().FindAll(x => Regex.IsMatch(x.Address, regex)));
        }


    }
}
