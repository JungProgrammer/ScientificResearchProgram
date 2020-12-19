using System;
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
    public class CRUDDataBase
    {
        //public static string loginFromDB = Environment.GetEnvironmentVariable("PGUSER");
        //public static string passwordFromDB = Environment.GetEnvironmentVariable("PGPASSWORD");

        public static string loginFromDB = "postgres";
        public static string passwordFromDB = "XeKhM9bQnRYah";


        private static NpgsqlConnection conn;

        /// <summary>
        /// Подключение к БД
        /// </summary>
        public static void ConnectToDataBase()
        {
            conn = new NpgsqlConnection($"Server=212.192.88.14; Port=5432; User Id={loginFromDB}; Password={passwordFromDB}; Database=postgres");
            conn.Open();
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
            cmd = new NpgsqlCommand("SELECT grants.id, grants.grantnumber, OKVED, nameNIOKR, p.title, startDate, endDate, price, p2.FIO, k.title, u.title, i.title, GRNTI, NIR, NOC, l.title FROM grants " +
                                                        "LEFT JOIN customers p on grants.customerId = p.customerid " +
                                                        "LEFT JOIN persons p2 on grants.leadNIOKRId = p2.id " +
                                                        "LEFT JOIN kafedras k on grants.kafedraId = k.id " +
                                                        "LEFT JOIN units u on grants.unitId = u.id " +
                                                        "LEFT JOIN laboratories l on grants.laboratoryid = l.id " +
                                                        "LEFT JOIN institutions i on grants.institutionId = i.id ORDER BY grants.id;", conn);
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
                    grants[grant_index].Customer = new Customer() { Title = reader[4].ToString() };
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
                    grants[grant_index].Laboratory = new Laboratory() { Title = reader[15].ToString() };
                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
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


        internal static Institution AddNewInstitution(string institutionTitle)
        {
            Institution newInstitution = null;

            NpgsqlCommand cmd = new NpgsqlCommand("insert into institutions (" +
                "title) " +
                "values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", institutionTitle));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM institutions ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                newInstitution = new Institution()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = institutionTitle
                };
            }

            return newInstitution;
        }

        internal static Unit AddNewUnit(string unitTitle, int institutionId)
        {
            Unit newUnit = null;

            NpgsqlCommand cmd = new NpgsqlCommand("insert into units (institutionid," +
                "title) " +
                "values(:institutionid," +
                ":title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", unitTitle));
            cmd.Parameters.Add(new NpgsqlParameter("institutionid", institutionId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM units ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                newUnit = new Unit()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = unitTitle
                };
            }


            return newUnit;
        }

        internal static Kafedra AddNewKafedra(string kafedraTitle, int unitId)
        {
            Kafedra newKafedra = null;

            NpgsqlCommand cmd = new NpgsqlCommand("insert into kafedras (unitid," +
                "title) " +
                "values(:unitid," +
                ":title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", kafedraTitle));
            cmd.Parameters.Add(new NpgsqlParameter("unitid", unitId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM kafedras ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                newKafedra = new Kafedra()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = kafedraTitle
                };
            }

            return newKafedra;
        }

        /// <summary>
        /// Добавление лаборатории в подразделение
        /// </summary>
        /// <param name="laboratoryTitle"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        internal static Laboratory AddNewLaboratoryToUnit(string laboratoryTitle, int unitId)
        {
            Laboratory newLaboratory = null;

            NpgsqlCommand cmd = new NpgsqlCommand("insert into laboratories (unitid," +
                "title) " +
                "values(:unitid," +
                ":title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", laboratoryTitle));
            cmd.Parameters.Add(new NpgsqlParameter("unitid", unitId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM laboratories ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                newLaboratory = new Laboratory()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = laboratoryTitle
                };
            }

            return newLaboratory;
        }

        /// <summary>
        /// Добавление лаборатории в кафедру
        /// </summary>
        /// <param name="laboratoryTitle"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        internal static Laboratory AddNewLaboratoryToKafedra(string laboratoryTitle, int kafedraId)
        {
            Laboratory newLaboratory = null;

            NpgsqlCommand cmd = new NpgsqlCommand("insert into laboratories (kafedraid," +
                "title) " +
                "values(:kafedraid," +
                ":title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", laboratoryTitle));
            cmd.Parameters.Add(new NpgsqlParameter("kafedraid", kafedraId));
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("SELECT id FROM laboratories ORDER BY id DESC ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                newLaboratory = new Laboratory()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Title = laboratoryTitle
                };
            }

            return newLaboratory;
        }

        /// <summary>
        /// Переименование универа
        /// </summary>
        /// <param name="institution"></param>
        internal static void RenameInstitution(Institution institution, string newTitle)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE institutions SET title = :title WHERE id = :institutionid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newTitle));
            cmd.Parameters.Add(new NpgsqlParameter("institutionid", institution.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Переименование подразделение
        /// </summary>
        /// <param name="institution"></param>
        internal static void RenameUnit(Unit unit, string newTitle)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE units SET title = :title WHERE id = :unitid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newTitle));
            cmd.Parameters.Add(new NpgsqlParameter("unitid", unit.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Переименование кафедры
        /// </summary>
        /// <param name="institution"></param>
        internal static void RenameKafedra(Kafedra kafedra, string newTitle)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE kafedras SET title = :title WHERE id = :kafedraid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newTitle));
            cmd.Parameters.Add(new NpgsqlParameter("kafedraid", kafedra.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Переименование лаборатории
        /// </summary>
        /// <param name="institution"></param>
        internal static void RenameLaboratory(Laboratory laboratory, string newTitle)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE laboratories SET title = :title WHERE id = :laboratoryid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", newTitle));
            cmd.Parameters.Add(new NpgsqlParameter("laboratoryid", laboratory.Id));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление университета
        /// </summary>
        /// <param name="institutionId"></param>
        internal static void DeleteInstitution(int institutionId)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM institutions WHERE id = :institutionid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("institutionid", institutionId));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление подразделения
        /// </summary>
        /// <param name="unitId"></param>
        internal static void DeleteUnit(int unitId)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM units WHERE id = :unitid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("unitid", unitId));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление кафедры
        /// </summary>
        /// <param name="kafedraId"></param>
        internal static void DeleteKafedra(int kafedraId)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM kafedras WHERE id = :kafedraid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("kafedraid", kafedraId));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление лаборатории
        /// </summary>
        /// <param name="laboratoryId"></param>
        internal static void DeleteLaboratory(int laboratoryId)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM laboratories WHERE id = :laboratoryid;", conn);
            cmd.Parameters.Add(new NpgsqlParameter("laboratoryid", laboratoryId));
            cmd.ExecuteNonQuery();
        }


        internal static WorkerWithUniversityStructure GetUniversityStructure()
        {
            WorkerWithUniversityStructure universityStructure = new WorkerWithUniversityStructure();

            // Сначала получаем всю структуру, где лаборатории привязаны к кафедрам
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT institutions.id, institutions.title, units.id, units.title, kafedras.id, kafedras.title, l1.id, l1.title FROM institutions " +
                                                    "JOIN units ON institutions.id = units.institutionid " +
                                                    "JOIN kafedras ON units.id = kafedras.unitid " +
                                                    "JOIN laboratories l1 ON kafedras.id = l1.kafedraid " +
                                                    "ORDER BY institutions.id; ", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                int institutionId;
                int unitId;
                int kafedraId;
                int laboratoryId;

                string institutionTitle;
                string unitTitle;
                string kafedraTitle;
                string laboratoryTitle;

                Institution institutionFound;
                Unit unitFound;
                Kafedra kafedraFound;
                Laboratory laboratoryFound;

                while (reader.Read())
                {
                    institutionId = Convert.ToInt32(reader[0]);
                    institutionTitle = reader[1].ToString();
                    unitId = Convert.ToInt32(reader[2]);
                    unitTitle = reader[3].ToString();
                    kafedraId = Convert.ToInt32(reader[4]);
                    kafedraTitle = reader[5].ToString();
                    laboratoryId = Convert.ToInt32(reader[6]);
                    laboratoryTitle = reader[7].ToString();

                    institutionFound = universityStructure.FindInstitution(institutionId);
                    // Если такого учреждения еще нет
                    if(institutionFound == null)
                    {
                        institutionFound = new Institution()
                        {
                            Id = institutionId,
                            Title = institutionTitle
                        };

                        // Добавляем в список учреждений
                        universityStructure.Institutions.Add(institutionFound);
                    }

                    unitFound = universityStructure.FindUnit(institutionFound, unitId);
                    // Если такого подразделения еще нет
                    if(unitFound == null)
                    {
                        unitFound = new Unit()
                        {
                            Id = unitId,
                            Title = unitTitle
                        };

                        // Добавляем его в список подразделений найденного университета
                        institutionFound.Units.Add(unitFound);
                    }

                    kafedraFound = universityStructure.FindKafedra(unitFound, kafedraId);
                    // Если такой кафедры еще нет
                    if(kafedraFound == null)
                    {
                        kafedraFound = new Kafedra()
                        {
                            Id = kafedraId,
                            Title = kafedraTitle
                        };

                        // Добавление кафедры в подразделение
                        unitFound.Kafedras.Add(kafedraFound);
                    }

                    laboratoryFound = universityStructure.FindLaboratoryInKafedra(kafedraFound, laboratoryId);
                    // Если такая лаборатория еще не существует
                    if(laboratoryFound == null)
                    {
                        laboratoryFound = new Laboratory()
                        {
                            Id = laboratoryId,
                            Title = laboratoryTitle
                        };

                        kafedraFound.Laboratories.Add(laboratoryFound);
                    }

                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();


            // Теперь получаем все записи, где лаборатория привязана к подразделению
            cmd = new NpgsqlCommand("SELECT institutions.id, institutions.title, units.id, units.title, laboratories.id, laboratories.title FROM institutions " +
                                        "JOIN units ON institutions.id = units.institutionid " +
                                        "JOIN laboratories ON unitid = units.id " +
                                        "ORDER BY institutions.id; ", conn);
            reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                int institutionId;
                int unitId;
                int laboratoryId;

                string institutionTitle;
                string unitTitle;
                string laboratoryTitle;

                Institution institutionFound;
                Unit unitFound;
                Laboratory laboratoryFound;

                while (reader.Read())
                {
                    institutionId = Convert.ToInt32(reader[0]);
                    institutionTitle = reader[1].ToString();
                    unitId = Convert.ToInt32(reader[2]);
                    unitTitle = reader[3].ToString();
                    laboratoryId = Convert.ToInt32(reader[4]);
                    laboratoryTitle = reader[5].ToString();

                    institutionFound = universityStructure.FindInstitution(institutionId);
                    // Если такого учреждения еще нет
                    if (institutionFound == null)
                    {
                        institutionFound = new Institution()
                        {
                            Id = institutionId,
                            Title = institutionTitle
                        };

                        // Добавляем в список учреждений
                        universityStructure.Institutions.Add(institutionFound);
                    }

                    unitFound = universityStructure.FindUnit(institutionFound, unitId);
                    // Если такого подразделения еще нет
                    if (unitFound == null)
                    {
                        unitFound = new Unit()
                        {
                            Id = unitId,
                            Title = unitTitle
                        };

                        // Добавляем его в список подразделений найденного университета
                        institutionFound.Units.Add(unitFound);
                    }

                    laboratoryFound = universityStructure.FindLaboratoryInUnit(unitFound, laboratoryId);
                    // Если такая лаборатория еще не существует
                    if (laboratoryFound == null)
                    {
                        laboratoryFound = new Laboratory()
                        {
                            Id = laboratoryId,
                            Title = laboratoryTitle
                        };

                        unitFound.Laboratories.Add(laboratoryFound);
                    }

                }
            }
            else
            {
                Debug.WriteLine("No rows found.");
            }
            reader.Close();

            return universityStructure;
        }

        /// <summary>
        /// Выгружает таблицу людей
        /// </summary>
        /// <param name="dataTable"></param>
        public static void LoadPersonsTable(DataTable dataTable)
        {
            int personIndex;
            int personId;
            int countOfPeople;

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

        // Для метода GetGrantsHeadersForFilters
        enum DataToComboBox
        {
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

            List<Customer> customerList = GetCustomers();
            List<ScienceType> scienceTypeList = GetScienceTypes();
            List<ResearchType> researchTypeList = GetResearchTypes();
            List<PriorityTrend> priorityTrendList = GetPriorityTrends();
            List<Institution> institutionList = GetInstitutions();
            List<Unit> unitList = GetUnits();
            List<Kafedra> kafedraList = GetKafedras();
            List<Laboratory> laboratoryList = GetLaboratories();
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
                        case DataToComboBox.institution:
                            newGrantHeader.DataToComboBox = institutionList.ConvertAll(x => (IContainer)x);
                            break;
                        case DataToComboBox.unit:
                            newGrantHeader.DataToComboBox = unitList.ConvertAll(x => (IContainer)x);
                            break;
                        case DataToComboBox.kafedra:
                            newGrantHeader.DataToComboBox = kafedraList.ConvertAll(x => (IContainer)x);
                            break;
                        case DataToComboBox.laboratory:
                            newGrantHeader.DataToComboBox = laboratoryList.ConvertAll(x => (IContainer)x);
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

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    WorkerWithTablesOnMainForm.AddHeadersToGrantTable(dataTable, reader[1].ToString(), reader[2].ToString());
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

        public static List<Customer> GetCustomers()
        {
            List<Customer> customersList = new List<Customer>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT customerid, title FROM customers ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    customersList.Add(new Customer()
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
        /// Получение списка лабораторий
        /// </summary>
        public static List<Laboratory> GetLaboratories()
        {
            List<Laboratory> laboratories = new List<Laboratory>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, title FROM laboratories ORDER BY title;", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    laboratories.Add(new Laboratory()
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

            return laboratories;
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
                "grantnumber, " +
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
                "laboratoryid, " +
                "grnti, " +
                "nir, " +
                "noc) " +
                "values(" +
                ":grantnumber, " +
                ":okved, " +
                ":nameniokr, " +
                ":customerid, " +
                ":startdate, " +
                ":enddate, " +
                ":price, " +
                ":leadniokrid, " +
                ":kafedraid, " +
                ":unitid, " +
                ":institutionid, " +
                ":laboratoryid, " +
                ":grnti, " +
                ":nir, " +
                ":noc)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantnumber", grant.grantNumber));
            cmd.Parameters.Add(new NpgsqlParameter("okved", grant.OKVED));
            cmd.Parameters.Add(new NpgsqlParameter("nameniokr", grant.NameNIOKR));
            cmd.Parameters.Add(new NpgsqlParameter("customerid", grant.Customer.Id));
            cmd.Parameters.Add(new NpgsqlParameter("startdate", grant.StartDate));
            cmd.Parameters.Add(new NpgsqlParameter("enddate", grant.EndDate));
            cmd.Parameters.Add(new NpgsqlParameter("leadniokrid", grant.LeadNIOKR.Id));
            cmd.Parameters.Add(new NpgsqlParameter("institutionid", grant.Institution != null ? grant.Institution.Id : 0));
            cmd.Parameters.Add(new NpgsqlParameter("unitid", grant.Unit != null ? grant.Unit.Id : 0));
            cmd.Parameters.Add(new NpgsqlParameter("kafedraid", grant.Kafedra != null ? grant.Kafedra.Id : 0));
            cmd.Parameters.Add(new NpgsqlParameter("laboratoryid", grant.Laboratory != null ? grant.Laboratory.Id : 0));
            cmd.Parameters.Add(new NpgsqlParameter("price", grant.Price));
            cmd.Parameters.Add(new NpgsqlParameter("grnti", grant.GRNTI));
            cmd.Parameters.Add(new NpgsqlParameter("nir", grant.NIR));
            cmd.Parameters.Add(new NpgsqlParameter("noc", grant.NOC == "Да"));

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
                "executorid)" +
                "values(" +
                ":grantid, " +
                ":executorid)", conn);
                cmd.Parameters.Add(new NpgsqlParameter("grantid", newMaxGrantId));
                cmd.Parameters.Add(new NpgsqlParameter("executorid", executor.Id));
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

        public static void InsertNewKafedraToDB(Kafedra kafedra)
        {
            // Вставляем в БД новую кафедру
            NpgsqlCommand cmd = new NpgsqlCommand("insert into kafedras (title) values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", kafedra.Title));

            cmd.ExecuteNonQuery();
        }

        public static void InsertNewDepositToDB(Depositor depositor)
        {
            // Вставляем в БД новый источник средств
            NpgsqlCommand cmd = new NpgsqlCommand("insert into depositors (title) values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", depositor.Title));

            cmd.ExecuteNonQuery();
        }

        public static void InsertNewUnitToDB(Unit unit)
        {
            // Вставляем в БД новое подразделение
            NpgsqlCommand cmd = new NpgsqlCommand("insert into units (title) values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", unit.Title));

            cmd.ExecuteNonQuery();
        }

        public static void InsertNewInstituitonToDB(Institution institution)
        {
            // Вставляем в БД новое учреждение
            NpgsqlCommand cmd = new NpgsqlCommand("insert into institutions (title) values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", institution.Title));

            cmd.ExecuteNonQuery();
        }

        public static void InsertNewResearchTypeToDB(ResearchType researchType)
        {
            // Вставляем в БД новый тип исследования
            NpgsqlCommand cmd = new NpgsqlCommand("insert into researchtypes (title) values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", researchType.Title));

            cmd.ExecuteNonQuery();
        }

        public static void InsertNewPriorityTrendsToDB(PriorityTrend priorityTrend)
        {
            // Вставляем в БД новое приоритетное напрвление
            NpgsqlCommand cmd = new NpgsqlCommand("insert into prioritytrends (title) values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", priorityTrend.Title));

            cmd.ExecuteNonQuery();
        }

        public static void InsertNewScienceTypeToDB(ScienceType scienceType)
        {
            // Вставляем в БД новый тип науки
            NpgsqlCommand cmd = new NpgsqlCommand("insert into sciencetypes (title) values(:title)", conn);
            cmd.Parameters.Add(new NpgsqlParameter("title", scienceType.Title));

            cmd.ExecuteNonQuery();
        }

        public static bool IsGrantNumberAvailable(string grantNumber)
        {
            ConnectToDataBase();
            // Проверяем не занят ли номер договора, который пытаются вставлять/изменять
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT grantNumber FROM grants WHERE grantNumber = :grantNumber", conn);
            cmd.Parameters.Add(new NpgsqlParameter("grantNumber", grantNumber));
            NpgsqlDataReader reader = cmd.ExecuteReader();
            bool answer;
            if (reader.HasRows)
                answer =  false;
            else
                answer =  true;
            CloseConnection();

            return answer;

        }

        public static Grant GetGrantByGrantNumber(string grantNumber)
        {
            ConnectToDataBase();
            Grant[] grants = GetAllGrants();
            CloseConnection();
            Grant grant = new Grant();
            for(int i = 0;i < grants.Length; i++)
            {
                if (grants[i].grantNumber == grantNumber){
                    grant = grants[i];
                    break;
                }
            }
            return grant;
        }

    }
}
