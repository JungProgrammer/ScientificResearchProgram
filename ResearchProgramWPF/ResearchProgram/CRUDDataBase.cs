﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace ResearchProgram
{
    class CRUDDataBase
    {
        public static string loginFromDB = Environment.GetEnvironmentVariable("PGUSER");
        public static string passwordFromDB = Environment.GetEnvironmentVariable("PGPASSWORD");

        private static NpgsqlConnection conn;

        /// <summary>
        /// Подключение к БД
        /// </summary>
        public static void ConnectByDataBase()
        {
            conn = new NpgsqlConnection($"Server=localhost; Port=5432; User Id={loginFromDB}; Password={passwordFromDB}; Database=postgres");
            conn.Open();
        }

        /// <summary>
        /// Закрытия соединения с БД
        /// </summary>
        public static void CloseConnect()
        {
            conn.Close();
        }

        /// <summary>
        /// Выгружает таблицу договоров на гланый экран
        /// </summary>
        /// <param name="dataTable"></param>
        public static void LoadGrantsTable(DataTable dataTable)
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
                    grants[grant_index].PriorityTrands.Add(new PriorityTrend() { 
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
                    grants[grant_index].ScienceType.Add(new ScienceType() { 
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
                    grants[grant_index].Depositor.Add(new Depositor()
                    {
                        Title = grantDeposit
                    });
                    grants[grant_index].DepositorSum.Add(float.Parse(grantDepositSum));
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
            cmd = new NpgsqlCommand("SELECT grants.id, grants.grantnumber, OKVED, nameNIOKR, p.FIO, startDate, endDate, price, p2.FIO, k.title, u.title, i.title, GRNTI, NIR, NOC FROM grants " +
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

                    grants[grant_index].grantNumber = reader[1].ToString();
                    grants[grant_index].OKVED = reader[2].ToString();
                    grants[grant_index].NameNIOKR = reader[3].ToString();
                    grants[grant_index].Customer = new Person() { FIO = reader[4].ToString() };
                    grants[grant_index].StartDate = Convert.ToDateTime(reader[5]);
                    grants[grant_index].EndDate = Convert.ToDateTime(reader[6]);
                    grants[grant_index].Price = float.Parse(reader[7].ToString());
                    grants[grant_index].LeadNIOKR = new Person() { FIO = reader[8].ToString() };
                    grants[grant_index].Kafedra = new Kafedra() { Title = reader[9].ToString() };
                    grants[grant_index].Unit = new Unit() { Title = reader[10].ToString() };
                    grants[grant_index].Institution = new Institution() { Title = reader[11].ToString() };
                    grants[grant_index].GRNTI = reader[12].ToString();
                    grants[grant_index].NIR = reader[13].ToString();
                    grants[grant_index].NOC = reader[14].ToString();
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();


            for (int i = 0; i < grants.Length; i++)
            {
                WorkerWithTablesOnMainForm.AddRowToGrantTable(dataTable, grants[i]);
            }
        }

        /// <summary>
        /// Выгружает таблицу договоров на гланый экран
        /// </summary>
        /// <param name="dataTable"></param>
        public static void LoadPersonsTable(DataTable dataTable)
        {
            int personIndex = 0;
            int personId = 0;
            int countOfPeople = 0;

            // массив людей
            Person[] persons = null;

            // Инициализация массива людей и присваивание им id
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, (SELECT COUNT(*) FROM persons) FROM persons;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                int i;
                reader.Read();
                countOfPeople = Convert.ToInt32(reader[1]);
                // Инициализация договоров
                persons = new Person[countOfPeople];
                for (i = 0; i < countOfPeople; i++) persons[i] = new Person();

                personId = Convert.ToInt32(reader[0]);
                persons[0].Id = personId;

                i = 1;
                while (reader.Read())
                {
                    personId = Convert.ToInt32(reader[0]);
                    persons[i].Id = personId;
                    i++;
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();


            // Получение работ человека
            cmd = new NpgsqlCommand("SELECT personid, title, salary, salaryrate FROM persons " +
                                        "JOIN salaryrates ON persons.id = salaryrates.personid " +
                                        "JOIN jobs ON salaryrates.jobid = jobs.id; ", conn);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    personId = Convert.ToInt32(reader[0]);
                    personIndex = ShowPersonIndex(persons, personId);

                    persons[personIndex].Jobs.Add(new Job() { 
                        Title = reader[1].ToString(),
                        Salary = float.Parse(reader[2].ToString()),
                        SalaryRate = float.Parse(reader[3].ToString())
                    });
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();



            // Получение остальных столбцов
            cmd = new NpgsqlCommand("SELECT id, fio, birthdate, sex, placeofwork, category, degree, rank FROM persons", conn);
            reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    personId = Convert.ToInt32(reader[0]);
                    personIndex = ShowPersonIndex(persons, personId);

                    persons[personIndex].FIO = reader[1].ToString();
                    persons[personIndex].BitrhDate = (DateTime)reader[2];
                    persons[personIndex].Sex = (bool)reader[3];
                    persons[personIndex].PlaceOfWork = reader[4].ToString();
                    persons[personIndex].Category = reader[5].ToString();
                    persons[personIndex].Degree = reader[6].ToString();
                    persons[personIndex].Rank = reader[7].ToString();
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();


            for (int i = 0; i < persons.Length; i++)
            {
                WorkerWithTablesOnMainForm.AddRowToPersonsTable(dataTable, persons[i]);
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
        /// Ищет индекс гранта в массиве по id гранта
        /// </summary>
        /// <param name="grants"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        static int ShowPersonIndex(Person[] persons, int id)
        {
            int index = 0;
            for (int i = 0; i < persons.Length; i++)
            {
                if (persons[i].Id == id) index = i;
            }

            return index;
        }

        /// <summary>
        /// Получение названия полей
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<GrantHeader> GetGrantsHeadersForFilters()
        {
            ObservableCollection<GrantHeader> grantHeaders = new ObservableCollection<GrantHeader>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, field_russian, field_english, is_combobox_needed, is_textbox_needed, is_comparison_needed, is_date_needed FROM filter_fields ORDER BY id", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grantHeaders.Add(new GrantHeader(){
                        nameOnRussia = reader[1].ToString(),
                        nameForElement = reader[2].ToString(),
                        Is_combobox_needed = (bool)reader[3],
                        Is_textbox_needed = (bool)reader[4],
                        Is_comparison_needed = (bool)reader[5],
                        Is_date_needed = (bool)reader[6]
                    });
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
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, field_title FROM fieldslist ORDER BY id", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

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
        /// Получение списка людей
        /// </summary>
        /// <returns></returns>
        public static List<Person> GetPersons()
        {
            List<Person> personsList = new List<Person>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT Id, FIO FROM persons ORDER BY FIO;", conn);
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
        public static List<Depositor> GetDeposits()
        {
            List<Depositor> depositsList = new List<Depositor>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM depositors ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    depositsList.Add(new Depositor() { 
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
        /// Получение списка кафедр
        /// </summary>
        /// <returns></returns>
        public static List<Kafedra> GetKafedras()
        {
            List<Kafedra> kafedrasList = new List<Kafedra>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM kafedras ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    kafedrasList.Add(new Kafedra()
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

            return kafedrasList;
        }
        /// <summary>
        /// Получение списка подразделений
        /// </summary>
        /// <returns></returns>
        public static List<Unit> GetUnits()
        {
            List<Unit> unitsList = new List<Unit>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM units ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    unitsList.Add(new Unit()
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

            return unitsList;
        }

        /// <summary>
        /// Получение списка учреждений
        /// </summary>
        /// <returns></returns>
        public static List<Institution> GetInstitutions()
        {
            List<Institution> institutionsList = new List<Institution>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM institutions ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    institutionsList.Add(new Institution() { 
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
            return institutionsList;
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
                    researchTypesList.Add(new ResearchType() { 
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
                    scienctTypeTypesList.Add(new ScienceType() { 
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
        public static void InsertNewPersonToDB(Person person)
        {
            // Id человека, который будет создан
            int newMaxPersonId = 0;

            // Вставляем в БД нового человека
            NpgsqlCommand cmd = new NpgsqlCommand("insert into persons (" +
                "fio, " +
                "birthdate, " +
                "sex, " +
                "placeofwork, " +
                "category, " +
                "degree, " +
                "rank) " +
                "values(:fio, " +
                ":birthdate, " +
                ":sex, " +
                ":placeofwork, " +
                ":category, " +
                ":degree, " +
                ":rank)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("fio", person.FIO));
            cmd.Parameters.Add(new NpgsqlParameter("birthdate", person.BitrhDate));
            cmd.Parameters.Add(new NpgsqlParameter("sex", person.Sex));
            cmd.Parameters.Add(new NpgsqlParameter("placeofwork", person.PlaceOfWork));
            cmd.Parameters.Add(new NpgsqlParameter("category", person.Category));
            cmd.Parameters.Add(new NpgsqlParameter("degree", person.Degree));
            cmd.Parameters.Add(new NpgsqlParameter("rank", person.Rank));

            cmd.ExecuteNonQuery();


            // Ищем id человека, которого только что добавили
            cmd = new NpgsqlCommand("SELECT id FROM persons ORDER BY id DESC", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                newMaxPersonId = Convert.ToInt32(reader[0]);
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();


            // Вставляем работы
            foreach (Job job in person.Jobs)
            {
                cmd = new NpgsqlCommand("insert into salaryrates (" +
                "personid, " +
                "jobid," +
                "salaryrate) " +
                "values(" +
                ":personid, " +
                ":jobid, " +
                ":salaryrate)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("personid", newMaxPersonId));
                cmd.Parameters.Add(new NpgsqlParameter("jobid", job.Id));
                cmd.Parameters.Add(new NpgsqlParameter("salaryrate", job.SalaryRate));
                cmd.ExecuteNonQuery();
            }
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
                "okved, " +
                "nameniokr, " +
                "customerid, " +
                "startdate, " +
                "enddate, " +
                "price, " +
                "leadniokrid, " +
                "kafedraid, " +
                "unitid, " +
                "institutionid, " +
                "grnti, " +
                "nir, " +
                "noc) " +
                "values(:okved, " +
                ":nameniokr, " +
                ":customerid, " +
                ":startdate, " +
                ":enddate, " +
                ":price, " +
                ":leadniokrid, " +
                ":kafedraid, " +
                ":unitid, " +
                ":institutionid, " +
                ":grnti, " +
                ":nir, " +
                ":noc)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("okved", grant.OKVED));
            cmd.Parameters.Add(new NpgsqlParameter("nameniokr", grant.NameNIOKR));
            cmd.Parameters.Add(new NpgsqlParameter("customerid", grant.Customer.Id));
            cmd.Parameters.Add(new NpgsqlParameter("startdate", grant.StartDate));
            cmd.Parameters.Add(new NpgsqlParameter("enddate", grant.EndDate));
            cmd.Parameters.Add(new NpgsqlParameter("leadniokrid", grant.LeadNIOKR.Id));
            cmd.Parameters.Add(new NpgsqlParameter("kafedraid", grant.Kafedra.Id));
            cmd.Parameters.Add(new NpgsqlParameter("unitid", grant.Unit.Id));
            cmd.Parameters.Add(new NpgsqlParameter("institutionid", grant.Institution.Id));
            cmd.Parameters.Add(new NpgsqlParameter("price", grant.Price));
            cmd.Parameters.Add(new NpgsqlParameter("grnti", grant.GRNTI));
            cmd.Parameters.Add(new NpgsqlParameter("nir", grant.NIR));
            cmd.Parameters.Add(new NpgsqlParameter("noc", grant.NOC));

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



            // Вставляем исполнителей
            foreach(Person executor in grant.Executor)
            {
                cmd = new NpgsqlCommand("insert into executors (" +
                "grantid, " +
                "executorid," +
                "isexecutorcontract) " +
                "values(" +
                ":grantid, " +
                ":executorid, " +
                ":isexecutorcontract)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newMaxGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("executorid", executor.Id));
                cmd.Parameters.Add(new NpgsqlParameter("isexecutorcontract", false));
                cmd.ExecuteNonQuery();
            }
            foreach (Person executor in grant.ExecutorContract)
            {
                cmd = new NpgsqlCommand("insert into executors (" +
                "grantid, " +
                "executorid," +
                "isexecutorcontract) " +
                "values(" +
                ":grantid, " +
                ":executorid, " +
                ":isexecutorcontract)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newMaxGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("executorid", executor.Id));
                cmd.Parameters.Add(new NpgsqlParameter("isexecutorcontract", true));
                cmd.ExecuteNonQuery();
            }


            // Добавление источников средств в БД
            for(int i = 0; i < grant.Depositor.Count(); i++)
            {
                cmd = new NpgsqlCommand("insert into grantdeposits (" +
                "grantid, " +
                "sourceid," +
                "partsum) " +
                "values(" +
                ":grantid, " +
                ":sourceid, " +
                ":partsum)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newMaxGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("sourceid", grant.Depositor[i].Id));
                cmd.Parameters.Add(new NpgsqlParameter("partsum", grant.DepositorSum[i]));
                cmd.ExecuteNonQuery();
            }

            // Добавление типов исследования
            foreach(ResearchType rType in grant.ResearchType)
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
            foreach(ScienceType sType in grant.ScienceType)
            {
                cmd = new NpgsqlCommand("insert into grantsciencetypes (" +
                "grantid, " +
                "sciencetypesid) " +
                "values(" +
                ":grantid, " +
                ":sciencetypesid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newMaxGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("sciencetypesid", sType.Id));
                cmd.ExecuteNonQuery();
            }

            // Добавление приоритетных направлений
            foreach (PriorityTrend priorityTrend in grant.PriorityTrands)
            {
                cmd = new NpgsqlCommand("insert into grantprioritytrends (" +
                "grantid, " +
                "prioritytrendsid) " +
                "values(" +
                ":grantid, " +
                ":prioritytrendsid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newMaxGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("prioritytrendsid", priorityTrend.Id));
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
    }
}
