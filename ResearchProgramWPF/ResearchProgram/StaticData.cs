using ResearchProgram.Classes;
using System;
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
        public static Guid universityStructureGUID;

        public static List<Person> persons;
        public static Dictionary<int, Person> persondsDict = new Dictionary<int, Person>();
        public static Guid personsGUID;

        public static List<Grant> grants;
        public static Dictionary<int, Grant> grantsDict = new Dictionary<int, Grant>();
        public static Guid grantsGUID;

        public static List<Customer> customers;
        public static Dictionary<int, Customer> customersDict = new Dictionary<int, Customer>();
        public static Guid customersGUID;

        public static List<Depositor> deposits;
        //public static Dictionary<int, Depositor> depositsDict = new Dictionary<int, Depositor>();
        public static Guid depositsGUID;

        public static List<ResearchType> researchTypes;
        //public static Dictionary<int, ResearchType> researchTypesDict = new Dictionary<int, ResearchType>();
        public static Guid researchTypesGUID;

        public static List<ScienceType> scienceTypes;
        //public static Dictionary<int, ScienceType> scienceTypesDict = new Dictionary<int, ScienceType>();
        public static Guid scienceTypesGUID;

        public static List<PriorityTrend> priorityTrends;
        //public static Dictionary<int, PriorityTrend> priorityTrendsDict = new Dictionary<int, PriorityTrend>();
        public static Guid priorityTrendsGUID;



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
            if (universityStructureNodes == null || universityStructureGUID != CRUDDataBase.GetTableUUID("work_place_structure", out universityStructureGUID)) 
                LoadUniversityStructNodesStatic();
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
            if (universityStructureNodes == null || universityStructureGUID != CRUDDataBase.GetTableUUID("work_place_structure", out universityStructureGUID)) LoadUniversityStructNodesStatic();

            return new ObservableCollection<UniversityStructureNode>(universityStructureNodes.FindAll(x => Regex.IsMatch(x.Address, regex)));
        }

        /// <summary>
        /// Загружает в память данные о людях
        /// </summary>
        public static void LoadPersons()
        {
            //TODO Триггер в БД
            persons = CRUDDataBase.GetPersonsInBulk();
            persondsDict = persons.ToDictionary(x => x.Id, x => x);
        }

        /// <summary>
        /// Получение человека по его Id
        /// </summary>
        public static Person GetPersonById(int Id)
        {
            if (persons == null || personsGUID != CRUDDataBase.GetTableUUID("persons", out personsGUID)) LoadPersons();
            if (persondsDict.TryGetValue(Id, out Person p))
                return p;
            else
                return null;
        }

        public static List<Person> GetAllPersons()
        {
            if (persons == null || personsGUID != CRUDDataBase.GetTableUUID("persons", out personsGUID)) LoadPersons();

            return persons;
        }

        public static void LoadGrants()
        {
            //TODO Триггер в БД
            grants = CRUDDataBase.GetGrantsInBulk();
            grantsDict = grants.ToDictionary(x => x.Id, x => x);
        }

        public static Grant GetGrantById(int Id)
        {
            if (grants == null || grantsGUID != CRUDDataBase.GetTableUUID("grants", out grantsGUID)) LoadGrants();

            if (grantsDict.TryGetValue(Id, out Grant g))
                return g;
            else
                return null;
        }

        public static List<Grant> GetAllGrants()
        {
            
            if (grants == null || grantsGUID != CRUDDataBase.GetTableUUID("grants", out grantsGUID)) LoadGrants();

            return grants;
        }

        public static void LoadCustomers()
        {
            customers = CRUDDataBase.GetCustomers();
            customersDict = customers.ToDictionary(x => x.Id, x => x);
        }

        public static Customer GetCustomerById(int Id)
        {
            if (customers == null || customersGUID != CRUDDataBase.GetTableUUID("customers", out customersGUID)) LoadCustomers();
            if (customersDict.TryGetValue(Id, out Customer c))
                return c;
            else
                return null;
        }

        public static List<Customer> GetAllCustomers()
        {
            if (customers == null || customersGUID != CRUDDataBase.GetTableUUID("customers", out customersGUID)) LoadCustomers();

            return customers;
        }

        public static void LoadDeposits()
        {
            deposits = CRUDDataBase.GetDeposits();
            //depositsDict = deposits.ToDictionary(x => x.Id, x => x);
        }

        public static List<Depositor> GetAllDeposits()
        {
            if (deposits == null || depositsGUID != CRUDDataBase.GetTableUUID("depositors", out depositsGUID)) LoadDeposits();

            return deposits;
        }

        public static void LoadResearchTypes()
        {
            researchTypes = CRUDDataBase.GetResearchTypes();
            //researchTypesDict = researchTypes.ToDictionary(x => x.Id, x => x);
        }

        public static List<ResearchType> GetAllResearchTypes()
        {
            if (researchTypes == null || researchTypesGUID != CRUDDataBase.GetTableUUID("researchtypes", out researchTypesGUID)) LoadResearchTypes();

            return researchTypes;
        }

        public static void LoadScienceTypes()
        {
            scienceTypes = CRUDDataBase.GetScienceTypes();
            //scienceTypesDict = scienceTypes.ToDictionary(x => x.Id, x => x);
        }

        public static List<ScienceType> GetAllScienceTypes()
        {
            if (scienceTypes == null || scienceTypesGUID != CRUDDataBase.GetTableUUID("sciencetypes", out scienceTypesGUID)) LoadScienceTypes();

            return scienceTypes;
        }

        public static void LoadPriorityTrends()
        {
            priorityTrends = CRUDDataBase.GetPriorityTrends();
            //priorityTrendsDict = priorityTrends.ToDictionary(x => x.Id, x => x);
        }

        public static List<PriorityTrend> GetAllPriorityTrends()
        {
            if (priorityTrends == null || priorityTrendsGUID != CRUDDataBase.GetTableUUID("prioritytrends", out priorityTrendsGUID)) LoadPriorityTrends();

            return priorityTrends;
        }
    }
}
