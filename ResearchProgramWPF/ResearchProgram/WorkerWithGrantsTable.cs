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
        public static void AddHeadersToGrantTable(DataTable grantsDataTable, string header, string id)
        {
            //grantsDataTable.Columns.Add(header);
            DataColumn column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = header,
                Caption = id
            };
            grantsDataTable.Columns.Add(column);
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
                row["Дата начала"]              = grant.StartDate.ToString("dd.MM.yyyy");
                row["Дата завершения"]          = grant.EndDate.ToString("dd.MM.yyyy");
                row["Общая сумма договора"]     = grant.Price * (Settings.Default.NDSKey ? 1 : 1 / Settings.Default.NDSValue);
                row["Средства"]                 = string.Join("\n", grant.Depositor);
                row["Подробное финансирование"] = string.Join("\n", grant.DepositorSum.Select(x => x * (Settings.Default.NDSKey ? 1 : 1 / Settings.Default.NDSValue)).ToArray());
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
            countOfPersonRows++;
            DataRow row = personsDataTable.NewRow();
            row["#"]                            = countOfPersonRows.ToString();
            row["ФИО"]                          = person.FIO;
            row["Дата рождения"]                = person.BitrhDate;
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
    }
}
