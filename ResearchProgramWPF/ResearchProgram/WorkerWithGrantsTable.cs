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
    public static class WorkerWithGrantsTable
    {
        // Таблица договоров
        public static DataTable GrantsDataTable { get;  set; }

        private static int countOfRows = 0;


        public static void AddHeadersToGrantTable(string header)
        {
            GrantsDataTable.Columns.Add(header);
        }

        public static void AddRowToGrantTable(Grant grant)
        {
            countOfRows++;

            GrantsDataTable.Rows.Add(
                    countOfRows.ToString(),
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
    }
}
