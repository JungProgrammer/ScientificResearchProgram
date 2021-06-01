using ResearchProgram.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace ResearchProgram
{
    static class StaticData
    {
        //Хранение названий средств и их "номера"
        public static Dictionary<string, string> depositsVerbose = new Dictionary<string, string>();

        public static List<UniversityStructureNode> universityStructureNodes = new List<UniversityStructureNode>();
        public static Dictionary<int, UniversityStructureNode> universityStructureNodesDict = new Dictionary<int, UniversityStructureNode>();

        public static List<Person> persons = new List<Person>();
        public static Dictionary<int, Person> persondsDict = new Dictionary<int, Person>();

        /// <summary>
        /// Загружает в память данные о структуре универа в виде списка и в виде словаря
        /// </summary>
        public static void LoadUniversityStructNodesStatic()
        {
            //TODO Триггер в БД
            universityStructureNodes = CRUDDataBase.GetStructureNodes("''").ToList();
            universityStructureNodesDict = universityStructureNodes.ToDictionary(x => x.Id, x => x);
        }

        /// <summary>
        /// Поиск в словаре структуры универа по Id
        /// </summary>
        public static UniversityStructureNode GetUniversityStructureNodeById(int Id)
        {
            if (universityStructureNodes == null) LoadUniversityStructNodesStatic();

            if (universityStructureNodesDict.ContainsKey(Id))
                return universityStructureNodesDict[Id];
            else
                return null;
        }

        /// <summary>
        /// Получение узлов структуры универа по регулярному выражению
        /// </summary>
        public static ObservableCollection<UniversityStructureNode> GetUniversityStructureNodeByRegex(string regex)
        {
            if (universityStructureNodes == null) LoadUniversityStructNodesStatic();

            return new ObservableCollection<UniversityStructureNode>(universityStructureNodes.FindAll(x => Regex.IsMatch(x.Address, regex)));
        }

        /// <summary>
        /// Загружает в память данные о людях
        /// </summary>
        public static void LoadPersons()
        {
            //TODO Триггер в БД
            persons = CRUDDataBase.GetPersons();
            persondsDict = persons.ToDictionary(x => x.Id, x => x);
        }

        /// <summary>
        /// Получение человека по его Id
        /// </summary>
        public static Person GetPersonById(int Id)
        {
            if (persons == null) LoadPersons();

            if (persondsDict.ContainsKey(Id))
                return persondsDict[Id];
            else
                return null;
        }

        public static List<Person> GetAllPersons()
        {
            if (persons == null) LoadPersons();

            return persons;
        }
    }
}
