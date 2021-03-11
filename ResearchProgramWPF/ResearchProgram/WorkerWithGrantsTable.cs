using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

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
                Caption = header
            };
            switch (column.ColumnName)
            {
                case "№":
                    column.DataType = Type.GetType("System.Int32");
                    break;
                case "Стоимость договора":
                    //column.DataType = Type.GetType("System.Double");
                    break;
            }


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
            Dictionary<string, float> depositDict = new Dictionary<string, float>();
            for (int i = 0; i < grant.Depositor.Count; i++)
            {
                string depositorStr;
                float depositorSum;
                float depositorSumNoNDS;
                if (GrantsFilters.CheckReceiptDate(grant.ReceiptDate[i]))
                {
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
                        if (depositorStr != String.Empty && depositorStr != String.Empty)
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
            }

            // Строки для отображения
            string depositors = String.Empty;
            string depositsSum = "";

            // Перевод словарей в строки отображения
            foreach (string depositor in depositDict.Keys)
            {
                depositors += depositor + '\n';
            }
            foreach (float depositorSum in depositDict.Values)
            {
                depositsSum += depositorSum.ToString("0.############") + '\n';
            }


            // Если договор подходит под фильтр
            if (GrantsFilters.CheckGrantOnCurFilter(grant))
            {
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
                row["Дата начала"] = grant.StartDate == new DateTime(1, 1, 1) ? "" : grant.StartDate.ToString("dd.MM.yyyy");
                row["Дата завершения"] = grant.EndDate == new DateTime(1, 1, 1) ? "" : grant.EndDate.ToString("dd.MM.yyyy");
                row["Стоимость договора"] = (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey) ? String.Format("{0:0.##}", grant.PriceNoNDS) : String.Format("{0:0.##}", grant.Price);
                //row["Стоимость договора"] = (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey) ? grant.PriceNoNDS : grant.Price;
                row["Источник финансирования"] = depositors;
                row["Поступления"] = depositsSum;
                row["Руководитель НИОКР"] = grant.LeadNIOKR.shortName();
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
            row["Дата рождения"] = person.BitrhDate == new DateTime(1, 1, 1) ? "" : person.BitrhDate.ToString("dd.MM.yyyy");
            row["Пол"] = person.Sex ? "M" : "Ж";
            //row["Место работы"]                 = person.PlaceOfWork;
            //row["Категория"]                    = person.Category;
            row["Степень"] = person.Degree.Title;
            row["Звание"] = person.Rank.Title;
            //row["Должность"]                    = string.Join("\n", person.Jobs);
            //row["Оклад"]                        = string.Join("\n", Job.GetSalariesFromPerson(person.Jobs));
            //row["Ставка"]                       = string.Join("\n", Job.GetSalaryRatesFromPerson(person.Jobs));
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
