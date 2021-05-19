using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
//using System.Windows.Controls;

namespace ResearchProgram
{
    public class WorkerWithTablesOnMainForm
    {
        private static int countOfGrantRows = 0;
        private static int countOfPersonRows = 0;

        public WorkerWithTablesOnMainForm() { }

        /// <summary>
        /// Загружает заголовки в grantsDataTable
        /// </summary>
        /// <param name="grantsDataTable"></param>
        /// <param name="header"></param>
        public static void AddHeadersToGrantTable(DataTable grantsDataTable, string header)
        {
            //grantsDataTable.Columns.Add(header);
            DataColumn column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = header,
                Caption = header,
            };
            //switch (column.ColumnName)
            //{
            //    case "№":
            //        column.DataType = Type.GetType("System.Int32");
            //        break;
            //    case "Стоимость договора":
            //        //column.DataType = Type.GetType("System.Double");
            //        break;
            //}

            grantsDataTable.Columns.Add(column);
        }

        /// <summary>
        /// Загружает заголовки в customerssDataTable
        /// </summary>
        /// <param name="grantsDataTable"></param>
        /// <param name="header"></param>
        public static void AddHeadersToCustomersTable(DataTable customersDataTable, string header)
        {
            DataColumn column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = header,
                Caption = header
            };
            customersDataTable.Columns.Add(column);
        }

        /// <summary>
        /// Загружает заголовки в personsDataTable
        /// </summary>
        /// <param name="personsDataTable"></param>
        /// <param name="header"></param>
        public static void AddHeadersToPersonTable(DataTable personsDataTable, string header)
        {
            DataColumn column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = header,
                Caption = header
            };
            personsDataTable.Columns.Add(column);

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


            //var first = string.Format("0k", grant.PriceNoNDS);
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
            row["Степень"] = person.Degree.Title;
            row["Звание"] = person.Rank.Title;
            personsDataTable.Rows.Add(row);
        }

        public static void AddRowToCustomersTable(DataTable customersDataTable, Customer customer)
        {
            DataRow row = customersDataTable.NewRow();
            row["id"] = customer.Id;
            row["Наименование"] = customer.ShortTitle;
            row["Полное наименование"] = customer.Title;
            customersDataTable.Rows.Add(row);
        }
    }
}
