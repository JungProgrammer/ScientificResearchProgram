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
using ResearchProgram.Classes;
using static ResearchProgram.Utilities;

namespace ResearchProgram.Forms
{
    public partial class GrantAggregationWindow : Window
    {
        DataTable _grantsDataTable;

        private List<TableHeader> _aggregationCountSource;
        public List<TableHeader> AggregationCountSource { get { return _aggregationCountSource; } set { _aggregationCountSource = value; } }

        private TableHeader _aggregationCountSelectedItem;
        public TableHeader AggregationCountSelectedItem { get { return _aggregationCountSelectedItem; } set { _aggregationCountSelectedItem = value; } }
        public GrantAggregationWindow(DataTable GrantDataTable)
        {
            InitializeComponent();
            _grantsDataTable = GrantDataTable;

            List<TableHeader> headers = new List<TableHeader>(WorkerWithTablesOnMainForm.GrantsHeaders);

            AggregationCountSource = new List<TableHeader>();
            IEnumerable<TableHeader> tableHeaders = headers.Where(x => x.IsService == false && x.IsCountable == true);
            foreach (TableHeader s in tableHeaders)
            {
                AggregationCountSource.Add(s);
            }
            AggregationCountSelectedItem = tableHeaders.First();

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

            List<int> ids = filteredRows.Select(x => Convert.ToInt32(x["id"])).ToList();
            List<Grant> filteredGrants = StaticData.grants.Where(x => ids.Contains(x.Id)).ToList();

            if (WhatToCountComboBox.SelectedIndex == 0)
            {
                double PriceSum = filteredGrants.Sum(x => x.Price);
                double PriceSumNoNds = filteredGrants.Sum(x => x.PriceNoNDS);

                Console.WriteLine(PriceSum);
                Console.WriteLine(PriceSumNoNds);

                // TODO СУММА СРЕДСТВ
            }
            else if (WhatToCountComboBox.SelectedIndex == 1)
            {
                Dictionary<string, int> stringDataDict = new Dictionary<string, int>();
                Dictionary<int, MappedValue> MappedDataDict = new Dictionary<int, MappedValue>();
                Dictionary<bool?, int> boolDataDict = new Dictionary<bool?, int>();

                switch (AggregationCountSelectedItem.Title)
                {

                    case "ОКВЭД":
                        foreach (Grant g in filteredGrants)
                            if (!stringDataDict.ContainsKey(g.OKVED))
                                stringDataDict[g.OKVED] = 1;
                            else
                                stringDataDict[g.OKVED]++;
                        break;

                    case "Наименование НИОКР":
                        foreach (Grant g in filteredGrants)
                            if (!stringDataDict.ContainsKey(g.NameNIOKR))
                                stringDataDict[g.NameNIOKR] = 1;
                            else
                                stringDataDict[g.NameNIOKR]++;
                        break;

                    case "Заказчик":
                        foreach (Grant g in filteredGrants)
                            foreach (Customer c in g.Customer)
                                if (!MappedDataDict.ContainsKey(c.Id))
                                    MappedDataDict[c.Id] = new MappedValue() { Count = 1, Title = c.Title };
                                else
                                    MappedDataDict[c.Id].Count++;
                        break;

                    case "Источник финансирования":
                        foreach (Grant g in filteredGrants)
                            foreach (GrantDepositor d in g.Depositors)
                                if (!MappedDataDict.ContainsKey(d.Depositor.Id))
                                    MappedDataDict[d.Depositor.Id] = new MappedValue() { Count = 1, Title = d.Depositor.Title };
                                else
                                    MappedDataDict[d.Depositor.Id].Count++;
                        break;

                    case "Руководитель НИОКР":
                        MappedDataDict[-1] = new MappedValue() { Count = 0, Title = "Нет значения" };
                        foreach (Grant g in filteredGrants)
                            if (g.LeadNIOKR != null)
                            {
                                if (!MappedDataDict.ContainsKey(g.LeadNIOKR.Id))
                                    MappedDataDict[g.LeadNIOKR.Id] = new MappedValue() { Count = 1, Title = g.LeadNIOKR.FIO };
                            }
                            else
                                MappedDataDict[-1].Count++;
                        break;

                    case "Исполнители":
                        foreach (Grant g in filteredGrants)
                            foreach (Person ex in g.Executor)
                                if (!MappedDataDict.ContainsKey(ex.Id))
                                    MappedDataDict[ex.Id] = new MappedValue() { Count = 1, Title = ex.FIO };
                                else
                                    MappedDataDict[ex.Id].Count++;
                        break;

                    case "Учреждение":
                        MappedDataDict[-1] = new MappedValue() { Count = 0, Title = "Нет значения" };
                        foreach (Grant g in filteredGrants)
                            if (g.FirstNode != null)
                            {
                                if (!MappedDataDict.ContainsKey(g.FirstNode.Id))
                                    MappedDataDict[g.FirstNode.Id] = new MappedValue() { Count = 1, Title = g.FirstNode.Title };
                            }
                            else
                                MappedDataDict[-1].Count++;
                        break;

                    case "Подразделение":
                        MappedDataDict[-1] = new MappedValue() { Count = 0, Title = "Нет значения" };
                        foreach (Grant g in filteredGrants)
                            if (g.SecondNode != null)
                            {
                                if (!MappedDataDict.ContainsKey(g.SecondNode.Id))
                                    MappedDataDict[g.SecondNode.Id] = new MappedValue() { Count = 1, Title = g.SecondNode.Title };
                            }
                            else
                                MappedDataDict[-1].Count++;
                        break;

                    case "Отдел":
                        MappedDataDict[-1] = new MappedValue() { Count = 0, Title = "Нет значения" };
                        foreach (Grant g in filteredGrants)
                            if (g.ThirdNode != null)
                            {
                                if (!MappedDataDict.ContainsKey(g.ThirdNode.Id))
                                    MappedDataDict[-1] = new MappedValue() { Count = 1, Title = g.ThirdNode.Title };
                            }
                            else
                                MappedDataDict[g.ThirdNode.Id].Count++;
                        break;

                    case "Структурная единица":
                        MappedDataDict[-1] = new MappedValue() { Count = 0, Title = "Нет значения" };
                        foreach (Grant g in filteredGrants)
                            if (g.FourthNode != null)
                            {
                                if (!MappedDataDict.ContainsKey(g.FourthNode.Id))
                                    MappedDataDict[g.FourthNode.Id] = new MappedValue() { Count = 1, Title = g.FourthNode.Title };
                            }
                            else
                                MappedDataDict[-1].Count++;
                        break;

                    case "ГРНТИ":
                        foreach (Grant g in filteredGrants)
                            if (!stringDataDict.ContainsKey(g.GRNTI))
                                stringDataDict[g.GRNTI] = 1;
                            else
                                stringDataDict[g.GRNTI]++;
                        break;

                    case "Тип исследования":
                        foreach (Grant g in filteredGrants)
                            foreach (ResearchType r in g.ResearchType)
                                if (!MappedDataDict.ContainsKey(r.Id))
                                    MappedDataDict[r.Id] = new MappedValue() { Count = 1, Title = r.Title };
                                else
                                    MappedDataDict[r.Id].Count++;
                        break;

                    case "Приоритетные направления":
                        foreach (Grant g in filteredGrants)
                            foreach (PriorityTrend p in g.PriorityTrends)
                                if (!MappedDataDict.ContainsKey(p.Id))
                                    MappedDataDict[p.Id] = new MappedValue() { Count = 1, Title = p.Title };
                                else
                                    MappedDataDict[p.Id].Count++;
                        break;

                    case "Тип науки":
                        foreach (Grant g in filteredGrants)
                            foreach (ScienceType s in g.ScienceType)
                                if (!MappedDataDict.ContainsKey(s.Id))
                                    MappedDataDict[s.Id] = new MappedValue() { Count = 1, Title = s.Title };
                                else
                                    MappedDataDict[s.Id].Count++;
                        break;

                    case "НИР или УСЛУГА":
                        foreach (Grant g in filteredGrants)
                            if (!stringDataDict.ContainsKey(g.NIR))
                                stringDataDict[g.NIR] = 1;
                            else
                                stringDataDict[g.NIR]++;
                        break;

                    case "НОЦ":
                        foreach (Grant g in filteredGrants)
                            if (!stringDataDict.ContainsKey(g.NOC))
                                stringDataDict[g.NOC] = 1;
                            else
                                stringDataDict[g.NOC]++;
                        break;

                    case "Наличие НДС":
                        foreach (Grant g in filteredGrants)
                            if (!boolDataDict.ContainsKey(g.isWIthNDS))
                                boolDataDict[g.isWIthNDS] = 1;
                            else
                                boolDataDict[g.isWIthNDS]++;
                        break;
                }


                //List<string> dataList = filteredRows.Select(x => x[AggregationCountSelectedItem.Title].ToString()).ToList();
                //if (AggregationCountSelectedItem.IsMultiple)
                //{
                //    List<string> tempList = new List<string>();
                //    foreach (string s in dataList)
                //    {
                //        tempList.Concat(s.Split('\n').ToList());
                //    }
                //    dataList = tempList;
                //}

                //foreach (string s in dataList)
                //{
                //    if (!dataDict.ContainsKey(s))
                //    {
                //        dataDict[s] = 1;
                //    }
                //    else
                //    {
                //        dataDict[s]++;
                //    }
                //}
                AggregationForm aggregationWindow = new AggregationForm(AggregationCountSelectedItem.Title, boolDataDict, MappedDataDict, stringDataDict)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Owner = this
                };
                // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
                aggregationWindow.Closing += (senders, args) => { aggregationWindow.Owner = null; };
                aggregationWindow.Show();
            }
        }
    }
}
