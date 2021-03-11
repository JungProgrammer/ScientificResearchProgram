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


        public static Grant[] GetAllGrants()
        {
            int grant_index;
            int grant_id;
            int countOfGrants;
            // массив договоров
            Grant[] grants = null;
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, (SELECT COUNT(*) FROM grants) FROM grants ORDER BY id;", conn);
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
                //Грантов нет, возвращаем пустой массив
                return new Grant[0];
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
                    grants[grant_index].ResearchType.Add(new ResearchType()
                    {
                        Title = researchType
                    });
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
                    grants[grant_index].PriorityTrands.Add(new PriorityTrend()
                    {
                        Title = priorityTrend
                    });
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
                    grants[grant_index].ScienceType.Add(new ScienceType()
                    {
                        Title = scienceType
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();

            // Получение спонсоров
            cmd = new NpgsqlCommand("SELECT grantId, title, PartSum, receiptDate, PartSumNoNDS FROM grantDeposits " +
                                        "JOIN depositors d on grantDeposits.sourceId = d.id " +
                                        "JOIN grants g on grantDeposits.grantId = g.id " +
                                        "ORDER BY grantId, sourceid; ", conn);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                string grantDeposit;
                string grantDepositSum;
                string grantDepositSumNoNDS;
                string receiptDate;
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    grantDeposit = reader[1].ToString();
                    grantDepositSum = reader[2].ToString();
                    grantDepositSumNoNDS = reader[4].ToString();
                    receiptDate = reader[3] != DBNull.Value ? DateTime.Parse(reader[3].ToString()).ToShortDateString() : string.Empty;
                    grants[grant_index].Depositor.Add(new Depositor()
                    {
                        Title = grantDeposit,
                    });
                    grants[grant_index].DepositorSum.Add(float.Parse(grantDepositSum));
                    grants[grant_index].DepositorSumNoNDS.Add(float.Parse(grantDepositSumNoNDS));
                    grants[grant_index].ReceiptDate.Add(receiptDate);
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();


            // Получение заказчиков
            cmd = new NpgsqlCommand("SELECT grant_id, customer_id, customers.title, customers.short_title FROM grants " +
                                        "JOIN grants_customers ON grants.id = grants_customers.grant_id " +
                                        "JOIN customers ON customers.customerid = grants_customers.customer_id; ", conn);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    grants[grant_index].Customer.Add(new Customer()
                    {
                        Id = Convert.ToInt32(reader[1]),
                        Title = reader[2].ToString(),
                        ShortTitle = reader[3].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();



            // Получение исполнителей
            cmd = new NpgsqlCommand("SELECT grantId, FIO, executorId FROM executors " +
                                        "JOIN persons p on executors.executorId = p.id " +
                                        "JOIN grants g on executors.grantId = g.id " +
                                        "ORDER BY grantId; ", conn);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader[0]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    grants[grant_index].Executor.Add(new Person()
                    {
                        Id = Convert.ToInt32(reader[2]),
                        FIO = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }

            reader.Close();

            // Получение остальных столбцов
            cmd = new NpgsqlCommand("SELECT grants.id as gid, grants.grantnumber as ggn, OKVED, nameNIOKR, startDate, endDate, price, p2.FIO as lead_niokr, " +
                " GRNTI, NIR, NOC, pricenonds, is_with_nds, " +
                "first_node_id, second_node_id, third_node_id, fourth_node_id FROM grants " +
                                                        "LEFT JOIN persons p2 on grants.leadNIOKRId = p2.id ORDER BY grants.id;", conn);
            reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grant_id = Convert.ToInt32(reader["gid"]);
                    grant_index = ShowGrantIndex(grants, grant_id);

                    grants[grant_index].grantNumber = reader["ggn"].ToString();
                    grants[grant_index].OKVED = reader["OKVED"].ToString();
                    grants[grant_index].NameNIOKR = reader["nameNIOKR"].ToString();
                    grants[grant_index].StartDate = Convert.ToDateTime(reader["startDate"]);
                    grants[grant_index].EndDate = Convert.ToDateTime(reader["endDate"]);
                    grants[grant_index].Price = float.Parse(reader["price"].ToString());
                    grants[grant_index].PriceNoNDS = float.Parse(reader["pricenonds"].ToString());
                    grants[grant_index].LeadNIOKR = new Person() { FIO = reader["lead_niokr"].ToString() };
                    grants[grant_index].GRNTI = reader["GRNTI"].ToString();
                    grants[grant_index].NIR = reader["NIR"].ToString();
                    grants[grant_index].NOC = reader["NOC"].ToString();
                    grants[grant_index].isWIthNDS = Convert.ToBoolean(reader["is_with_nds"]);
                    grants[grant_index].FirstNode = reader["first_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["first_node_id"])) : new UniversityStructureNode();
                    grants[grant_index].SecondNode = reader["second_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["second_node_id"])) : new UniversityStructureNode();
                    grants[grant_index].ThirdNode = reader["third_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["third_node_id"])) : new UniversityStructureNode();
                    grants[grant_index].FourthNode = reader["fourth_node_id"] != DBNull.Value ? GetStructNodeById(Convert.ToInt32(reader["fourth_node_id"])) : new UniversityStructureNode();
                }
            }
            reader.Close();
            return grants;
        }

        /// <summary>
        /// Выгружает таблицу договоров на гланый экран
        /// </summary>
        /// <param name="dataTable"></param>
        public static void LoadGrantsTable(DataTable dataTable)
        {
            dataTable.Rows.Clear();

            Grant[] grants = GetAllGrants();

            for (int i = 0; i < grants.Length; i++)
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
            List<Person> persons = GetPersons();
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
            dataTable.Rows.Clear();

            // массив людей
            List<Customer> customers = new List<Customer>();

            // Получение остальных столбцов
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT customerid, title, short_title FROM customers ORDER BY customerid", conn);
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

            for (int i = 0; i < customers.Count; i++)
            {
                WorkerWithTablesOnMainForm.AddRowToCustomersTable(dataTable, customers[i]);
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
            for (int i = 0; i < grants.Length; i++)
            {
                if (grants[i].Id == id) index = i;
            }

            return index;
        }

        // Для метода GetGrantsHeadersForFilters
        enum DataToComboBox
        {
            okved = 1,
            customer = 4,
            deposits = 8,
            leadNIOKR = 9,
            institution = 10,
            unit = 11,
            kafedra = 12,
            laboratory = 13,
            executors = 14,
            researchTypes = 15,
            priorityTrends = 16,
            ScienceTypeItem = 17,
            NIRItem = 18
        }

        /// <summary>
        /// Получение названия полей
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<GrantHeader> GetGrantsHeadersForFilters()
        {
            ObservableCollection<GrantHeader> grantHeaders = new ObservableCollection<GrantHeader>();

            List<OKVED> okvedList = new List<OKVED>
            {
                new OKVED() {Title = "72.19"},
                new OKVED() {Title = "72.20"}
            };
            List<Customer> customerList = GetCustomers();
            List<ScienceType> scienceTypeList = GetScienceTypes();
            List<ResearchType> researchTypeList = GetResearchTypes();
            List<PriorityTrend> priorityTrendList = GetPriorityTrends();
            List<Person> peopleList = GetPersons();
            List<Depositor> depositList = GetDeposits();
            List<Nir> nirList = new List<Nir>() {
                new Nir() {Title = "НИР" },
                new Nir() {Title = "УСЛУГА" }
            };


            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, field_russian, field_english, is_combobox_needed, is_textbox_needed, is_comparison_needed, is_date_needed FROM filter_fields ORDER BY id", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                GrantHeader newGrantHeader;
                DataToComboBox curId;
                while (reader.Read())
                {
                    curId = (DataToComboBox)reader[0];

                    newGrantHeader = new GrantHeader()
                    {
                        nameOnRussia = reader[1].ToString(),
                        nameForElement = reader[2].ToString(),
                        Is_combobox_needed = (bool)reader[3],
                        Is_textbox_needed = (bool)reader[4],
                        Is_comparison_needed = (bool)reader[5],
                        Is_date_needed = (bool)reader[6]
                    };
                    switch (curId)
                    {
                        case DataToComboBox.okved:
                            newGrantHeader.DataToComboBox = new List<IContainer>(okvedList);
                            break;
                        case DataToComboBox.customer:
                            newGrantHeader.DataToComboBox = new List<IContainer>(customerList);
                            break;
                        case DataToComboBox.deposits:
                            newGrantHeader.DataToComboBox = depositList.ConvertAll(x => (IContainer)x);
                            break;
                        case DataToComboBox.leadNIOKR:
                            newGrantHeader.DataToComboBox = new List<IContainer>(peopleList);
                            break;
                        case DataToComboBox.executors:
                            newGrantHeader.DataToComboBox = new List<IContainer>(peopleList);
                            break;
                        case DataToComboBox.researchTypes:
                            newGrantHeader.DataToComboBox = researchTypeList.ConvertAll(x => (IContainer)x);
                            break;
                        case DataToComboBox.priorityTrends:
                            newGrantHeader.DataToComboBox = priorityTrendList.ConvertAll(x => (IContainer)x);
                            break;
                        case DataToComboBox.ScienceTypeItem:
                            newGrantHeader.DataToComboBox = scienceTypeList.ConvertAll(x => (IContainer)x);
                            break;
                        case DataToComboBox.NIRItem:
                            newGrantHeader.DataToComboBox = nirList.ConvertAll(x => (IContainer)x);
                            break;
                    }

                    grantHeaders.Add(newGrantHeader);
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();

            return grantHeaders;
        }


        /// <summary>
        /// Создание заголовков для таблицы договоров
        /// </summary>
        public static void CreateGrantsHeaders(DataTable dataTable)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, field_title, field_id FROM fieldslist ORDER BY id", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            WorkerWithTablesOnMainForm.AddHeadersToPersonTable(dataTable, "id");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    WorkerWithTablesOnMainForm.AddHeadersToGrantTable(dataTable, reader[1].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
        }

        /// <summary>
        /// Создание заголовков для таблицы людей
        /// </summary>
        public static void CreatePersonsHeaders(DataTable dataTable)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, name_field FROM fields_persons_list ORDER BY id", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            WorkerWithTablesOnMainForm.AddHeadersToPersonTable(dataTable, "id");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    WorkerWithTablesOnMainForm.AddHeadersToPersonTable(dataTable, reader[1].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
        }

        /// <summary>
        /// Создание заголовков для таблицы заказчиков
        /// </summary>
        public static void CreateCustomersHeaders(DataTable dataTable)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, name_field FROM fields_customers_list ORDER BY id", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            WorkerWithTablesOnMainForm.AddHeadersToCustomersTable(dataTable, "id");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    WorkerWithTablesOnMainForm.AddHeadersToCustomersTable(dataTable, reader[1].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
        }
        /// <summary>
        /// Получение списка людей
        /// </summary>
        /// <returns></returns>
        public static List<Person> GetPersons(bool is_jobs_needed = false)
        {
            List<Person> personsList = new List<Person>();
            List<int> persons_ids = new List<int>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id FROM persons ORDER BY id; ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                    persons_ids.Add(Convert.ToInt32(reader[0]));
            reader.Close();
            for (int i = 0; i < persons_ids.Count; i++)
            {
                personsList.Add(GetPerson(persons_ids[i], is_jobs_needed));
            }

            return personsList;
        }

        public static Person GetPerson(int person_id, bool is_jobs_needed = false)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT persons.id as pid, fio, birthdate, sex, degree_id,wd.title, rank_id, wr.title FROM persons " +
                                                    "LEFT  JOIN work_degree wd ON persons.degree_id = wd.id " +
                                                    "LEFT JOIN work_rank wr on persons.rank_id = wr.id " +
                                                    "WHERE persons.id = :person_id; ", conn);
            cmd.Parameters.Add(new NpgsqlParameter(":person_id", person_id));

            NpgsqlDataReader reader = cmd.ExecuteReader();

            Person newPerson = new Person();
            if (reader.HasRows)
            {
                reader.Read();
                newPerson.Id = Convert.ToInt32(reader["pid"]);

                newPerson.FIO = reader[1].ToString();

                newPerson.BitrhDate = (DateTime)reader[2];

                newPerson.Sex = (bool)reader[3];

                if (reader[4] != DBNull.Value)
                {
                    newPerson.Degree = new WorkDegree
                    {
                        Id = Convert.ToInt32(reader[4]),
                        Title = reader[5].ToString()
                    };
                }
                else
                {
                    newPerson.Degree = new WorkDegree();
                }

                if (reader[6] != DBNull.Value)
                {
                    newPerson.Rank = new WorkRank
                    {
                        Id = Convert.ToInt32(reader[6]),
                        Title = reader[7].ToString()
                    };
                }
                else
                {
                    newPerson.Rank = new WorkRank();
                }

            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            if (is_jobs_needed)
            {
                cmd = new NpgsqlCommand("SELECT persons_work_places.id, category_id, wc.title, is_main_work_place, first_node_id, second_node_id, third_node_id, fourth_node_id, " +
                    "(SELECT title FROM work_place_structure WHERE id = first_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = first_node_id), " +
                    "(SELECT title FROM work_place_structure WHERE id = second_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = second_node_id), " +
                    "(SELECT title FROM work_place_structure WHERE id = third_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = third_node_id), " +
                    "(SELECT title FROM work_place_structure WHERE id = fourth_node_id), " +
                    "(SELECT address FROM work_place_structure WHERE id = fourth_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = first_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = second_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = third_node_id), " +
                    "(SELECT short_title FROM work_place_structure WHERE id = fourth_node_id) " +
                    " FROM persons " +
                    "JOIN persons_work_places ON persons.id = persons_work_places.person_id " +
                    "LEFT JOIN work_categories wc on persons_work_places.category_id = wc.id " +
                    "WHERE persons.id = :person_id", conn);
                cmd.Parameters.Add(new NpgsqlParameter("person_id", newPerson.Id));

                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PersonWorkPlace workPlace = new PersonWorkPlace();
                        workPlace.workCategory = new WorkCategories();
                        workPlace.Id = reader[0] != DBNull.Value ? Convert.ToInt32(reader[0]) : -1;
                        workPlace.workCategory.Id = reader[1] != DBNull.Value ? Convert.ToInt32(reader[1]) : -1;
                        workPlace.workCategory.Title = reader[2] != DBNull.Value ? reader[2].ToString() : "";
                        workPlace.IsMainWorkPlace = reader[3] != DBNull.Value ? Convert.ToBoolean(reader[3]) : false;


                        if (reader[4] != DBNull.Value)
                        {
                            workPlace.firstNode = new UniversityStructureNode { Id = Convert.ToInt32(reader[4]) };
                            workPlace.firstNode.Title = reader[8] != DBNull.Value ? reader[8].ToString() : "";
                            workPlace.firstNode.Address = reader[9] != DBNull.Value ? reader[9].ToString() : "";
                            workPlace.firstNode.ShortTitle = reader[16] != DBNull.Value ? reader[16].ToString() : "";
                        }
                        else
                        {
                            workPlace.firstNode = new UniversityStructureNode();
                        }

                        if (reader[5] != DBNull.Value)
                        {
                            workPlace.secondNode = new UniversityStructureNode { Id = Convert.ToInt32(reader[5]) };
                            workPlace.secondNode.Title = reader[10] != DBNull.Value ? reader[10].ToString() : "";
                            workPlace.secondNode.Address = reader[11] != DBNull.Value ? reader[11].ToString() : "";
                            workPlace.secondNode.ShortTitle = reader[17] != DBNull.Value ? reader[17].ToString() : "";
                        }
                        else
                        {
                            workPlace.secondNode = new UniversityStructureNode();
                        }

                        if (reader[6] != DBNull.Value)
                        {
                            workPlace.thirdNode = new UniversityStructureNode { Id = Convert.ToInt32(reader[6]) };
                            workPlace.thirdNode.Title = reader[12] != DBNull.Value ? reader[12].ToString() : "";
                            workPlace.thirdNode.Address = reader[13] != DBNull.Value ? reader[13].ToString() : "";
                            workPlace.thirdNode.ShortTitle = reader[18] != DBNull.Value ? reader[18].ToString() : "";
                        }
                        else
                        {
                            workPlace.thirdNode = new UniversityStructureNode();
                        }

                        if (reader[7] != DBNull.Value)
                        {
                            workPlace.fourthNode = new UniversityStructureNode { Id = Convert.ToInt32(reader[7]) };
                            workPlace.fourthNode.Title = reader[14] != DBNull.Value ? reader[14].ToString() : "";
                            workPlace.fourthNode.Address = reader[15] != DBNull.Value ? reader[15].ToString() : "";
                            workPlace.fourthNode.ShortTitle = reader[19] != DBNull.Value ? reader[19].ToString() : "";
                        }
                        else
                        {
                            workPlace.fourthNode = new UniversityStructureNode();
                        }
                        workPlace.jobList = new List<Job>();

                        newPerson.workPlaces.Add(workPlace);
                    }
                }
                reader.Close();



                foreach (PersonWorkPlace workPlace in newPerson.workPlaces)
                {

                    cmd = new NpgsqlCommand("SELECT persons_jobs.job_id, j.title, j.salary, salary_rate FROM persons_jobs " +
                        "LEFT JOIN jobs j on persons_jobs.job_id = j.id " +
                        "WHERE persons_work_places_id = :persons_work_places_id;", conn);
                    cmd.Parameters.Add(new NpgsqlParameter("persons_work_places_id", workPlace.Id));
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            workPlace.jobList.Add(new Job
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Title = reader[1].ToString(),
                                Salary = Convert.ToSingle(reader[2]),
                                SalaryRate = reader[3] == DBNull.Value ? 0 : Convert.ToSingle(reader[3])
                            });
                        }
                    }
                    reader.Close();
                }
            }
            return newPerson;
        }

        public static List<Customer> GetCustomers()
        {
            List<Customer> customersList = new List<Customer>();
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
        public static List<PriorityTrend> GetPriorityTrends()
        {
            List<PriorityTrend> priorityTrendsList = new List<PriorityTrend>();
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
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return priorityTrendsList;
        }

        /// <summary>
        /// Получение списка типов исследования
        /// </summary>
        /// <returns></returns>
        public static List<ResearchType> GetResearchTypes()
        {
            List<ResearchType> researchTypesList = new List<ResearchType>();
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
        public static List<ScienceType> GetScienceTypes()
        {
            List<ScienceType> scienctTypeTypesList = new List<ScienceType>();
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
            cmd.Parameters.Add(new NpgsqlParameter("birthdate", person.BitrhDate));
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
            cmd.Parameters.Add(new NpgsqlParameter("startdate", fixedGrant.StartDate));
            cmd.Parameters.Add(new NpgsqlParameter("id", fixedGrant.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление даты окончания в бд
        /// </summary>
        public static void UpdateEndDate(Grant fixedGrant)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE grants SET enddate = :enddate WHERE id = :id", conn);
            cmd.Parameters.Add(new NpgsqlParameter("enddate", fixedGrant.EndDate));
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
            cmd.Parameters.Add(new NpgsqlParameter("bd", fixedPerson.BitrhDate));
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
            int newMaxGrantId = 0;

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
                ":fourth_node_id)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantnumber", grant.grantNumber));
            cmd.Parameters.Add(new NpgsqlParameter("okved", grant.OKVED));
            cmd.Parameters.Add(new NpgsqlParameter("nameniokr", grant.NameNIOKR));
            cmd.Parameters.Add(new NpgsqlParameter("startdate", grant.StartDate));
            cmd.Parameters.Add(new NpgsqlParameter("enddate", grant.EndDate));
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


            cmd.ExecuteNonQuery();



            // Ищем id договора, который только что добавили
            cmd = new NpgsqlCommand("SELECT id FROM grants ORDER BY id DESC", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                newMaxGrantId = Convert.ToInt32(reader[0]);
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();


            // Вставляем заказчиков
            CRUDDataBase.AddCustomers(grant, newMaxGrantId);

            // Вставляем исполнителей
            CRUDDataBase.AddExecutors(grant, newMaxGrantId);

            // Добавление источников средств в БД
            CRUDDataBase.AddDeposits(grant, newMaxGrantId);

            // Добавление типов исследования
            foreach (ResearchType rType in grant.ResearchType)
            {
                cmd = new NpgsqlCommand("insert into grantresearchtype (" +
                "grantid, " +
                "researchtypeid) " +
                "values(" +
                ":grantid, " +
                ":researchtypeid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newMaxGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("researchtypeid", rType.Id));
                cmd.ExecuteNonQuery();
            }

            // Добавление типов науки
            CRUDDataBase.AddScienceTypes(grant, newMaxGrantId);

            // Добавление приоритетных направлений
            CRUDDataBase.AddPriorityTrends(grant, newMaxGrantId);
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

        public static Grant GetGrantById(string grantId)
        {
            ConnectToDataBase();
            Grant[] grants = GetAllGrants();
            CloseConnection();
            Grant grant = new Grant();
            for (int i = 0; i < grants.Length; i++)
            {
                if (grants[i].Id == Convert.ToInt32(grantId))
                {
                    grant = grants[i];
                    break;
                }
            }
            return grant;
        }

        public static Person GetPersonByPersonId(string personId)
        {
            ConnectToDataBase();
            Person person = GetPerson(Convert.ToInt32(personId), true);
            CloseConnection();
            return person;
        }

        public static Customer GetCustomerByCustomerId(string customerId)
        {
            ConnectToDataBase();
            List<Customer> customers = GetCustomers();
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

        public static ObservableCollection<UniversityStructureNode> GetStructureNodes(string regex)
        {
            ConnectToDataBase();

            ObservableCollection<UniversityStructureNode> NodeList = new ObservableCollection<UniversityStructureNode>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, address, title, short_title FROM work_place_structure WHERE address ~ " + regex + "ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    NodeList.Add(new UniversityStructureNode()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Address = reader[1].ToString(),
                        Title = reader[2].ToString(),
                        ShortTitle = reader[3].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            CloseConnection();
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
    }
}
