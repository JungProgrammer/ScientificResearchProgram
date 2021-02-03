using System;
using System.Data;
using Npgsql;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
                    column.DataType = Type.GetType("System.Double");
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
            string depositors = String.Empty;
            string depositsSum = String.Empty;
            for (int i = 0; i < grant.Depositor.Count; i++)
            {
                if (GrantsFilters.CheckReceiptDate(grant.ReceiptDate[i]))
                {
                    depositors += grant.Depositor[i].Title + "\n";
                    if (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey)
                    {
                            depositsSum += grant.DepositorSumNoNDS[i] + "\n";
                    }
                    else
                    {
                        depositsSum += grant.DepositorSum[i] + "\n";
                    }
                }
            }



            // Если договор подходит под фильтр
            if (GrantsFilters.CheckGrantOnCurFilter(grant))
            {
                if (grantsDataTable.Rows.Count == 0) countOfGrantRows = 0;
                countOfGrantRows++;
                DataRow row = grantsDataTable.NewRow();
                row["№"]                        = countOfGrantRows.ToString();
                row["Номер договора"]           = grant.grantNumber;
                row["ОКВЭД"]                    = grant.OKVED;
                row["Наименование НИОКР"]       = grant.NameNIOKR;
                row["Заказчик"]                 = string.Join("\n", grant.Customer);
                row["Дата начала"]              = grant.StartDate == new DateTime(1, 1, 1) ? "": grant.StartDate.ToString("dd.MM.yyyy");
                row["Дата завершения"]          = grant.EndDate == new DateTime(1, 1, 1) ? "" : grant.EndDate.ToString("dd.MM.yyyy");
                row["Стоимость договора"]       = (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey) ? grant.PriceNoNDS.ToString() : grant.Price.ToString();
                row["Источник финансирования"]  = depositors;
                row["Поступления"]              = depositsSum;
                row["Руководитель НИОКР"]       = grant.LeadNIOKR.shortName();
                row["Исполнители"]              = string.Join("\n", grant.Executor.Select(x => x.shortName()).ToArray()); 
                row["Учреждение"]               = grant.Institution;
                row["Подразделение"]            = grant.Unit;
                row["Кафедра"]                  = grant.Kafedra;
                row["Лаборатория"]              = grant.Laboratory;
                row["ГРНТИ"]                    = grant.GRNTI;
                row["Тип исследования"]         = string.Join("\n", grant.ResearchType);
                row["Приоритетные направления"] = string.Join("\n", grant.PriorityTrands);
                row["Тип науки"]                = string.Join("\n", grant.ScienceType);
                row["НИР или УСЛУГА"]           = grant.NIR;
                row["НОЦ"]                      = grant.NOC == "True" ? "Да" : grant.NOC == "False" ? "Нет" : "";
                row["Наличие НДС"]              = grant.isWIthNDS ? "Да" : "Нет";
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
            row["#"]                            = countOfPersonRows.ToString();
            row["id"]                           = person.Id;
            row["ФИО"]                          = person.FIO;
            row["Дата рождения"]                = person.BitrhDate == new DateTime(1, 1, 1) ? "" : person.BitrhDate.ToString("dd.MM.yyyy");
            row["Пол"]                          = person.Sex ? "M" : "Ж";
            row["Место работы"]                 = person.PlaceOfWork;
            row["Категория"]                    = person.Category;
            row["Степень"]                      = person.Degree;
            row["Звание"]                       = person.Rank;
            row["Должность"]                    = string.Join("\n", person.Jobs);
            row["Оклад"]                        = string.Join("\n", Job.GetSalariesFromPerson(person.Jobs));
            row["Ставка"]                       = string.Join("\n", Job.GetSalaryRatesFromPerson(person.Jobs));
            personsDataTable.Rows.Add(row);

        }

        public static void AddRowToCustomersTable(DataTable customersDataTable, Customer customer)
        {
            DataRow row = customersDataTable.NewRow();
            row["id"] = customer.Id;
            row["Наименование"] = customer.Title;
            customersDataTable.Rows.Add(row);
        }
    }
}
