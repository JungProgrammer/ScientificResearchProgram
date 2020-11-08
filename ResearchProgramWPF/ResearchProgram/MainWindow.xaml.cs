using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace ResearchProgram
{
    public class Grant
    {
        public Grant()
        {
            Depositor = new List<string>();
            DepositorSum = new List<string>();
            Executor = new List<string>();
            ResearchType = new List<string>();
            PriorityTrand = new List<string>();
            ExecutorContract = new List<string>();
            ScienceType = new List<string>();
        }

        // Id
        public int Id { get; set; }
        // ОКВЕД
        public string OKVED { get; set; }
        // Наименование НИОКР
        public string NameNIOKR { get; set; }
        // Заказчик
        public string Customer { get; set; }
        // Дата начала договора
        public DateTime StartDate { get; set; }
        // Дата окончания договора
        public DateTime EndDate { get; set; }
        // Цена договора
        public float Price { get; set; }
        // Средства
        public List<string> Depositor { get; set; }
        // Часть средств
        public List<string> DepositorSum { get; set; }
        // руководитель
        public string LeadNIOKR { get; set; }
        // Исполнитель
        public List<string> Executor { get; set; }
        // Кафедра
        public string Kafedra { get; set; }
        // Подразделение
        public string Unit { get; set; }
        // Учреждение
        public string Institution { get; set; }
        // ГРНТИ
        public string GRNTI { get; set; }
        // Тип исследования
        public List<string> ResearchType { get; set; }
        // Приоритетные направления
        public List<string> PriorityTrand { get; set; }
        // Исполнители по договору
        public List<string> ExecutorContract { get; set; }
        // Тип науки
        public List<string> ScienceType { get; set; }
        // НИР или услуга
        public string NIR { get; set; }
        // НОЦ
        public string NOC { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataTable GrantsDataTable { get; private set; }
        public DataTable PeopleDataTable { get; private set; }

        public MainWindow()
        {

            InitializeComponent();


            // Загружаем данные в таблицу грантов
            LoadGrantsTable();
            // Загружаем данные в таблицу людей
            LoadPeopleTable();

            DataContext = this;
        }

        /// <summary>
        /// Загрузка данных в таблицу договоров
        /// </summary>
        private void LoadGrantsTable()
        {
            var ds = new DataSet("Grants");
            this.GrantsDataTable = ds.Tables.Add("GrantsTable");

            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.CreateHeaders(GrantsDataTable);
            CRUDDataBase.LoadTable(GrantsDataTable);
            CRUDDataBase.CloseConnect();
        }

        /// <summary>
        /// Загрузка данных в таблицу людей
        /// </summary>
        private void LoadPeopleTable()
        {
            var ds = new DataSet("Grants");
            this.PeopleDataTable = ds.Tables.Add("PeopleTable");
            this.PeopleDataTable.Columns.Add("First");
            this.PeopleDataTable.Columns.Add("Second");

            this.PeopleDataTable.Rows.Add("11", "12");
            this.PeopleDataTable.Rows.Add("21", "22");
        }

    }



    public static class CRUDDataBase
    {
        public static string loginFromDB = Environment.GetEnvironmentVariable("PGUSER");
        public static string passwordFromDB = Environment.GetEnvironmentVariable("PGPASSWORD");

        private static NpgsqlConnection conn;

        public static void ConnectByDataBase()
        {
            conn = new NpgsqlConnection($"Server=localhost; Port=5432; User Id={loginFromDB}; Password={passwordFromDB}; Database=postgres");
            conn.Open();
        }

        public static void LoadTable(DataTable dataTable)
        {
            int grant_index = 0;
            int grant_id = 0;
            int countOfGrants = 0;
            // массив договоров
            Grant[] grants = null;

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, (SELECT COUNT(*) FROM grants) FROM grants;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                int i;
                reader.Read();
                countOfGrants = Convert.ToInt32(reader[1]);
                // Инициализация договоров
                grants = new Grant[countOfGrants];
                for (i = 0; i < countOfGrants; i++) grants[i] = new Grant();

                grant_id = Convert.ToInt32(reader[0]);
                grants[0].Id = grant_id;

                i = 1;
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grants[i].Id = grant_id;
                    i++;
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();
 

            // Получение типов исследования
            cmd = new NpgsqlCommand("SELECT grantId, rT.title FROM grantResearchType " +
                                    "JOIN researchTypes rT on grantResearchType.researchTypeId = rT.id " +
                                    "JOIN grants g on grantResearchType.grantId = g.id " +
                                    "ORDER BY grantId; ", conn);
            reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                string researchType;
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    researchType = reader[1].ToString();
                    grants[grant_index].ResearchType.Add(researchType);
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }


            reader.Close();



            // Получение приоритетных направлений
            cmd = new NpgsqlCommand("SELECT grantId, title FROM grantPriorityTrends " +
                                        "JOIN priorityTrends on grantPriorityTrends.priorityTrendsId = priorityTrends.id " +
                                        "JOIN grants on grantPriorityTrends.grantId = grants.id " +
                                        "ORDER BY grantId;", conn);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                string priorityTrend;
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    priorityTrend = reader[1].ToString();
                    grants[grant_index].PriorityTrand.Add(priorityTrend);
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();

            // Получение типов наук
            cmd = new NpgsqlCommand("SELECT grantId, title FROM grantScienceTypes " +
                                        "JOIN scienceTypes sT on grantScienceTypes.scienceTypesId = sT.id " +
                                        "JOIN grants g on grantScienceTypes.grantId = g.id " +
                                        "ORDER BY grantId; ", conn);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                string scienceType;
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    scienceType = reader[1].ToString();
                    grants[grant_index].ScienceType.Add(scienceType);
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();

            // Получение спонсоров
            cmd = new NpgsqlCommand("SELECT grantId, title, PartSum FROM grantDeposits " + 
                                        "JOIN depositors d on grantDeposits.sourceId = d.id " +
                                        "JOIN grants g on grantDeposits.grantId = g.id " +
                                        "ORDER BY grantId; ", conn);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                string grantDeposit;
                string grantDepositSum;
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    grantDeposit = reader[1].ToString();
                    grantDepositSum = reader[2].ToString();
                    grants[grant_index].Depositor.Add(grantDeposit);
                    grants[grant_index].DepositorSum.Add(grantDepositSum);
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();

            // Получение исполнителей
            cmd = new NpgsqlCommand("SELECT grantId, FIO, isExecutorContract FROM executors " +
                                        "JOIN persons p on executors.executorId = p.id " +
                                        "JOIN grants g on executors.grantId = g.id " +
                                        "ORDER BY grantId; ", conn);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                string FIO;
                bool isExecutorContract;
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    isExecutorContract = Convert.ToBoolean(reader[2]);

                    grant_index = ShowGrantIndex(grants, grant_id);

                    FIO = reader[1].ToString();
                    if (isExecutorContract)
                    {
                        grants[grant_index].ExecutorContract.Add(FIO);
                        //убрать__________________________________________________________________________
                        grants[grant_index].Executor.Add("Нет такого");
                    }
                    else
                    {
                        grants[grant_index].Executor.Add(FIO);
                        //убрать__________________________________________________________________________
                        grants[grant_index].ExecutorContract.Add("Нет такого");
                    }
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();

            // Получение остальных столбцов
            cmd = new NpgsqlCommand("SELECT grants.id, OKVED, nameNIOKR, p.FIO, startDate, endDate, price, p2.FIO, k.title, u.title, i.title, GRNTI, NIR, NOC FROM grants " +
                                                        "JOIN persons p on grants.customerId = p.id " +
                                                        "JOIN persons p2 on grants.leadNIOKRId = p2.id " +
                                                        "JOIN kafedras k on grants.kafedraId = k.id " +
                                                        "JOIN units u on grants.unitId = u.id " +
                                                        "JOIN institutions i on grants.institutionId = i.id; ", conn);
            reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    grants[grant_index].OKVED = reader[1].ToString();
                    grants[grant_index].NameNIOKR = reader[2].ToString();
                    grants[grant_index].Customer = reader[3].ToString();
                    grants[grant_index].StartDate = Convert.ToDateTime(reader[4]);
                    grants[grant_index].EndDate = Convert.ToDateTime(reader[5]);
                    grants[grant_index].Price = Convert.ToInt32(reader[6]);
                    grants[grant_index].LeadNIOKR = reader[7].ToString();
                    grants[grant_index].Kafedra = reader[8].ToString();
                    grants[grant_index].Unit = reader[9].ToString();
                    grants[grant_index].Institution = reader[10].ToString();
                    grants[grant_index].GRNTI = reader[11].ToString();
                    grants[grant_index].NIR = reader[12].ToString();
                    grants[grant_index].NOC = reader[13].ToString();
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();


            for(int i = 0; i < grants.Length; i++)
            {
                dataTable.Rows.Add((i + 1).ToString(),
                    grants[i].OKVED,
                    grants[i].NameNIOKR,
                    grants[i].Customer,
                    grants[i].StartDate,
                    grants[i].EndDate,
                    grants[i].Price,
                    grants[i].Depositor.ElementAt(0),
                    grants[i].DepositorSum.ElementAt(0),
                    grants[i].LeadNIOKR,
                    grants[i].Executor.ElementAt(0),
                    grants[i].Kafedra,
                    grants[i].Unit,
                    grants[i].Institution,
                    grants[i].GRNTI,
                    grants[i].ResearchType.ElementAt(0),
                    grants[i].ExecutorContract.ElementAt(0),
                    grants[i].ScienceType.ElementAt(0),
                    grants[i].NIR,
                    grants[i].NOC);
            }
        }

        /// <summary>
        /// Ищет индекс гранта в массиве по id гранта
        /// </summary>
        /// <param name="grants"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        static int ShowGrantIndex(Grant[] grants, int id)
        {
            int index = 0;
            for(int i = 0; i < grants.Length; i++)
            {
                if (grants[i].Id == id) index = i;
            }

            return index;
        }

        /// <summary>
        /// Создание заголовков для таблиц
        /// </summary>
        public static void CreateHeaders(DataTable dataTable)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, field_title FROM fieldslist ORDER BY id", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dataTable.Columns.Add(reader[1].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
        }

        public static void CloseConnect()
        {
            conn.Close();
        }
    }
}
