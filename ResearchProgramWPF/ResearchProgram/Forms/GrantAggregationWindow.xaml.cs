using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResearchProgram.Forms
{
    public partial class GrantAggregationWindow : Window
    {
        DataTable _grantsDataTable;
        public GrantAggregationWindow(DataTable GrantDataTable)
        {
            InitializeComponent();
            _grantsDataTable = GrantDataTable;
            DataContext = this;
        }

        private void StartAggregatopnButton_Click(object sender, RoutedEventArgs e)
        {
            List<DataRow> filteredRows;
            if (GrantsFilters.grantFilter != null)
            {
                //Получаем строки, которые отфильтрованы простым фильтром
                filteredRows = _grantsDataTable.Select(GrantsFilters.grantFilter).ToList();
            }
            else
            {
                //Получаем строки, которые отфильтрованы сложным фильтром
                filteredRows = _grantsDataTable.Select().ToList();
            }
            Console.WriteLine(filteredRows.Count);

            List<int> ids = filteredRows.Select(x => (int)x["id"]).ToList();
            List<Grant> filteredGrants = StaticData.grants.Where(x => ids.Contains(x.Id)).ToList();

            if (WhatToCountComboBox.SelectedIndex == 0)
            {
                double PriceSum = filteredGrants.Sum(x => x.Price);
                double PriceSumNoNds = filteredGrants.Sum(x => x.PriceNoNDS);

                Console.WriteLine(PriceSum);
                Console.WriteLine(PriceSumNoNds);
            }
            else if (WhatToCountComboBox.SelectedIndex == 1)
            {

            }
        }
    }
}
