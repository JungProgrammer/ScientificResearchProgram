using Npgsql;
using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace ResearchProgram
{
    public class CRUDDataBase
    {
        //public static string loginFromDB = Environment.GetEnvironmentVariable("PGUSER");
        //public static string passwordFromDB = Environment.GetEnvironmentVariable("PGPASSWORD");

        public static string loginFromDB = "postgres";
        public static string passwordFromDB = "XeKhM9bQnRYah";

        // К какой базе подключаться при режиме сборки
#if DEBUG
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif

        private static NpgsqlConnection conn;


        /// <summary>
        /// Подключение к БД
        /// </summary>
        public static void ConnectToDataBase()
        {
            string Database;
            if (DEBUG)
            {
                Database = "test_db";
            }
            else
            {
                Database = "postgres";
            }
            conn = new NpgsqlConnection($"Server=212.192.88.14; Port=5432; User Id={loginFromDB}; Password={passwordFromDB}; Database={Database}");
            try
            {
                conn.Open();
            }
            catch (Npgsql.NpgsqlException)
            {
                MessageBox.Show("Нет подключения к серверу баз данных. Попробуйте позже.\nПрограмма будет закрыта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        public static NpgsqlConnection GetNewConnection()
        {
            NpgsqlConnection NewConnection;
            string Database;
            if (DEBUG)
            {
                Database = "test_db";
            }
            else
            {
                Database = "postgres";
            }
            NewConnection = new NpgsqlConnection($"Server=212.192.88.14; Port=5432; User Id={loginFromDB}; Password={passwordFromDB}; Database={Database}");
            try
            {
                NewConnection.Open();
            }
            catch (Npgsql.NpgsqlException)
            {
                MessageBox.Show("Нет подключения к серверу баз данных. Попробуйте позже.\nПрограмма будет закрыта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            return NewConnection;
        }

        /// <summary>
        /// Закрытия соединения с БД
        /// </summary>
        public static void CloseConnection()
        {
            conn.Close();
        }

        /// <summary>
        /// Получение id грантов, которые необходимо отобразить в таблице(тут учитываются примененные фильтры)
        /// </summary>
        /// <returns></returns>
        public static HashSet<int> GetGrantIds()
        {
            NpgsqlCommand cmd;
            NpgsqlDataReader reader;
            int grantId;

            //Множество из id грантов, которые необходимо получить(например в запросе без фильтров тут будут храниться все id. Но если будет активен фильтр, то id отфильтруются)
            HashSet<int> grantIds = new HashSet<int>();

            NpgsqlConnection connection = GetNewConnection();
            //Фильтры не активны
            cmd = new NpgsqlCommand("SELECT id FROM grants ORDER BY id;", connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grantId = Convert.ToInt32(reader["id"]);
                    grantIds.Add(grantId);
                }
            }
            else
            {
                //Грантов нет, возвращаем пустой список
                return grantIds;
            }
            reader.Close();


            if (GrantsFilters.IsActive())
            {
                //Фильтры активны
                HashSet<int> tempHash = new HashSet<int>();

                // Основные поля гранта
                {
                    cmd = new NpgsqlCommand(GrantsFilters.GetGrantQuarry(), connection);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            grantId = Convert.ToInt32(reader["id"]);
                            tempHash.Add(grantId);
                        }
                    }
                    reader.Close();
                    grantIds.IntersectWith(tempHash);
                    if (grantIds.Count == 0)
                    {
                        return grantIds;
                    }
                    reader.Close();
                    tempHash = new HashSet<int>();
                }
                //Типы исследований
                if (GrantsFilters.ResearchType != null)
                {
                    cmd = new NpgsqlCommand(GrantsFilters.GetResearchTypesQuarry(), connection);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            grantId = Convert.ToInt32(reader["grantId"]);
                            tempHash.Add(grantId);
                        }

                    }
                    reader.Close();
                    grantIds.IntersectWith(tempHash);
                    if (grantIds.Count == 0)
                    {
                        return grantIds;
                    }
                }

                //Приоритетные направления
                if (GrantsFilters.PriorityTrend != null)
                {
                    cmd = new NpgsqlCommand(GrantsFilters.GetPriorityTrendsQuarry(), connection);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            grantId = Convert.ToInt32(reader["grantId"]);
                            tempHash.Add(grantId);
                        }
                    }
                    reader.Close();
                    grantIds.IntersectWith(tempHash);
                    if (grantIds.Count == 0)
                    {
                        return grantIds;
                    }

                    tempHash = new HashSet<int>();
                }

                //Типы наук
                if (GrantsFilters.ScienceType != null)
                {
                    cmd = new NpgsqlCommand(GrantsFilters.GetScienceTypesQuarry(), connection);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            grantId = Convert.ToInt32(reader["grantId"]);
                            tempHash.Add(grantId);
                        }
                    }
                    reader.Close();
                    grantIds.IntersectWith(tempHash);
                    if (grantIds.Count == 0)
                    {
                        return grantIds;
                    }
                    tempHash = new HashSet<int>();
                }

                //Средства
                if (GrantsFilters.Depositors != null)
                {
                    for (int i = 0; i < GrantsFilters.Depositors.Count; i++)
                    {
                        cmd = new NpgsqlCommand(GrantsFilters.GetDepositorQuarryByIndex(i), connection);
                        reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                grantId = Convert.ToInt32(reader["grantId"]);
                                tempHash.Add(grantId);
                            }
                        }
                        reader.Close();
                        grantIds.IntersectWith(tempHash);
                        if (grantIds.Count == 0)
                        {
                            return grantIds;
                        }
                        reader.Close();
                        tempHash = new HashSet<int>();
                    }
                }
                // Заказчики
                if (GrantsFilters.Customer != null)
                {
                    cmd = new NpgsqlCommand(GrantsFilters.GetCustomersQuarry(), connection);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            grantId = Convert.ToInt32(reader["grant_id"]);
                            tempHash.Add(grantId);
                        }
                    }
                    reader.Close();
                    grantIds.IntersectWith(tempHash);
                    if (grantIds.Count == 0)
                    {
                        return grantIds;
                    }
                    reader.Close();
                    tempHash = new HashSet<int>();
                }

                // Исполнители
                if (GrantsFilters.Executor != null)
                {
                    cmd = new NpgsqlCommand(GrantsFilters.GetExecutorsQuarry(), connection);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            grantId = Convert.ToInt32(reader["grantId"]);
                            tempHash.Add(grantId);
                        }
                    }
                    reader.Close();
                    grantIds.IntersectWith(tempHash);
                    if (grantIds.Count == 0)
                    {
                        return grantIds;
                    }
                    reader.Close();
                    tempHash = new HashSet<int>();
                }
            }
            return grantIds;
        }

        public static Grant GetGrantById(int grantId)
        {
            Grant grant = new Grant();
            grant.Id = grantId;

            Console.WriteLine(grantId);
            NpgsqlConnection connection = GetNewConnection();

            // Получение типов исследования
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title, grantResearchType.researchTypeId rId FROM grantResearchType " +
                                    "JOIN researchTypes rT on grantResearchType.researchTypeId = rT.id " +
                                    "WHERE grantId = :grantId; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant.ResearchType.Add(new ResearchType()
                    {
                        Id = Convert.ToInt32(reader["rId"]),
                        Title = reader["title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение приоритетных направлений
            cmd = new NpgsqlCommand("SELECT title, grantPriorityTrends.priorityTrendsId pId FROM grantPriorityTrends " +
                                        "JOIN priorityTrends on grantPriorityTrends.priorityTrendsId = priorityTrends.id " +
                                    "WHERE grantId = :grantId; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant.PriorityTrands.Add(new PriorityTrend()
                    {
                        Id = Convert.ToInt32(reader["pId"]),
                        Title = reader["title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение типов наук
            cmd = new NpgsqlCommand("SELECT  title, grantScienceTypes.scienceTypesId sId FROM grantScienceTypes " +
                                        "JOIN scienceTypes sT on grantScienceTypes.scienceTypesId = sT.id " +
                                    "WHERE grantId = :grantId; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant.ScienceType.Add(new ScienceType()
                    {
                        Id = Convert.ToInt32(reader["sId"]),
                        Title = reader["title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение спонсоров
            cmd = new NpgsqlCommand("SELECT title, PartSum, receiptDate, PartSumNoNDS, grantDeposits.sourceId sId FROM grantDeposits " +
                                        "JOIN depositors d on grantDeposits.sourceId = d.id " +
                                    "WHERE grantId = :grantId; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                double grantDepositSum;
                double grantDepositSumNoNDS;
                string receiptDate;
                while (reader.Read())
                {
                    grantDepositSum = reader.GetDouble(1);
                    grantDepositSumNoNDS = reader.GetDouble(3);
                    receiptDate = reader["receiptDate"] != DBNull.Value ? DateTime.Parse(reader["receiptDate"].ToString()).ToShortDateString() : string.Empty;
                    grant.Depositor.Add(new Depositor()
                    {
                        Id = Convert.ToInt32(reader["sId"]),
                        Title = reader["title"].ToString(),
                    });
                    grant.DepositorSum.Add(grantDepositSum);
                    grant.DepositorSumNoNDS.Add(grantDepositSumNoNDS);
                    grant.ReceiptDate.Add(receiptDate);
                }
            }
            reader.Close();

            // Получение заказчиков
            cmd = new NpgsqlCommand("SELECT customer_id, title, short_title FROM grants_customers " +
                                        "JOIN customers ON customers.customerid = grants_customers.customer_id " +
                                                                            "WHERE grant_id = :grantId; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant.Customer.Add(new Customer()
                    {
                        Id = Convert.ToInt32(reader["customer_id"]),
                        Title = reader["title"].ToString(),
                        ShortTitle = reader["short_title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение исполнителей
            cmd = new NpgsqlCommand("SELECT  FIO, executorId FROM executors " +
                                    "JOIN persons p on executors.executorId = p.id " +
                                    "WHERE grantId = :grantId; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant.Executor.Add(new Person()
                    {
                        Id = Convert.ToInt32(reader["executorId"]),
                        FIO = reader["FIO"].ToString()
                    });
                }
            }

            reader.Close();

            // Получение остальных столбцов
            cmd = new NpgsqlCommand("SELECT grants.grantnumber as ggn, OKVED, nameNIOKR, startDate, endDate, price, p2.FIO as lead_niokr, p2.ID as lead_niokr_id, " +
                                    " GRNTI, NIR, NOC, pricenonds, is_with_nds, " +
                                    "first_node_id, second_node_id, third_node_id, fourth_node_id FROM grants " +
                                    "LEFT JOIN persons p2 on grants.leadNIOKRId = p2.id " +
                                    "WHERE grants.id = :grantId " +
                                    "ORDER BY grants.id; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                grant.grantNumber = reader["ggn"].ToString();
                grant.OKVED = reader["OKVED"].ToString();
                grant.NameNIOKR = reader["nameNIOKR"].ToString();
                grant.StartDate = reader["startDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["startDate"]) : null;
                grant.EndDate = reader["endDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["endDate"]) : null;
                grant.Price = reader.GetDouble(5);
                grant.PriceNoNDS = reader.GetDouble(11);
                grant.LeadNIOKR = reader["lead_niokr"] != DBNull.Value ? new Person() { FIO = reader["lead_niokr"].ToString(), Id = Convert.ToInt32(reader["lead_niokr_id"]) } : null;
                grant.GRNTI = reader["GRNTI"].ToString();
                grant.NIR = reader["NIR"].ToString();
                grant.NOC = reader["NOC"].ToString();
                grant.isWIthNDS = Convert.ToBoolean(reader["is_with_nds"]);
                grant.FirstNode = reader["first_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["first_node_id"])) : new UniversityStructureNode();
                grant.SecondNode = reader["second_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["second_node_id"])) : new UniversityStructureNode();
                grant.ThirdNode = reader["third_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["third_node_id"])) : new UniversityStructureNode();
                grant.FourthNode = reader["fourth_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["fourth_node_id"])) : new UniversityStructureNode();
            }
            reader.Close();

            connection.Close();
            return grant;

        }

        public static List<Grant> GetGrants()
        {
            List<Grant> grants = new List<Grant>();
            if (GrantsFilters.IsActive())
            {
                Console.WriteLine("FILTERS ACTIVE");
                List<int> grantIds = GetGrantIds().ToList();
                for (int i = 0; i < grantIds.Count; i++)
                {
                    grants.Add(GetGrantById(grantIds[i]));
                }
            }
            else
            {
                Console.WriteLine("FILTERS INACTIVE");
                grants = GetGrantsInBulk();
            }
            return grants;
        }
        public static List<Grant> GetGrantsInBulk()
        {
            List<Grant> grants = new List<Grant>();
            Dictionary<int, Grant> grantsDict = new Dictionary<int, Grant>();
            NpgsqlConnection connection = GetNewConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT grants.id as gid, grants.grantnumber as ggn, OKVED, nameNIOKR, startDate, endDate, price, p2.FIO as lead_niokr, p2.ID as lead_niokr_id, " +
                                    " GRNTI, NIR, NOC, pricenonds, is_with_nds, " +
                                    " first_node_id, second_node_id, third_node_id, fourth_node_id FROM grants " +
                                    " LEFT JOIN persons p2 on grants.leadNIOKRId = p2.id " +
                                    " ORDER BY grants.id; ", connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int grantId = reader.GetInt16(0);
                    grantsDict[grantId] = new Grant()
                    {
                        Id = grantId,
                        grantNumber = reader["ggn"].ToString(),
                        OKVED = reader["OKVED"].ToString(),
                        NameNIOKR = reader["nameNIOKR"].ToString(),
                        StartDate = reader["startDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["startDate"]) : null,
                        EndDate = reader["endDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["endDate"]) : null,
                        Price = reader.GetDouble(6),
                        PriceNoNDS = reader.GetDouble(12),
                        LeadNIOKR = reader["lead_niokr"] != DBNull.Value ? new Person() { FIO = reader["lead_niokr"].ToString(), Id = Convert.ToInt32(reader["lead_niokr_id"]) } : null,
                        GRNTI = reader["GRNTI"].ToString(),
                        NIR = reader["NIR"].ToString(),
                        NOC = reader["NOC"].ToString(),
                        isWIthNDS = Convert.ToBoolean(reader["is_with_nds"]),
                        FirstNode = reader["first_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["first_node_id"])) : new UniversityStructureNode(),
                        SecondNode = reader["second_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["second_node_id"])) : new UniversityStructureNode(),
                        ThirdNode = reader["third_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["third_node_id"])) : new UniversityStructureNode(),
                        FourthNode = reader["fourth_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["fourth_node_id"])) : new UniversityStructureNode()
                    };

                }
            }
            reader.Close();

            // Получение типов исследования
            cmd = new NpgsqlCommand("SELECT grantid, title, grantResearchType.researchTypeId rId FROM grantResearchType " +
                                    "JOIN researchTypes rT on grantResearchType.researchTypeId = rT.id; ", connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grantsDict[reader.GetInt16(0)].ResearchType.Add(new ResearchType()
                    {
                        Id = Convert.ToInt32(reader["rId"]),
                        Title = reader["title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение приоритетных направлений
            cmd = new NpgsqlCommand("SELECT grantid, title, grantPriorityTrends.priorityTrendsId pId FROM grantPriorityTrends " +
                                        "JOIN priorityTrends on grantPriorityTrends.priorityTrendsId = priorityTrends.id;", connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grantsDict[reader.GetInt16(0)].PriorityTrands.Add(new PriorityTrend()
                    {
                        Id = Convert.ToInt32(reader["pId"]),
                        Title = reader["title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение типов наук
            cmd = new NpgsqlCommand("SELECT grantid, title, grantScienceTypes.scienceTypesId sId FROM grantScienceTypes " +
                                        "JOIN scienceTypes sT on grantScienceTypes.scienceTypesId = sT.id;", connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grantsDict[reader.GetInt16(0)].ScienceType.Add(new ScienceType()
                    {
                        Id = Convert.ToInt32(reader["sId"]),
                        Title = reader["title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение спонсоров
            cmd = new NpgsqlCommand("SELECT grantid, title, PartSum, receiptDate, PartSumNoNDS, grantDeposits.sourceId sId FROM grantDeposits " +
                                        "JOIN depositors d on grantDeposits.sourceId = d.id;", connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                double grantDepositSum;
                double grantDepositSumNoNDS;
                string receiptDate;
                while (reader.Read())
                {
                    grantDepositSum = reader.GetDouble(2);
                    grantDepositSumNoNDS = reader.GetDouble(4);
                    receiptDate = reader["receiptDate"] != DBNull.Value ? DateTime.Parse(reader["receiptDate"].ToString()).ToShortDateString() : string.Empty;
                    grantsDict[reader.GetInt16(0)].Depositor.Add(new Depositor()
                    {
                        Id = Convert.ToInt32(reader["sId"]),
                        Title = reader["title"].ToString(),
                    });
                    grantsDict[reader.GetInt16(0)].DepositorSum.Add(grantDepositSum);
                    grantsDict[reader.GetInt16(0)].DepositorSumNoNDS.Add(grantDepositSumNoNDS);
                    grantsDict[reader.GetInt16(0)].ReceiptDate.Add(receiptDate);
                }
            }
            reader.Close();

            // Получение заказчиков
            cmd = new NpgsqlCommand("SELECT grant_id, customer_id, title, short_title FROM grants_customers " +
                                        "JOIN customers ON customers.customerid = grants_customers.customer_id;", connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grantsDict[reader.GetInt16(0)].Customer.Add(new Customer()
                    {
                        Id = Convert.ToInt32(reader["customer_id"]),
                        Title = reader["title"].ToString(),
                        ShortTitle = reader["short_title"].ToString()
                    });
                }
            }
            reader.Close();

            // Получение исполнителей
            cmd = new NpgsqlCommand("SELECT grantid, FIO, executorId FROM executors " +
                                    "JOIN persons p on executors.executorId = p.id;", connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grantsDict[reader.GetInt16(0)].Executor.Add(new Person()
                    {
                        Id = Convert.ToInt32(reader["executorId"]),
                        FIO = reader["FIO"].ToString()
                    });
                }
            }

            reader.Close();

            grants = grantsDict.Values.ToList();

            connection.Close();
            return grants;
        }


        /// <summary>
        /// Выгружает таблицу договоров на гланый экран
        /// </summary>
        /// <param name="dataTable"></param>
        public static void LoadGrantsTable(DataTable dataTable)
        {
            dataTable.Rows.Clear();

            List<Grant> grants = GetGrants();

            for (int i = 0; i < grants.Count; i++)
            {
                WorkerWithTablesOnMainForm.AddRowToGrantTable(dataTable, grants[i]);
            }
        }

        /// <summary>
        /// Выгружает таблицу людей
        /// </summary>
        /// <param name="dataTable"></param>
        public static void LoadPersonsTable(DataTable dataTable)
        {
            dataTable.Rows.Clear();
            List<Person> persons = GetPersons(true);
            for (int i = 0; i < persons.Count; i++)
            {
                WorkerWithTablesOnMainForm.AddRowToPersonsTable(dataTable, persons[i]);
            }
        }

        /// <summary>
        /// Загрузка всех должностей
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<Job> LoadJobs()
        {
            ObservableCollection<Job> jobs = new ObservableCollection<Job>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title, salary FROM jobs;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    jobs.Add(new Job()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString(),
                        Salary = float.Parse(reader[2].ToString())
                    });
                }
            }

            return jobs;
        }

        /// <summary>
        /// Загрузка из бд всех степеней
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<WorkDegree> LoadDegrees()
        {
            ObservableCollection<WorkDegree> degrees = new ObservableCollection<WorkDegree>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_degree;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    degrees.Add(new WorkDegree()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }

            return degrees;
        }

        /// <summary>
        /// Загрузка рангов
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<WorkRank> LoadRanks()
        {
            ObservableCollection<WorkRank> workRanks = new ObservableCollection<WorkRank>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_rank;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    workRanks.Add(new WorkRank()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }

            return workRanks;
        }

        public static ObservableCollection<WorkCategories> LoadCategories()
        {
            ObservableCollection<WorkCategories> workCategories = new ObservableCollection<WorkCategories>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_categories;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    workCategories.Add(new WorkCategories()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }

            return workCategories;
        }

        /// <summary>
        /// Добавление новой должности
        /// </summary>
        /// <param name="newJobName">Название новой должности</param>
        /// <param name="newSalary">Ставка</param>
        /// <returns></returns>
        public static Job AddJob(string newJobName, string newSalary)
        {
            newJobName = newJobName.Trim(' ');
            Job job = null;

            float salary = ResearchProgram.Parser.ConvertToRightFloat(newSalary);

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO jobs (title, salary) VALUES (:title, :salary);", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newJobName));
            cmd.Parameters.Add(new NpgsqlParameter("salary", salary));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM jobs ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                job = new Job()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = newJobName,
                    Salary = salary
                };
            }

            return job;
        }

        /// <summary>
        /// Добавление степени
        /// </summary>
        /// <param name="newDegreeName"></param>
        /// <returns></returns>
        public static WorkDegree AddDegree(string newDegreeName)
        {
            newDegreeName = newDegreeName.Trim(' ');
            WorkDegree workDegree = null;

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO work_degree (title) VALUES (:title);", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newDegreeName));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM work_degree ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                workDegree = new WorkDegree()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = newDegreeName
                };
            }

            return workDegree;
        }

        /// <summary>
        /// Добавление звания
        /// </summary>
        /// <param name="newRankName"></param>
        /// <returns></returns>
        public static WorkRank AddRank(string newRankName)
        {
            newRankName = newRankName.Trim(' ');
            WorkRank workRank = null;

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO work_rank (title) VALUES (:title);", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newRankName));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM work_rank ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                workRank = new WorkRank()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = newRankName
                };
            }

            return workRank;
        }

        /// <summary>
        /// Добавление категории
        /// </summary>
        /// <param name="newCategoryName"></param>
        /// <returns></returns>
        public static WorkCategories AddCategory(string newCategoryName)
        {
            newCategoryName = newCategoryName.Trim(' ');
            WorkCategories workCategory = null;

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO work_categories (title) VALUES (:title);", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newCategoryName));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM work_rank ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                workCategory = new WorkCategories()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = newCategoryName
                };
            }

            return workCategory;
        }

        /// <summary>
        /// Удаление должности
        /// </summary>
        /// <param name="job"></param>
        public static void DeleteJob(Job job)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM jobs WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", job.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление степени
        /// </summary>
        /// <param name="workDegree"></param>
        public static void DeleteDegree(WorkDegree workDegree)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM work_degree WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", workDegree.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление звания
        /// </summary>
        /// <param name="workRank"></param>
        public static void DeleteRank(WorkRank workRank)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM work_rank WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", workRank.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="workCategory"></param>
        public static void DeleteCategory(WorkCategories workCategory)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM work_categories WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", workCategory.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Изменение должности
        /// </summary>
        /// <param name="job"></param>
        public static void EditJob(Job job)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE jobs SET title = :title, salary = :salary WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", job.Id));
            cmd.Parameters.Add(new NpgsqlParameter("title", job.Title));
            cmd.Parameters.Add(new NpgsqlParameter("salary", job.Salary));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Изменение степени
        /// </summary>
        /// <param name="workDegree"></param>
        public static void EditDegree(WorkDegree workDegree)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE work_degree SET title = :title WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", workDegree.Id));
            cmd.Parameters.Add(new NpgsqlParameter("title", workDegree.Title));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Изменение звания
        /// </summary>
        /// <param name="workRank"></param>
        public static void EditRank(WorkRank workRank)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE work_rank SET title = :title WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", workRank.Id));
            cmd.Parameters.Add(new NpgsqlParameter("title", workRank.Title));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Изменение степени
        /// </summary>
        /// <param name="workCategory"></param>
        public static void EditCategory(WorkCategories workCategory)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE work_categories SET title = :title WHERE id = :id;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", workCategory.Id));
            cmd.Parameters.Add(new NpgsqlParameter("title", workCategory.Title));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Выгружает таблицу заказчиков
        /// </summary>
        /// <param name="dataTable"></param>
        public static void LoadCustomersTable(DataTable dataTable)
        {
            NpgsqlConnection connection = GetNewConnection();
            dataTable.Rows.Clear();

            // массив людей
            List<Customer> customers = new List<Customer>();

            // Получение остальных столбцов
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT customerid, title, short_title FROM customers ORDER BY customerid", connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Customer c = new Customer
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString(),
                        ShortTitle = reader[2].ToString()
                    };
                    customers.Add(c);
                }
            }
            reader.Close();
            connection.Close();
            for (int i = 0; i < customers.Count; i++)
            {
                WorkerWithTablesOnMainForm.AddRowToCustomersTable(dataTable, customers[i]);
            }
        }

        /// <summary>
        /// Получение списка людей
        /// </summary>
        /// <returns></returns>
        public static List<Person> GetPersons(bool is_jobs_needed = false)
        {
            List<Person> persons = new List<Person>();
            if (PersonsFilters.IsActive())
            {
                Console.WriteLine("PERSONS FILTERS ACTIVE");
                List<int> personsIds = GetPersonsIds().ToList();
                for (int i = 0; i < personsIds.Count; i++)
                {
                    persons.Add(GetPersonById(personsIds[i]));
                }
            }
            else
            {
                Console.WriteLine("PERSONS FILTERS INACTIVE");
                persons = GetPersonsInBulk();
            }

            return persons;
        }

        public static HashSet<int> GetPersonsIds()
        {

            return new HashSet<int>();
        }


        public static List<Person> GetPersonsInBulk()
        {
            Dictionary<int, Person> personsDict = new Dictionary<int, Person>();
            NpgsqlConnection connection = GetNewConnection();
            int personId;

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT persons.id as pid, fio, birthdate, sex, degree_id, wd.title wdt, rank_id, wr.title wrt FROM persons " +
                                                    "LEFT  JOIN work_degree wd ON persons.degree_id = wd.id " +
                                                    "LEFT JOIN work_rank wr on persons.rank_id = wr.id ", connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    personId = Convert.ToInt32(reader["pid"]);
                    personsDict[personId] = new Person()
                    {
                        Id = Convert.ToInt32(reader["pid"]),
                        FIO = reader["fio"].ToString(),
                        BitrhDate = reader["birthdate"] is DBNull ? null : (DateTime?)reader["birthdate"],
                        Sex = (bool)reader["sex"],
                        Degree = reader["degree_id"] != DBNull.Value ? new WorkDegree { Id = Convert.ToInt32(reader["degree_id"]), Title = reader["wdt"].ToString() } : null,
                        Rank = reader["rank_id"] != DBNull.Value ? new WorkRank { Id = Convert.ToInt32(reader["rank_id"]), Title = reader["wrt"].ToString() } : null
                    };
                }
            }

            reader.Close();
            cmd = new NpgsqlCommand("SELECT persons.id pid, persons_work_places.id pwpid, category_id cid, wc.title wct, is_main_work_place, first_node_id, second_node_id, third_node_id, fourth_node_id" +
                                    " FROM persons " +
                                    "JOIN persons_work_places ON persons.id = persons_work_places.person_id " +
                                    "LEFT JOIN work_categories wc on persons_work_places.category_id = wc.id ", connection);

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    personId = Convert.ToInt32(reader["pid"]);
                    PersonWorkPlace workPlace = new PersonWorkPlace();
                    workPlace.workCategory = new WorkCategories();

                    workPlace.Id = reader["pwpid"] != DBNull.Value ? Convert.ToInt32(reader["pwpid"]) : -1;
                    workPlace.workCategory.Id = reader["cid"] != DBNull.Value ? Convert.ToInt32(reader["cid"]) : -1;
                    workPlace.workCategory.Title = reader["wct"] != DBNull.Value ? reader["wct"].ToString() : "";
                    workPlace.IsMainWorkPlace = reader["is_main_work_place"] != DBNull.Value ? Convert.ToBoolean(reader["is_main_work_place"]) : false;


                    if (reader["first_node_id"] != DBNull.Value)
                    {
                        workPlace.firstNode = StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["first_node_id"]));
                    }
                    else
                    {
                        workPlace.firstNode = null;
                    }

                    if (reader["second_node_id"] != DBNull.Value)
                    {
                        workPlace.secondNode = StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["second_node_id"]));
                    }
                    else
                    {
                        workPlace.secondNode = null;
                    }

                    if (reader["third_node_id"] != DBNull.Value)
                    {
                        workPlace.thirdNode = StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["third_node_id"]));
                    }
                    else
                    {
                        workPlace.thirdNode = null;
                    }

                    if (reader["fourth_node_id"] != DBNull.Value)
                    {
                        workPlace.fourthNode = StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["fourth_node_id"]));
                    }
                    else
                    {
                        workPlace.fourthNode = null;
                    }

                    personsDict[personId].workPlaces.Add(workPlace);
                }
            }
            reader.Close();
            connection.Close();
            return personsDict.Values.ToList();

        }

        public static Person GetPersonById(int person_id, bool is_jobs_needed = false)
        {
            NpgsqlConnection connection = GetNewConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT fio, birthdate, sex, degree_id, wd.title wdt, rank_id, wr.title wrt FROM persons " +
                                                    "LEFT  JOIN work_degree wd ON persons.degree_id = wd.id " +
                                                    "LEFT JOIN work_rank wr on persons.rank_id = wr.id " +
                                                    "WHERE persons.id = :person_id; ", connection);
            cmd.Parameters.Add(new NpgsqlParameter(":person_id", person_id));

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Person newPerson = new Person();
            if (reader.HasRows)
            {
                reader.Read();

                newPerson = new Person()
                {
                    Id = person_id,
                    FIO = reader["fio"].ToString(),
                    BitrhDate = reader["birthdate"] is DBNull ? null : (DateTime?)reader["birthdate"],
                    Sex = (bool)reader["sex"],
                    Degree = reader["degree_id"] != DBNull.Value ? new WorkDegree { Id = Convert.ToInt32(reader["degree_id"]), Title = reader["wdt"].ToString() } : null,
                    Rank = reader["rank_id"] != DBNull.Value ? new WorkRank { Id = Convert.ToInt32(reader["rank_id"]), Title = reader["wrt"].ToString() } : null
                };
            }

            reader.Close();
            if (is_jobs_needed)
            {
                Dictionary<int, PersonWorkPlace> workPlaces = new Dictionary<int, PersonWorkPlace>();
                int workPlaceId;
                cmd = new NpgsqlCommand("SELECT persons_work_places.id pwpid, category_id cid, wc.title wct, is_main_work_place, first_node_id, second_node_id, third_node_id, fourth_node_id " +
                    " FROM persons " +
                    "JOIN persons_work_places ON persons.id = persons_work_places.person_id " +
                    "LEFT JOIN work_categories wc on persons_work_places.category_id = wc.id " +
                    "WHERE persons.id = :person_id", connection);
                cmd.Parameters.Add(new NpgsqlParameter("person_id", newPerson.Id));

                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        workPlaceId = Convert.ToInt32(reader["pwpid"]);
                        workPlaces[workPlaceId] = new PersonWorkPlace()
                        {
                            workCategory = new WorkCategories(),
                            Id = workPlaceId,
                            firstNode = reader["first_node_id"] != DBNull.Value ? StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["first_node_id"])) : null,
                            secondNode = reader["second_node_id"] != DBNull.Value ? StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["second_node_id"])) : null,
                            thirdNode = reader["third_node_id"] != DBNull.Value ? StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["third_node_id"])) : null,
                            fourthNode = reader["fourth_node_id"] != DBNull.Value ? StaticData.GetUniversityStructureNodeById(Convert.ToInt32(reader["fourth_node_id"])) : null
                        };

                        workPlaces[workPlaceId].workCategory.Id = reader["cid"] != DBNull.Value ? Convert.ToInt32(reader["cid"]) : -1;
                        workPlaces[workPlaceId].workCategory.Title = reader["wct"] != DBNull.Value ? reader["wct"].ToString() : "";
                        workPlaces[workPlaceId].IsMainWorkPlace = reader["is_main_work_place"] != DBNull.Value ? Convert.ToBoolean(reader["is_main_work_place"]) : false;
                    }
                }
                reader.Close();

                cmd = new NpgsqlCommand("SELECT persons_jobs.job_id jid, j.title jti, j.salary jsl, salary_rate, persons_work_places_id FROM persons_jobs " +
                    "LEFT JOIN jobs j on persons_jobs.job_id = j.id " +
                    "JOIN persons_work_places pwp on persons_jobs.persons_work_places_id = pwp.id " +
                    "JOIN persons p on pwp.person_id = p.id " +
                    "WHERE p.id = :persond_id; ", connection);
                cmd.Parameters.Add(new NpgsqlParameter("persond_id", person_id));
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        workPlaceId = Convert.ToInt32(reader["persons_work_places_id"]);
                        if (workPlaces[workPlaceId].jobList == null) workPlaces[workPlaceId].jobList = new List<Job>();

                        workPlaces[workPlaceId].jobList.Add(new Job
                        {
                            Id = Convert.ToInt32(reader["jid"]),
                            Title = reader["jti"].ToString(),
                            Salary = Convert.ToSingle(reader["jsl"]),
                            SalaryRate = reader["salary_rate"] == DBNull.Value ? 0 : Convert.ToSingle(reader["salary_rate"])
                        });
                    }
                }
                reader.Close();

                newPerson.workPlaces = workPlaces.Values.ToList();
            }
            connection.Close();
            return newPerson;
        }

        public static ObservableCollection<Customer> GetCustomers()
        {
            ObservableCollection<Customer> customersList = new ObservableCollection<Customer>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT customerid, title, short_title FROM customers ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    customersList.Add(new Customer()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString(),
                        ShortTitle = reader[2].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();

            return customersList;
        }

        /// <summary>
        /// Получить заказчиков в новом потоке
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<Customer> GetCustomersInNewThread()
        {
            NpgsqlConnection connection = GetNewConnection();

            ObservableCollection<Customer> customersList = new ObservableCollection<Customer>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT customerid, title, short_title FROM customers ORDER BY title;", connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    customersList.Add(new Customer()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString(),
                        ShortTitle = reader[2].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();

            connection.Close();

            return customersList;
        }

        /// <summary>
        /// Получение списка средств
        /// </summary>
        /// <returns></returns>
        public static List<Depositor> GetDeposits()
        {
            List<Depositor> depositsList = new List<Depositor>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM depositors ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    depositsList.Add(new Depositor()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return depositsList;
        }

        /// <summary>
        /// Получение приоритетных направлений
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<PriorityTrend> GetPriorityTrends()
        {
            ObservableCollection<PriorityTrend> priorityTrendsList = new ObservableCollection<PriorityTrend>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM prioritytrends ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    priorityTrendsList.Add(new PriorityTrend()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            reader.Close();
            return priorityTrendsList;
        }

        /// <summary>
        /// Получение списка типов исследования
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<ResearchType> GetResearchTypes()
        {
            ObservableCollection<ResearchType> researchTypesList = new ObservableCollection<ResearchType>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM researchTypes ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    researchTypesList.Add(new ResearchType()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return researchTypesList;
        }

        /// <summary>
        /// Получение списка типов науки
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<ScienceType> GetScienceTypes()
        {
            ObservableCollection<ScienceType> scienctTypeTypesList = new ObservableCollection<ScienceType>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM scienceTypes ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    scienctTypeTypesList.Add(new ScienceType()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return scienctTypeTypesList;
        }

        /// <summary>
        /// Загрузка в БД нового человека
        /// </summary>
        /// <param name="person"></param>
        public static Person InsertNewPersonToDB(Person person)
        {
            // Вставляем в БД нового человека
            NpgsqlCommand cmd = new NpgsqlCommand("insert into persons (" +
                "fio, " +
                "birthdate, " +
                "sex, " +
                "degree_id, " +
                "rank_id) " +
                "values(" +
                ":fio, " +
                ":birthdate, " +
                ":sex, " +
                ":degree_id, " +
                ":rank_id);", conn);
            cmd.Parameters.Add(new NpgsqlParameter("fio", person.FIO));
            if (person.BitrhDate == null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("birthdate", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("birthdate", person.BitrhDate));
            }
            cmd.Parameters.Add(new NpgsqlParameter("sex", person.Sex));
            if (person.Degree.Title != null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("degree_id", person.Degree.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("degree_id", DBNull.Value));

            }
            if (person.Rank.Title != null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("rank_id", person.Rank.Id));

            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("rank_id", DBNull.Value));
            }

            cmd.ExecuteNonQuery();


            // Ищем id человека, которого только что добавили
            cmd = new NpgsqlCommand("SELECT id FROM persons ORDER BY id DESC", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                person.Id = Convert.ToInt32(reader[0]);
            }
            reader.Close();

            InserttNewPlaceOfWork(person);

            return person;
        }

        /// <summary>
        /// Обновление номера гранта в бд
        /// </summary>
        public static void UpdateGrantNumber(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET grantnumber = :grantnumber WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantnumber", fixedGrant.grantNumber));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление ОКВЕДА в бд
        /// </summary>
        public static void UpdateOKVED(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET okved = :okved WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("okved", fixedGrant.OKVED));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление НИОКРа в бд
        /// </summary>
        public static void UpdateNameNIOKR(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET nameniokr = :nameniokr WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("nameniokr", fixedGrant.NameNIOKR));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление заказчиков в бд
        /// </summary>
        public static void UpdateCustomers(Grant fixedGrant)
        {
            // Сначала удаление прошлых приоритетных направлений
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM grants_customers WHERE grant_id = :grant_id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grant_id", fixedGrant.Id));
            cmd.ExecuteNonQuery();

            // Вставка новых источников средств
            CRUDDataBase.AddCustomers(fixedGrant, fixedGrant.Id);
        }

        /// <summary>
        /// Обновление даты начала в бд
        /// </summary>
        public static void UpdateStartDate(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET startdate = :startdate WHERE id = :id", conn);
            if (fixedGrant.StartDate == null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("startdate", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("startdate", fixedGrant.StartDate));
            }
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление даты окончания в бд
        /// </summary>
        public static void UpdateEndDate(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET enddate = :enddate WHERE id = :id", conn);
            if (fixedGrant.EndDate == null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("enddate", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("enddate", fixedGrant.EndDate));
            }
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление общей суммы в бд
        /// </summary>
        public static void UpdatePrice(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET price = :price WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("price", fixedGrant.Price));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }
        public static void UpdatePriceNoNDS(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET pricenonds = :price WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("price", fixedGrant.PriceNoNDS));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление источников средств в бд
        /// </summary>
        public static void UpdateDeposits(Grant fixedGrant)
        {
            // Сначала удаление прошлых источников средств
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM grantdeposits WHERE grantid = :grantid", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantid", fixedGrant.Id));
            cmd.ExecuteNonQuery();

            // Вставка новых источников средств
            CRUDDataBase.AddDeposits(fixedGrant, fixedGrant.Id);
        }

        /// <summary>
        /// Обновление руководителя проекта в бд
        /// </summary>
        public static void UpdateLeadNiokr(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET leadniokrid = :leadniokrid WHERE id = :id", conn);
            if (fixedGrant.LeadNIOKR != null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("leadniokrid", fixedGrant.LeadNIOKR.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("leadniokrid", DBNull.Value));
            }
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление исполнителей в бд
        /// </summary>
        public static void UpdateExecutors(Grant fixedGrant)
        {
            // Сначала удаление прошлых исполнителей
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM executors WHERE grantid = :grantid", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantid", fixedGrant.Id));
            cmd.ExecuteNonQuery();

            // Вставка новых источников средств
            CRUDDataBase.AddExecutors(fixedGrant, fixedGrant.Id);
        }

        public static void UpdateWorkPlace(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET first_node_id = :first_node_id, second_node_id = :second_node_id, third_node_id = :third_node_id, fourth_node_id = :fourth_node_id WHERE id = :id", conn);
            if (fixedGrant.FirstNode.Title != null && fixedGrant.FirstNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("first_node_id", fixedGrant.FirstNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("first_node_id", DBNull.Value));
            }

            if (fixedGrant.SecondNode.Title != null && fixedGrant.SecondNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("second_node_id", fixedGrant.SecondNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("second_node_id", DBNull.Value));
            }

            if (fixedGrant.ThirdNode.Title != null && fixedGrant.ThirdNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("third_node_id", fixedGrant.ThirdNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("third_node_id", DBNull.Value));
            }

            if (fixedGrant.FourthNode.Title != null && fixedGrant.FourthNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("fourth_node_id", fixedGrant.FourthNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("fourth_node_id", DBNull.Value));
            }
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление ГРНТИ в бд
        /// </summary>
        public static void UpdateGRNTI(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET grnti = :grnti WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grnti", fixedGrant.GRNTI));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление типа исследования в бд
        /// </summary>
        public static void UpdateResearchType(Grant fixedGrant)
        {
            if (fixedGrant.ResearchType.Count > 0)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM grantresearchtype WHERE grantid = :grantid", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", fixedGrant.Id));
                cmd.ExecuteNonQuery();

                cmd = new NpgsqlCommand("insert into grantresearchtype (" +
                "grantid, " +
                "researchtypeid) " +
                "values(" +
                ":grantid, " +
                ":researchtypeid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", fixedGrant.Id));
                cmd.Parameters.Add(new NpgsqlParameter("researchtypeid", fixedGrant.ResearchType[0].Id));
                cmd.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// Обновление приоритетных направлений в бд
        /// </summary>
        public static void UpdatePriorityTrends(Grant fixedGrant)
        {
            // Сначала удаление прошлых приоритетных направлений
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM grantprioritytrends WHERE grantid = :grantid", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantid", fixedGrant.Id));
            cmd.ExecuteNonQuery();

            // Вставка новых источников средств
            CRUDDataBase.AddPriorityTrends(fixedGrant, fixedGrant.Id);
        }

        /// <summary>
        /// Обновление типов науки в бд
        /// </summary>
        public static void UpdateScienceTypes(Grant fixedGrant)
        {
            // Сначала удаление прошлых типов науки
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM grantsciencetypes WHERE grantid = :grantid", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantid", fixedGrant.Id));
            cmd.ExecuteNonQuery();

            // Вставка новых источников средств
            CRUDDataBase.AddScienceTypes(fixedGrant, fixedGrant.Id);
        }

        /// <summary>
        /// Обновление НИР в бд
        /// </summary>
        public static void UpdateNIR(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET nir = :nir WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("nir", fixedGrant.NIR));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление НОЦ в бд
        /// </summary>
        public static void UpdateNOC(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET noc = :noc WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("noc", fixedGrant.NOC == "Да" ? true : false));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateIsWithNDS(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET is_with_nds = :is_with_nds WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("is_with_nds", fixedGrant.isWIthNDS));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateFIO(Person fixedPerson)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET fio = :fio WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("fio", fixedPerson.FIO));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedPerson.Id));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateBirthDate(Person fixedPerson)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET birthdate = :bd WHERE id = :id", conn);
            if (fixedPerson.BitrhDate == null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("bd", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("bd", fixedPerson.BitrhDate));
            }
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedPerson.Id));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateSex(Person fixedPerson)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET sex = :sex WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("sex", fixedPerson.Sex));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedPerson.Id));
            cmd.ExecuteNonQuery();
        }

        public static void InserttNewPlaceOfWork(Person person)
        {
            foreach (PersonWorkPlace placeOfWork in person.workPlaces)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("insert into persons_work_places (" +
                "person_id, " +
                "category_id," +
                "first_node_id, " +
                "second_node_id, " +
                "third_node_id, " +
                "fourth_node_id, " +
                "is_main_work_place) " +
                "values(" +
                ":person_id, " +
                ":category_id, " +
                ":first_node_id, " +
                ":second_node_id, " +
                ":third_node_id, " +
                ":fourth_node_id, " +
                ":is_main_work_place)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("person_id", person.Id));
                if (placeOfWork.workCategory.Title != null)
                {
                    cmd.Parameters.Add(new NpgsqlParameter("category_id", placeOfWork.workCategory.Id));
                }
                else
                {
                    cmd.Parameters.Add(new NpgsqlParameter("category_id", DBNull.Value));
                }

                if (placeOfWork.firstNode.Title != null)
                {
                    cmd.Parameters.Add(new NpgsqlParameter("first_node_id", placeOfWork.firstNode.Id));
                }
                else
                {
                    cmd.Parameters.Add(new NpgsqlParameter("first_node_id", DBNull.Value));
                }

                if (placeOfWork.secondNode.Title != null)
                {
                    cmd.Parameters.Add(new NpgsqlParameter("second_node_id", placeOfWork.secondNode.Id));
                }
                else
                {
                    cmd.Parameters.Add(new NpgsqlParameter("second_node_id", DBNull.Value));
                }

                if (placeOfWork.thirdNode.Title != null)
                {
                    cmd.Parameters.Add(new NpgsqlParameter("third_node_id", placeOfWork.thirdNode.Id));
                }
                else
                {
                    cmd.Parameters.Add(new NpgsqlParameter("third_node_id", DBNull.Value));
                }

                if (placeOfWork.fourthNode.Title != null)
                {
                    cmd.Parameters.Add(new NpgsqlParameter("fourth_node_id", placeOfWork.fourthNode.Id));
                }
                else
                {
                    cmd.Parameters.Add(new NpgsqlParameter("fourth_node_id", DBNull.Value));
                }

                cmd.Parameters.Add(new NpgsqlParameter("is_main_work_place", placeOfWork.IsMainWorkPlace));

                cmd.ExecuteNonQuery();

                int newWorkPlaceId = 0;
                // Ищем id места работы, которое только что добавили
                cmd = new NpgsqlCommand("SELECT id FROM persons_work_places ORDER BY id DESC", conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    newWorkPlaceId = Convert.ToInt32(reader[0]);
                }
                reader.Close();

                foreach (Job job in placeOfWork.jobList)
                {
                    cmd = new NpgsqlCommand("insert into persons_jobs (" +
                    "persons_work_places_id, " +
                    "job_id, " +
                    "salary_rate)" +
                    "values(" +
                    ":persons_work_places_id, " +
                    ":job_id, " +
                    ":salary_rate)", conn);
                    cmd.Parameters.Add(new NpgsqlParameter("persons_work_places_id", newWorkPlaceId));
                    cmd.Parameters.Add(new NpgsqlParameter("job_id", job.Id));
                    cmd.Parameters.Add(new NpgsqlParameter("salary_rate", job.SalaryRate));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdatePlaceOfWork(Person fixedPerson)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM persons_work_places WHERE person_id = :person_id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("person_id", fixedPerson.Id));
            cmd.ExecuteNonQuery();
            InserttNewPlaceOfWork(fixedPerson);
        }

        public static void UpdateDegree(Person fixedPerson)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET degree_id = :degree_id WHERE id = :id", conn);
            if (fixedPerson.Degree.Title != null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("degree_id", fixedPerson.Degree.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("degree_id", DBNull.Value));
            }
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedPerson.Id));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateRank(Person fixedPerson)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET rank_id = :rank_id WHERE id = :id", conn);
            if (fixedPerson.Rank.Title != null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("rank_id", fixedPerson.Rank.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("rank_id", DBNull.Value));
            }
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedPerson.Id));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateCustomer(Customer fixedCustomer)
        {
            ConnectToDataBase();
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE customers SET title = :title, short_title = :short_title WHERE customerid = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedCustomer.Id));
            cmd.Parameters.Add(new NpgsqlParameter("title", fixedCustomer.Title));
            cmd.Parameters.Add(new NpgsqlParameter("short_title", fixedCustomer.ShortTitle));
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        /// <summary>
        /// Загрузка в БД нового договора
        /// </summary>
        /// <param name="grant"></param>
        public static void InsertNewGrantToDB(Grant grant)
        {
            // Id договора, который будет создан
            int newGrantId;

            // Вставляем в бд новый договор
            NpgsqlCommand cmd = new NpgsqlCommand("insert into grants (" +
                "grantnumber, " +
                "okved, " +
                "nameniokr, " +
                "startdate, " +
                "enddate, " +
                "price, " +
                "pricenonds, " +
                "leadniokrid, " +
                "grnti, " +
                "nir, " +
                "noc," +
                "is_with_nds, " +
                "first_node_id, " +
                "second_node_id, " +
                "third_node_id, " +
                "fourth_node_id) " +
                "values(" +
                ":grantnumber, " +
                ":okved, " +
                ":nameniokr, " +
                ":startdate, " +
                ":enddate, " +
                ":price, " +
                ":priceNoNDS, " +
                ":leadniokrid, " +
                ":grnti, " +
                ":nir, " +
                ":noc," +
                ":is_with_nds, " +
                ":first_node_id, " +
                ":second_node_id, " +
                ":third_node_id, " +
                ":fourth_node_id) RETURNING Id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantnumber", grant.grantNumber));
            cmd.Parameters.Add(new NpgsqlParameter("okved", grant.OKVED));
            cmd.Parameters.Add(new NpgsqlParameter("nameniokr", grant.NameNIOKR));

            if (grant.StartDate == null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("startdate", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("startdate", grant.StartDate));
            }
            if (grant.EndDate == null)
            {
                cmd.Parameters.Add(new NpgsqlParameter("enddate", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("enddate", grant.EndDate));
            }

            if (grant.LeadNIOKR != null)
                cmd.Parameters.Add(new NpgsqlParameter("leadniokrid", grant.LeadNIOKR.Id));
            else
                cmd.Parameters.Add(new NpgsqlParameter("leadniokrid", DBNull.Value));

            cmd.Parameters.Add(new NpgsqlParameter("price", grant.Price));
            cmd.Parameters.Add(new NpgsqlParameter("priceNoNDS", grant.PriceNoNDS));
            cmd.Parameters.Add(new NpgsqlParameter("grnti", grant.GRNTI));
            cmd.Parameters.Add(new NpgsqlParameter("nir", grant.NIR));
            cmd.Parameters.Add(new NpgsqlParameter("noc", grant.NOC == "Да"));
            cmd.Parameters.Add(new NpgsqlParameter("is_with_nds", grant.isWIthNDS));

            if (grant.FirstNode.Title != null && grant.FirstNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("first_node_id", grant.FirstNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("first_node_id", DBNull.Value));
            }

            if (grant.SecondNode.Title != null && grant.SecondNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("second_node_id", grant.SecondNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("second_node_id", DBNull.Value));
            }

            if (grant.ThirdNode.Title != null && grant.ThirdNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("third_node_id", grant.ThirdNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("third_node_id", DBNull.Value));
            }

            if (grant.FourthNode.Title != null && grant.FourthNode.Title != "")
            {
                cmd.Parameters.Add(new NpgsqlParameter("fourth_node_id", grant.FourthNode.Id));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("fourth_node_id", DBNull.Value));
            }

            NpgsqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            newGrantId = Convert.ToInt32(reader[0]);
            reader.Close();


            // Вставляем заказчиков
            CRUDDataBase.AddCustomers(grant, newGrantId);

            // Вставляем исполнителей
            CRUDDataBase.AddExecutors(grant, newGrantId);

            // Добавление источников средств в БД
            CRUDDataBase.AddDeposits(grant, newGrantId);

            // Добавление типов исследования
            foreach (ResearchType rType in grant.ResearchType)
            {
                cmd = new NpgsqlCommand("insert into grantresearchtype (" +
                "grantid, " +
                "researchtypeid) " +
                "values(" +
                ":grantid, " +
                ":researchtypeid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("researchtypeid", rType.Id));
                cmd.ExecuteNonQuery();
            }

            // Добавление типов науки
            CRUDDataBase.AddScienceTypes(grant, newGrantId);

            // Добавление приоритетных направлений
            CRUDDataBase.AddPriorityTrends(grant, newGrantId);
        }

        /// <summary>
        /// Добавление заказчиков в бд
        /// </summary>
        /// <param name="grant"></param>
        /// <param name="grantId"></param>
        public static void AddCustomers(Grant grant, int grantId)
        {
            foreach (Customer customer in grant.Customer)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("insert into grants_customers (" +
                "grant_id, " +
                "customer_id)" +
                "values(" +
                ":grant_id, " +
                ":customer_id)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grant_id", grantId));
                cmd.Parameters.Add(new NpgsqlParameter("customer_id", customer.Id));
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Добавление исполнителей в бд
        /// </summary>
        /// <param name="grant"></param>
        /// <param name="grantId"></param>
        public static void AddExecutors(Grant grant, int grantId)
        {
            foreach (Person executor in grant.Executor)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("insert into executors (" +
                "grantid, " +
                "executorid)" +
                "values(" +
                ":grantid, " +
                ":executorid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", grantId));
                cmd.Parameters.Add(new NpgsqlParameter("executorid", executor.Id));
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Добавление источников средств в бд
        /// </summary>
        /// <param name="grant"></param>
        /// <param name="grantId"></param>
        public static void AddDeposits(Grant grant, int grantId)
        {
            for (int i = 0; i < grant.Depositor.Count(); i++)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("insert into grantdeposits (" +
                "grantid, " +
                "sourceid," +
                "partsum," +
                "partsumnonds," +
                "receiptDate) " +
                "values(" +
                ":grantid, " +
                ":sourceid, " +
                ":partsum," +
                ":partsumnonds," +
                ":receiptDate)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", grantId));
                cmd.Parameters.Add(new NpgsqlParameter("sourceid", grant.Depositor[i].Id));
                cmd.Parameters.Add(new NpgsqlParameter("partsum", grant.DepositorSum[i]));
                cmd.Parameters.Add(new NpgsqlParameter("partsumnonds", grant.DepositorSumNoNDS[i]));
                cmd.Parameters.Add(new NpgsqlParameter("receiptDate", grant.ReceiptDate[i] != string.Empty ? DateTime.Parse(grant.ReceiptDate[i]) : DateTime.MinValue));
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddPriorityTrends(Grant grant, int grantId)
        {
            foreach (PriorityTrend priorityTrend in grant.PriorityTrands)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("insert into grantprioritytrends (" +
                "grantid, " +
                "prioritytrendsid) " +
                "values(" +
                ":grantid, " +
                ":prioritytrendsid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", grantId));
                cmd.Parameters.Add(new NpgsqlParameter("prioritytrendsid", priorityTrend.Id));
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddScienceTypes(Grant grant, int grantId)
        {
            foreach (ScienceType sType in grant.ScienceType)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("insert into grantsciencetypes (" +
                "grantid, " +
                "sciencetypesid) " +
                "values(" +
                ":grantid, " +
                ":sciencetypesid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", grantId));
                cmd.Parameters.Add(new NpgsqlParameter("sciencetypesid", sType.Id));
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Получение списка должностей с их зарплатами из БД
        /// </summary>
        /// <returns></returns>
        public static List<Job> GetJobs()
        {
            List<Job> jobsList = new List<Job>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title, salary FROM jobs ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    jobsList.Add(new Job()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString(),
                        Salary = Convert.ToInt32(reader[2])
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return jobsList;
        }

        public static Person GetPersonByPersonId(string personId)
        {
            ConnectToDataBase();
            Person person = GetPersonById(Convert.ToInt32(personId), true);
            CloseConnection();
            return person;
        }

        public static Customer GetCustomerByCustomerId(string customerId)
        {
            ConnectToDataBase();
            ObservableCollection<Customer> customers = GetCustomers();
            CloseConnection();
            Customer customer = new Customer();
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].Id == Convert.ToInt32(customerId))
                {
                    customer = customers[i];
                    break;
                }
            }
            return customer;
        }

        public static void DeleteGrant(string grantNumber)
        {
            int grantId = -1;
            ConnectToDataBase();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id FROM grants WHERE grantNumber = :grantNumber;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantNumber", grantNumber));

            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                grantId = Convert.ToInt32(reader[0]);
            }
            reader.Close();

            cmd = new NpgsqlCommand("DELETE FROM grants WHERE grantNumber = :grantNumber", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantNumber", grantNumber));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM grantsciencetypes WHERE grantId = :grantId", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM grantprioritytrends WHERE grantId = :grantId", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM grantdeposits WHERE grantId = :grantId", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM executors WHERE grantId = :grantId", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM grants_customers WHERE grant_id = :grantId", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM grantresearchtype WHERE grantId = :grantId", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantId", grantId));
            cmd.ExecuteNonQuery();

            Console.WriteLine(grantNumber);
            CloseConnection();
        }

        public static void DeletePerson(int PersonId)
        {
            ConnectToDataBase();
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM executors WHERE executorId = :personId", conn);
            cmd.Parameters.Add(new NpgsqlParameter("personId", PersonId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM persons WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", PersonId));
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public static void DeleteCustomer(int customerId)
        {
            ConnectToDataBase();
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM customers WHERE customerid = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("id", customerId));
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public static void AddNewCustomer(Customer customer)
        {
            ConnectToDataBase();
            NpgsqlCommand cmd = new NpgsqlCommand("insert into customers (title, short_title) values(:title, :short_title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", customer.Title));
            cmd.Parameters.Add(new NpgsqlParameter("short_title", customer.ShortTitle));
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public static List<WorkCategories> GetWorkCategories()
        {
            List<WorkCategories> workCategories = new List<WorkCategories>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_categories ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    workCategories.Add(new WorkCategories()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return workCategories;
        }

        public static List<WorkDegree> GetWorkDegrees()
        {
            List<WorkDegree> workDegrees = new List<WorkDegree>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_degree ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    workDegrees.Add(new WorkDegree()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return workDegrees;
        }

        public static List<WorkDegree> GetWorkDegreesInNewThread()
        {
            NpgsqlConnection connection = GetNewConnection();

            List<WorkDegree> workDegrees = new List<WorkDegree>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_degree ORDER BY title;", connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    workDegrees.Add(new WorkDegree()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();

            connection.Close();
            return workDegrees;
        }

        public static List<WorkRank> GetWorkRanks()
        {
            List<WorkRank> workRanks = new List<WorkRank>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_rank ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    workRanks.Add(new WorkRank()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return workRanks;
        }

        public static List<WorkRank> GetWorkRanksInNewThreads()
        {
            NpgsqlConnection connection = GetNewConnection();

            List<WorkRank> workRanks = new List<WorkRank>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM work_rank ORDER BY title;", connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    workRanks.Add(new WorkRank()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Title = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();

            connection.Close();
            return workRanks;
        }

        public static ObservableCollection<UniversityStructureNode> GetStructureNodes(string regex)
        {
            NpgsqlConnection con = GetNewConnection();

            ObservableCollection<UniversityStructureNode> NodeList = new ObservableCollection<UniversityStructureNode>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, address, title, short_title FROM work_place_structure WHERE address ~ " + regex + " ORDER BY title;", con);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    NodeList.Add(new UniversityStructureNode()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Address = reader["address"].ToString(),
                        Title = reader["title"].ToString(),
                        ShortTitle = reader["short_title"].ToString()
                    });
                }
            }


            reader.Close();
            con.Close();
            return NodeList;
        }

        public static UniversityStructureNode GetStructNodeById(int StructNodeId)
        {
            UniversityStructureNode Node = new UniversityStructureNode();
            NpgsqlConnection connection = GetNewConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, address, title, short_title FROM work_place_structure WHERE id = :id;", connection);
            cmd.Parameters.Add(new NpgsqlParameter(":id", StructNodeId));
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Node.Id = StructNodeId;
                    Node.Address = reader[1].ToString();
                    Node.Title = reader[2].ToString();
                    Node.ShortTitle = reader[3].ToString();
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            connection.Close();
            return Node;
        }

        public static ObservableCollection<UniversityStructureNode> GetAllFirstNodesByPerson(Person person)
        {
            ObservableCollection<UniversityStructureNode> universityStructureNodes = new ObservableCollection<UniversityStructureNode>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT first_node_id, is_main_work_place, " +
                    "(SELECT title FROM work_place_structure WHERE id = first_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = first_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = first_node_id) " +
                    "FROM persons_work_places " +
                    "WHERE person_id = :person_id", conn);
            cmd.Parameters.Add(new NpgsqlParameter(":person_id", person.Id));
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    universityStructureNodes.Add(new UniversityStructureNode
                    {
                        Id = reader[0] != DBNull.Value ? Convert.ToInt32(reader[0]) : -1,
                        Title = reader[2] != DBNull.Value ? reader[2].ToString() : "",
                        Address = reader[3] != DBNull.Value ? reader[3].ToString() : "",
                        ShortTitle = reader[4] != DBNull.Value ? reader[4].ToString() : "",
                        IsMainWorkPlace = reader[1] != DBNull.Value ? Convert.ToBoolean(reader[1]) : false
                    });
                }
            }
            reader.Close();
            return universityStructureNodes;
        }
        public static ObservableCollection<UniversityStructureNode> GetAllSecondNodesByPerson(Person person)
        {
            ObservableCollection<UniversityStructureNode> universityStructureNodes = new ObservableCollection<UniversityStructureNode>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT second_node_id, is_main_work_place, " +
                    "(SELECT title FROM work_place_structure WHERE id = second_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = second_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = second_node_id) " +
                    "FROM persons_work_places " +
                    "WHERE person_id = :person_id", conn);
            cmd.Parameters.Add(new NpgsqlParameter(":person_id", person.Id));
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    universityStructureNodes.Add(new UniversityStructureNode
                    {
                        Id = reader[0] != DBNull.Value ? Convert.ToInt32(reader[0]) : -1,
                        Title = reader[2] != DBNull.Value ? reader[2].ToString() : "",
                        Address = reader[3] != DBNull.Value ? reader[3].ToString() : "",
                        ShortTitle = reader[4] != DBNull.Value ? reader[4].ToString() : "",
                        IsMainWorkPlace = reader[1] != DBNull.Value ? Convert.ToBoolean(reader[1]) : false
                    });
                }
            }
            reader.Close();
            return universityStructureNodes;
        }
        public static ObservableCollection<UniversityStructureNode> GetAllThirdNodesByPerson(Person person)
        {
            ObservableCollection<UniversityStructureNode> universityStructureNodes = new ObservableCollection<UniversityStructureNode>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT third_node_id, is_main_work_place, " +
                    "(SELECT title FROM work_place_structure WHERE id = third_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = third_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = third_node_id) " +
                    "FROM persons_work_places " +
                    "WHERE person_id = :person_id", conn);
            cmd.Parameters.Add(new NpgsqlParameter(":person_id", person.Id));
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    universityStructureNodes.Add(new UniversityStructureNode
                    {
                        Id = reader[0] != DBNull.Value ? Convert.ToInt32(reader[0]) : -1,
                        Title = reader[2] != DBNull.Value ? reader[2].ToString() : "",
                        Address = reader[3] != DBNull.Value ? reader[3].ToString() : "",
                        ShortTitle = reader[4] != DBNull.Value ? reader[4].ToString() : "",
                        IsMainWorkPlace = reader[1] != DBNull.Value ? Convert.ToBoolean(reader[1]) : false
                    });
                }
            }
            reader.Close();
            return universityStructureNodes;
        }
        public static ObservableCollection<UniversityStructureNode> GetAllFourthNodesByPerson(Person person)
        {
            ObservableCollection<UniversityStructureNode> universityStructureNodes = new ObservableCollection<UniversityStructureNode>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT fourth_node_id, is_main_work_place, " +
                    "(SELECT title FROM work_place_structure WHERE id = fourth_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = fourth_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = fourth_node_id) " +
                    "FROM persons_work_places " +
                    "WHERE person_id = :person_id", conn);
            cmd.Parameters.Add(new NpgsqlParameter(":person_id", person.Id));
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    universityStructureNodes.Add(new UniversityStructureNode
                    {
                        Id = reader[0] != DBNull.Value ? Convert.ToInt32(reader[0]) : -1,
                        Title = reader[2] != DBNull.Value ? reader[2].ToString() : "",
                        Address = reader[3] != DBNull.Value ? reader[3].ToString() : "",
                        ShortTitle = reader[4] != DBNull.Value ? reader[4].ToString() : "",
                        IsMainWorkPlace = reader[1] != DBNull.Value ? Convert.ToBoolean(reader[1]) : false
                    });
                }
            }
            reader.Close();
            return universityStructureNodes;
        }

        public static void AddNewUniversityStructureNode(TreeNode ParentNode, string NewTitle, string NewShortTitle)
        {
            NpgsqlConnection connection = GetNewConnection();
            NpgsqlCommand cmd;
            NpgsqlDataReader reader;
            for (int i = 1; i < 1000000; i++)
            {
                cmd = new NpgsqlCommand("SELECT id, title, address FROM  work_place_structure WHERE address = :address", connection);
                cmd.Parameters.Add(new NpgsqlParameter("address", ParentNode.Address + "." + i.ToString()));
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    continue;
                }
                else
                {
                    reader.Close();
                    cmd = new NpgsqlCommand("INSERT INTO work_place_structure(title,short_title, address) VALUES(:title, :short_title, :address);", connection);
                    cmd.Parameters.Add(new NpgsqlParameter("title", NewTitle));
                    cmd.Parameters.Add(new NpgsqlParameter("short_title", NewShortTitle));
                    cmd.Parameters.Add(new NpgsqlParameter("address", ParentNode.Address + "." + i.ToString()));
                    cmd.ExecuteNonQuery();
                    break;
                }
            }
            connection.Close();
        }

        public static void DeleteUniversityStructureNode(TreeNode treeNode)
        {
            NpgsqlConnection connection = GetNewConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM work_place_structure WHERE id = :id", connection);
            cmd.Parameters.Add(new NpgsqlParameter("id", treeNode.Id));
            cmd.ExecuteNonQuery();
            cmd = new NpgsqlCommand("DELETE FROM work_place_structure WHERE address ~'^" + treeNode.Address + "\\.'", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public static void EditUniversityStructureNode(TreeNode treeNode, string NewTitle, string NewShortTitle)
        {
            NpgsqlConnection connection = GetNewConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE work_place_structure SET title = :title, short_title = :short_title WHERE id = :id", connection);
            cmd.Parameters.Add(new NpgsqlParameter("id", treeNode.Id));
            cmd.Parameters.Add(new NpgsqlParameter("title", NewTitle));
            cmd.Parameters.Add(new NpgsqlParameter("short_title", NewShortTitle));
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public static string GetActualVersion()
        {
            NpgsqlConnection connection = GetNewConnection();
            string Version = "";
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT value FROM config WHERE title='version';", connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                Version = reader["value"].ToString();
            }

            reader.Close();
            connection.Close();

            return Version;
        }

        public static bool IsPersonAlreadyExists(Person person)
        {
            bool isPersonExists = false;
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT FIO, birthdate FROM persons;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read() && !isPersonExists)
                {
                    if (reader["FIO"].ToString().Trim(' ') == person.FIO.Trim(' ') && Convert.ToDateTime(reader["birthdate"].ToString()) == person.BitrhDate)
                    {
                        isPersonExists = true;
                    }
                }
            }
            reader.Close();

            return isPersonExists;
        }

        public static bool IsGrantAlreadyExists(Grant grant)
        {
            bool isGrantExists = false;
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT grantnumber, nameniokr FROM grants;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read() && !isGrantExists)
                {
                    if (reader["grantnumber"].ToString().Trim(' ') == grant.grantNumber.Trim(' ') && reader["nameniokr"].ToString().Trim(' ') == grant.NameNIOKR.Trim(' '))
                    {
                        isGrantExists = true;
                    }
                }
            }
            reader.Close();

            return isGrantExists;
        }

        public static bool IsRankAlreadyExists(string rankName)
        {
            bool isRankExists = false;
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM work_rank;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read() && !isRankExists)
                {
                    if (reader["title"].ToString().Trim(' ') == rankName.Trim(' '))
                    {
                        isRankExists = true;
                    }
                }
            }
            reader.Close();

            return isRankExists;
        }

        public static bool IsJobAlreadyExists(string jobName)
        {
            bool isJobExists = false;
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM jobs;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read() && !isJobExists)
                {
                    if (reader["title"].ToString().Trim(' ') == jobName.Trim(' '))
                    {
                        isJobExists = true;
                    }
                }
            }
            reader.Close();

            return isJobExists;
        }

        public static bool IsDegreeAlreadyExists(string degreeName)
        {
            bool isDegreeExists = false;
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM work_degree;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read() && !isDegreeExists)
                {
                    if (reader["title"].ToString().Trim(' ') == degreeName.Trim(' '))
                    {
                        isDegreeExists = true;
                    }
                }
            }
            reader.Close();

            return isDegreeExists;
        }

        public static bool IsCategoryAlreadyExists(string categoryName)
        {
            bool isCategoryExists = false;
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM work_categories;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read() && !isCategoryExists)
                {
                    if (reader["title"].ToString().Trim(' ') == categoryName.Trim(' '))
                    {
                        isCategoryExists = true;
                    }
                }
            }
            reader.Close();

            return isCategoryExists;
        }

        public static void GetDepositsVerbose()
        {
            NpgsqlConnection connection = GetNewConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title, verbose_name FROM depositors;", connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    StaticData.DepositsVerbose.Add(reader["verbose_name"].ToString(), reader["title"].ToString());
                }
            }
            reader.Close();
            connection.Close();
        }
    }
}
