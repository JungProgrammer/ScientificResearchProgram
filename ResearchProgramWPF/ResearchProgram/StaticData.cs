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

        public static List<UniversityStructureNode> universityStructureNodes;
        public static Dictionary<int, UniversityStructureNode> universityStructureNodesDict = new Dictionary<int, UniversityStructureNode>();

        public static List<Person> persons;
        public static Dictionary<int, Person> persondsDict = new Dictionary<int, Person>();

        public static List<Grant> grants;
        public static Dictionary<int, Grant> grantsDict = new Dictionary<int, Grant>();

        public static List<Customer> customers;
        public static Dictionary<int, Customer> customersDict = new Dictionary<int, Customer>();

        public static List<Depositor> deposits;
        public static Dictionary<int, Depositor> depositsDict = new Dictionary<int, Depositor>();



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
            if (universityStructureNodesDict.TryGetValue(Id, out UniversityStructureNode u))
                return u;
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
            if (persondsDict.TryGetValue(Id, out Person p))
                return p;
            else
                return null;
        }

        public static List<Person> GetAllPersons()
        {
            if (persons == null) LoadPersons();

            return persons;
        }

        public static void LoadGrants()
        {
            //TODO Триггер в БД
            grants = CRUDDataBase.GetGrants();
            grantsDict = grants.ToDictionary(x => x.Id, x => x);
        }

        public static Grant GetGrantById(int Id)
        {
            if (grants == null) LoadGrants();

            if (grantsDict.TryGetValue(Id, out Grant g))
                return g;
            else
                return null;
        }

        public static List<Grant> GetAllGrants()
        {
            if (grants == null) LoadGrants();

            return grants;
        }

        public static void LoadCustomers()
        {
            customers = CRUDDataBase.GetCustomers();
            customersDict = customers.ToDictionary(x => x.Id, x => x);
        }

        public static Customer GetCustomerById(int Id)
        {
            if (customers == null) LoadCustomers();
            if (customersDict.TryGetValue(Id, out Customer c))
                return c;
            else
                return null;
        }

        public static List<Customer> GetAllCustomers()
        {
            if (customers == null) LoadCustomers();

            return customers;
        }

        public static void LoadDeposits()
        {
            deposits = CRUDDataBase.GetDeposits();
            depositsDict = deposits.ToDictionary(x => x.Id, x => x);
        }

        public static List<Depositor> GetAllDeposits()
        {
            if (deposits == null) LoadDeposits();

            return deposits;
        }
    }
}
