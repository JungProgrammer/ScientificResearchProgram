using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ResearchProgram
{
    public static class WorkerWithTablesOnMainForm
    {
        private static int countOfGrantRows = 0;
        private static int countOfPersonRows = 0;
        private static int countOfCustomerRows = 0;

        public static List<TableHeader> PersonsHeaders = new List<TableHeader>()
        {
            new TableHeader{Title = "id", IsService=true },
            new TableHeader{Title = "#" , IsService=true},
            new TableHeader{Title = "ФИО" },
            new TableHeader{Title = "Дата рождения" },
            new TableHeader{Title = "Пол" },
            new TableHeader{Title = "Степень" },
            new TableHeader{Title = "Звание" },
            new TableHeader{Title = "Учреждение" },
            new TableHeader{Title = "Подразделение" },
            new TableHeader{Title = "Отдел" },
            new TableHeader{Title = "Структурная единица" },
        };

        public static List<TableHeader> GrantsHeaders = new List<TableHeader>()
        {
            new TableHeader{Title = "id", IsService=true },
            new TableHeader{Title = "№", IsService=true },
            new TableHeader{Title = "Номер договора" },
            new TableHeader{Title = "ОКВЭД", IsCountable = true },
            new TableHeader{Title = "Наименование НИОКР", IsCountable = true},
            new TableHeader{Title = "Заказчик", IsMultiple = true, IsCountable = true },
            new TableHeader{Title = "Дата начала" },
            new TableHeader{Title = "Дата завершения" },
            new TableHeader{Title = "Стоимость договора" },
            new TableHeader{Title = "Источник финансирования", IsMultiple = true, IsCountable = true },
            new TableHeader{Title = "Поступления", IsMultiple = true },
            new TableHeader{Title = "Руководитель НИОКР", IsCountable = true },
            new TableHeader{Title = "Исполнители", IsMultiple = true, IsCountable = true },
            new TableHeader{Title = "Учреждение", IsCountable = true },
            new TableHeader{Title = "Подразделение", IsCountable = true },
            new TableHeader{Title = "Отдел", IsCountable = true },
            new TableHeader{Title = "Структурная единица", IsCountable = true },
            new TableHeader{Title = "ГРНТИ", IsCountable = true },
            new TableHeader{Title = "Тип исследования", IsMultiple = true, IsCountable = true },
            new TableHeader{Title = "Приоритетные направления", IsMultiple = true, IsCountable = true },
            new TableHeader{Title = "Тип науки", IsMultiple = true, IsCountable = true },
            new TableHeader{Title = "НИР или УСЛУГА", IsCountable = true },
            new TableHeader{Title = "НОЦ", IsCountable = true },
            new TableHeader{Title = "Наличие НДС", IsCountable = true }
        };

        public static List<TableHeader> CustomersHeaders = new List<TableHeader>()
        {
            new TableHeader{Title = "id", IsService=true },
            new TableHeader{Title = "#", IsService=true },
            new TableHeader{Title = "Наименование" },
            new TableHeader{Title = "Полное наименование" },
        };

        /// <summary>
        /// Загружает заголовки в указанную таблицу
        /// </summary>
        public static void CreateHeaders(DataTable dataTable, List<TableHeader> headers)
        {
            foreach (TableHeader header in headers)
            {
                DataColumn column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = header.Title,
                    Caption = header.Title
                };
                dataTable.Columns.Add(column);
            }
        }

        /// <summary>
        /// Добавляет строку в grantDataTable
        /// </summary>
        /// <param name="grantsDataTable"></param>
        /// <param name="grant"></param>
        public static void AddRowToGrantTable(DataTable grantsDataTable, Grant grant)
        {
            // Словарь для отображения средств
            Dictionary<string, double> depositDict = new Dictionary<string, double>();
            for (int i = 0; i < grant.Depositor.Count; i++)
            {
                string depositorStr;
                double depositorSum;
                double depositorSumNoNDS;

                depositorStr = grant.Depositor[i].Title;
                depositorSum = grant.DepositorSum[i];
                depositorSumNoNDS = grant.DepositorSumNoNDS[i];

                // Если в словаре такое средство уже есть, то суммируем
                if (depositDict.ContainsKey(depositorStr))
                {
                    if (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey)
                    {
                        depositDict[depositorStr] += depositorSumNoNDS;
                    }
                    else
                    {
                        depositDict[depositorStr] += depositorSum;
                    }
                }
                else
                {
                    if (depositorStr != string.Empty && depositorStr != string.Empty)
                    {
                        if (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey)
                        {
                            depositDict.Add(depositorStr, depositorSumNoNDS);
                        }
                        else
                        {
                            depositDict.Add(depositorStr, depositorSum);
                        }
                    }
                }
            }

            // Строки для отображения
            string depositors = string.Empty;
            string depositsSum = "";

            // Перевод словарей в строки отображения
            foreach (string depositor in depositDict.Keys)
            {
                depositors += depositor + '\n';
            }
            foreach (double depositorSum in depositDict.Values)
            {
                depositsSum += string.Format("{0:#,0.##}", depositorSum) + '\n';
            }

            var first = grant.PriceNoNDS.ToString("C");

            if (grantsDataTable.Rows.Count == 0) countOfGrantRows = 0;
            countOfGrantRows++;
            DataRow row = grantsDataTable.NewRow();
            row["id"] = grant.Id;
            row["№"] = countOfGrantRows.ToString();
            row["Номер договора"] = grant.grantNumber;
            row["ОКВЭД"] = grant.OKVED;
            row["Наименование НИОКР"] = grant.NameNIOKR;
            row["Заказчик"] = string.Join("\n", grant.Customer);
            row["Дата начала"] = grant.StartDate == null ? "" : grant.StartDate?.ToString("dd.MM.yyyy");
            row["Дата завершения"] = grant.EndDate == null ? "" : grant.EndDate?.ToString("dd.MM.yyyy");
            row["Стоимость договора"] = (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey) ? String.Format("{0:#,0.##}", grant.PriceNoNDS) : String.Format("{0:#,0.##}", grant.Price);
            row["Источник финансирования"] = depositors;
            row["Поступления"] = depositsSum;
            row["Руководитель НИОКР"] = grant.LeadNIOKR != null ? grant.LeadNIOKR.shortName() : "";
            row["Исполнители"] = string.Join("\n", grant.Executor.Select(x => x.shortName()).ToArray());
            row["Учреждение"] = grant.FirstNode;
            row["Подразделение"] = grant.SecondNode;
            row["Отдел"] = grant.ThirdNode;
            row["Структурная единица"] = grant.FourthNode;
            row["ГРНТИ"] = grant.GRNTI;
            row["Тип исследования"] = string.Join("\n", grant.ResearchType);
            row["Приоритетные направления"] = string.Join("\n", grant.PriorityTrands);
            row["Тип науки"] = string.Join("\n", grant.ScienceType);
            row["НИР или УСЛУГА"] = grant.NIR;
            row["НОЦ"] = grant.NOC == "True" ? "Да" : grant.NOC == "False" ? "Нет" : "";
            row["Наличие НДС"] = grant.isWIthNDS ? "Да" : "Нет";
            grantsDataTable.Rows.Add(row);
        }

        /// <summary>
        /// Добавляет строку в personDataTable
        /// </summary>
        /// <param name="personsDataTable"></param>
        /// <param name="person"></param>
        public static void AddRowToPersonsTable(DataTable personsDataTable, Person person)
        {
            if (personsDataTable.Rows.Count == 0) countOfPersonRows = 0;
            countOfPersonRows++;
            DataRow row = personsDataTable.NewRow();
            row["#"] = countOfPersonRows.ToString();
            row["id"] = person.Id;
            row["ФИО"] = person.FIO;
            row["Дата рождения"] = person.BitrhDate == null ? "" : person.BitrhDate?.ToString("dd.MM.yyyy");
            row["Пол"] = person.Sex ? "M" : "Ж";
            row["Степень"] = person.Degree != null ? person.Degree.Title : "";
            row["Звание"] = person.Rank != null ? person.Rank.Title : "";

            PersonWorkPlace personWorkPlace = person.GetActiveWorkPlace();

            row["Учреждение"] = personWorkPlace.firstNode != null ? personWorkPlace.firstNode.ToString() : "";
            row["Подразделение"] = personWorkPlace.secondNode != null ? personWorkPlace.secondNode.ToString() : "";
            row["Отдел"] = personWorkPlace.thirdNode != null ? personWorkPlace.thirdNode.ToString() : "";
            row["Структурная единица"] = personWorkPlace.fourthNode != null ? personWorkPlace.fourthNode.ToString() : "";
            personsDataTable.Rows.Add(row);
        }

        public static void AddRowToCustomersTable(DataTable customersDataTable, Customer customer)
        {
            if (customersDataTable.Rows.Count == 0) countOfCustomerRows = 0;
            countOfCustomerRows++;
            DataRow row = customersDataTable.NewRow();
            row["id"] = customer.Id;
            row["#"] = countOfCustomerRows;
            row["Наименование"] = customer.ShortTitle;
            row["Полное наименование"] = customer.Title;
            customersDataTable.Rows.Add(row);
        }
    }
}
