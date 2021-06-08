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
            foreach (TableHeader s in headers.Where(x => x.IsService == false && x.IsCountable == true))
            {
                AggregationCountSource.Add(s);
            }
            AggregationCountComboBox.SelectedIndex = 0;

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
            }
            else if (WhatToCountComboBox.SelectedIndex == 1)
            {
                Dictionary<string, int> stringDataDict = new Dictionary<string, int>();
                Dictionary<int, int> intDataDict = new Dictionary<int, int>();
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
                                if (!intDataDict.ContainsKey(c.Id))
                                    intDataDict[c.Id] = 1;
                                else
                                    intDataDict[c.Id]++;
                        break;

                    case "Источник финансирования":
                        foreach (Grant g in filteredGrants)
                            foreach (Depositor d in g.Depositor)
                                if (!intDataDict.ContainsKey(d.Id))
                                    intDataDict[d.Id] = 1;
                                else
                                    intDataDict[d.Id]++;
                        break;

                    case "Руководитель НИОКР":
                        foreach (Grant g in filteredGrants)
                            if (g.LeadNIOKR != null)
                            {
                                if (!intDataDict.ContainsKey(g.LeadNIOKR.Id))
                                    intDataDict[g.LeadNIOKR.Id] = 1;
                            }
                            else
                                intDataDict[-1]++;
                        break;

                    case "Исполнители":
                        foreach (Grant g in filteredGrants)
                            foreach (Person ex in g.Executor)
                                if (!intDataDict.ContainsKey(ex.Id))
                                    intDataDict[ex.Id] = 1;
                                else
                                    intDataDict[ex.Id]++;
                        break;

                    case "Учреждение":
                        foreach (Grant g in filteredGrants)
                            if (g.FirstNode != null)
                            {
                                if (!intDataDict.ContainsKey(g.FirstNode.Id))
                                    intDataDict[g.FirstNode.Id] = 1;
                            }
                            else
                                intDataDict[-1]++;
                        break;

                    case "Подразделение":
                        foreach (Grant g in filteredGrants)
                            if (g.SecondNode != null)
                            {
                                if (!intDataDict.ContainsKey(g.SecondNode.Id))
                                    intDataDict[g.SecondNode.Id] = 1;
                            }
                            else
                                intDataDict[-1]++;
                        break;

                    case "Отдел":
                        foreach (Grant g in filteredGrants)
                            if (g.ThirdNode != null)
                            {
                                if (!intDataDict.ContainsKey(g.ThirdNode.Id))
                                    intDataDict[-1] = 1;
                            }
                            else
                                intDataDict[g.ThirdNode.Id]++;
                        break;

                    case "Структурная единица":
                        foreach (Grant g in filteredGrants)
                            if (g.FourthNode != null)
                            {
                                if (!intDataDict.ContainsKey(g.FourthNode.Id))
                                    intDataDict[g.FourthNode.Id] = 1;
                            }
                            else
                                intDataDict[-1]++;
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
                                if (!intDataDict.ContainsKey(r.Id))
                                    intDataDict[r.Id] = 1;
                                else
                                    intDataDict[r.Id]++;
                        break;

                    case "Приоритетные направления":
                        foreach (Grant g in filteredGrants)
                            foreach (PriorityTrend p in g.PriorityTrands)
                                if (!intDataDict.ContainsKey(p.Id))
                                    intDataDict[p.Id] = 1;
                                else
                                    intDataDict[p.Id]++;
                        break;

                    case "Тип науки":
                        foreach (Grant g in filteredGrants)
                            foreach (ScienceType s in g.ScienceType)
                                if (!intDataDict.ContainsKey(s.Id))
                                    intDataDict[s.Id] = 1;
                                else
                                    intDataDict[s.Id]++;
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

                Console.WriteLine("aboba");
            }
        }
    }
}
