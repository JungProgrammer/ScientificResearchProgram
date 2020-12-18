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
            grantsDataTable.Columns.Add(header);
        }

        /// <summary>
        /// Загружает заголовки в personsDataTable
        /// </summary>
        /// <param name="personsDataTable"></param>
        /// <param name="header"></param>
        public static void AddHeadersToPersonTable(DataTable personsDataTable, string header)
        {
            personsDataTable.Columns.Add(header);
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

                grantsDataTable.Rows.Add(
                           countOfGrantRows.ToString(),
                           grant.grantNumber,
                           grant.OKVED,
                           grant.NameNIOKR,
                           grant.Customer.Title,
                           grant.StartDate.ToString("dd.MM.yyyy"),
                           grant.EndDate.ToString("dd.MM.yyyy"),
                           grant.Price * (Settings.Default.NDSKey ? 1 : 1 / Settings.Default.NDSValue),
                           String.Join("\n", grant.Depositor),
                           String.Join("\n", grant.DepositorSum.Select(x => x * (Settings.Default.NDSKey ? 1 : 1 / Settings.Default.NDSValue)).ToArray()),
                           grant.LeadNIOKR.shortName(),
                           String.Join("\n", grant.Executor.Select(x => x.shortName()).ToArray()),
                           grant.Institution,
                           grant.Unit,
                           grant.Kafedra,
                           grant.Laboratory,
                           grant.GRNTI,
                           String.Join("\n", grant.ResearchType),
                           String.Join("\n", grant.PriorityTrands),
                           String.Join("\n", grant.ScienceType),
                           grant.NIR,
                           grant.NOC == "True" ? "Да" : grant.NOC == "False" ? "Нет" : "");
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

            personsDataTable.Rows.Add(
                    countOfPersonRows.ToString(),
                    person.FIO,
                    person.BitrhDate,
                    person.Sex ? "M" : "Ж",
                    person.PlaceOfWork,
                    person.Category,
                    person.Degree,
                    person.Rank,
                    String.Join("\n", person.Jobs),
                    String.Join("\n", Job.GetSalariesFromPerson(person.Jobs)),
                    String.Join("\n", Job.GetSalaryRatesFromPerson(person.Jobs))
                    );
        }
    }
}
