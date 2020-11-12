using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Diagnostics;

namespace ResearchProgram
{
    class CRUDDataBase
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
            cmd = new NpgsqlCommand("SELECT grantId, FIO, isExecutorContract, executorId FROM executors " +
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
                        grants[grant_index].ExecutorContract.Add(new Person()
                        {
                            Id = Convert.ToInt32(reader[3]),
                            FIO = reader[1].ToString()
                        });
                    }
                    else
                    {
                        grants[grant_index].Executor.Add(new Person()
                        {
                            Id = Convert.ToInt32(reader[3]),
                            FIO = reader[1].ToString()
                        });
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
                    grants[grant_index].Customer = new Person() { FIO = reader[3].ToString() };
                    grants[grant_index].StartDate = Convert.ToDateTime(reader[4]);
                    grants[grant_index].EndDate = Convert.ToDateTime(reader[5]);
                    grants[grant_index].Price = Convert.ToInt32(reader[6]);
                    grants[grant_index].LeadNIOKR = new Person() { FIO = reader[7].ToString() };
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


            for (int i = 0; i < grants.Length; i++)
            {
                dataTable.Rows.Add((i + 1).ToString(),
                    grants[i].OKVED,
                    grants[i].NameNIOKR,
                    grants[i].Customer,
                    grants[i].StartDate,
                    grants[i].EndDate,
                    grants[i].Price,
                    String.Join("\n", grants[i].Depositor),
                    String.Join("\n", grants[i].DepositorSum),
                    grants[i].LeadNIOKR,
                    String.Join("\n", grants[i].Executor),
                    grants[i].Kafedra,
                    grants[i].Unit,
                    grants[i].Institution,
                    grants[i].GRNTI,
                    String.Join("\n", grants[i].ResearchType),
                    String.Join("\n", grants[i].PriorityTrand),
                    String.Join("\n", grants[i].ExecutorContract),
                    String.Join("\n", grants[i].ScienceType),
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
            for (int i = 0; i < grants.Length; i++)
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
        /// <summary>
        /// Получение списка людей
        /// </summary>
        /// <returns></returns>
        public static List<Person> GetPersons()
        {
            List<Person> personsList = new List<Person>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT Id,FIO FROM persons ORDER BY FIO;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    personsList.Add(new Person()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        FIO = reader[1].ToString()
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return personsList;
        }
        /// <summary>
        /// Получение списка средств
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDeposits()
        {
            List<string> depositsList = new List<string>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM depositors ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    depositsList.Add(reader[0].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return depositsList;
        }

        public static void CloseConnect()
        {
            conn.Close();
        }
        /// <summary>
        /// Получение списка кафедр
        /// </summary>
        /// <returns></returns>
        public static List<string> GetKafedras()
        {
            List<string> kafedrasList = new List<string>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM kafedras ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    kafedrasList.Add(reader[0].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return kafedrasList;
        }
        /// <summary>
        /// Получение списка подразделений
        /// </summary>
        /// <returns></returns>
        public static List<string> GetUnits()
        {
            List<string> unitsList = new List<string>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM units ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    unitsList.Add(reader[0].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return unitsList;
        }

        /// <summary>
        /// Получение списка учреждений
        /// </summary>
        /// <returns></returns>
        public static List<string> GetInstitutions()
        {
            List<string> institutionsList = new List<string>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM institutions ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    institutionsList.Add(reader[0].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return institutionsList;
        }

        /// <summary>
        /// Получение списка типов исследования
        /// </summary>
        /// <returns></returns>
        public static List<string> GetResearchTypes()
        {
            List<string> researchTypesList = new List<string>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM researchTypes ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    researchTypesList.Add(reader[0].ToString());
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
        public static List<string> GetScienceTypes()
        {
            List<string> researchTypesList = new List<string>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM scienceTypes ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    researchTypesList.Add(reader[0].ToString());
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();
            return researchTypesList;
        }
    }
}
