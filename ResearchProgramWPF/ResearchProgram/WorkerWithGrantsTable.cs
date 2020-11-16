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

        public static void AddHeadersToGrantTable(DataTable grantsTable, string header)
        {
            grantsTable.Columns.Add(header);
        }

        public static void AddHeadersToPersonTable(DataTable personsTable, string header)
        {
            personsTable.Columns.Add(header);
        }

        public static void AddRowToGrantTable(DataTable dataTable, Grant grant)
        {
            countOfGrantRows++;

            dataTable.Rows.Add(
                    countOfGrantRows.ToString(),
                    grant.grantNumber,
                    grant.OKVED,
                    grant.NameNIOKR,
                    grant.Customer,
                    grant.StartDate,
                    grant.EndDate,
                    grant.Price,
                    String.Join("\n", grant.Depositor),
                    String.Join("\n", grant.DepositorSum),
                    grant.LeadNIOKR,
                    String.Join("\n", grant.Executor),
                    grant.Kafedra,
                    grant.Unit,
                    grant.Institution,
                    grant.GRNTI,
                    String.Join("\n", grant.ResearchType),
                    String.Join("\n", grant.PriorityTrands),
                    String.Join("\n", grant.ExecutorContract),
                    String.Join("\n", grant.ScienceType),
                    grant.NIR,
                    grant.NOC);
        }

        public static void AddRowToPersonsTable(DataTable dataTable, Person person)
        {
            countOfPersonRows++;

            dataTable.Rows.Add(
                    countOfGrantRows.ToString(),
                    person.FIO,
                    person.BitrhDate,
                    person.Sex.ToString(),
                    person.PlaceOfWork,
                    person.Category,
                    person.Degree,
                    person.Rank,
                    String.Join("\n", person.Jobs)
                    );
        }
    }
}
