using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ResearchProgram
{
    public static class GrantsFilters
    {

        // ОКВЕД
        public static ObservableCollection<OKVED> OKVED;
        // ГРНТИ
        public static string GRNTI;
        // Номер договора
        public static string grantNumber;
        // руководитель
        public static ObservableCollection<Person> LeadNIOKR;
        // Наименование НИОКР
        public static string NameNIOKR;
        // НИР или услуга
        public static string NIR;
        // НОЦ
        public static bool? NOC;
        // Дата начала договора
        public static DateTime? StartDate;
        // Дата окончания договора
        public static DateTime? EndDate;

        // Заказчик
        public static ObservableCollection<Customer> Customer;
        // Исполнитель
        public static ObservableCollection<Person> Executor;

        // Без НДС
        public static bool? IsNoNDS;
        // Цена договора
        public static FilterRange Price;
        // Средства
        public static ObservableCollection<FilterRange> Depositors;
        // Кафедра
        public static ObservableCollection<UniversityStructureNode> FirstNode;
        // Подразделение
        public static ObservableCollection<UniversityStructureNode> SecondNode;
        // Учреждение
        public static ObservableCollection<UniversityStructureNode> ThirdNode;
        // Лаборатория
        public static ObservableCollection<UniversityStructureNode> FourthNode;

        // Тип исследования
        public static ObservableCollection<ResearchType> ResearchType;
        // Приоритетные направления
        public static ObservableCollection<PriorityTrend> PriorityTrend;
        // Тип науки
        public static ObservableCollection<ScienceType> ScienceType;



        ///// <summary>
        ///// Обнуляет все фильтры
        ///// </summary>
        public static void ResetFilters()
        {
            grantNumber = null;
            OKVED = null;
            NameNIOKR = null;
            Customer = null;
            StartDate = null;
            EndDate = null;
            Price = null;
            IsNoNDS = null;
            Depositors = null;
            LeadNIOKR = null;
            Executor = null;
            FirstNode = null;
            SecondNode = null;
            ThirdNode = null;
            FourthNode = null;
            GRNTI = null;
            ResearchType = null;
            PriorityTrend = null;
            ScienceType = null;
            NIR = null;
            NOC = null;
        }

        /// <summary>
        /// Проверка, есть ли в фильтре хоть одно заполненное значение
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            Type thisType = typeof(GrantsFilters);
            FieldInfo[] Field = thisType.GetFields();
            for (int i = 0; i < Field.Length; i++)
            {
                if (Field[i].GetValue(null) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetSignLiteral(FilterRange.Signs sign)
        {
            switch (sign)
            {
                case FilterRange.Signs.More:
                    return " > ";
                case FilterRange.Signs.MoreEqual:
                    return " >= ";
                case FilterRange.Signs.Equal:
                    return " = ";
                case FilterRange.Signs.Less:
                    return " < ";
                case FilterRange.Signs.LessEqual:
                    return " <= ";
                default:
                    return " = ";
            }
        }

        public static string GetResearchTypesQuarry()
        {
            string quarry = "SELECT grantId FROM grantResearchType ";
            if (ResearchType.Count > 0)
            {
                quarry += " WHERE researchtypeid = " + string.Join(" OR researchtypeid = ", ResearchType.Select(x => x.Id).ToArray());
            }

            quarry += " ORDER BY grantId; ";
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetPriorityTrendsQuarry()
        {
            string quarry = "SELECT grantId FROM grantPriorityTrends ";
            if (PriorityTrend.Count > 0)
            {
                quarry += " WHERE prioritytrendsid = " + string.Join(" OR prioritytrendsid = ", PriorityTrend.Select(x => x.Id).ToArray());
            }

            quarry += " ORDER BY grantId; ";
            Console.WriteLine(quarry);
            return quarry;
        }
        public static string GetScienceTypesQuarry()
        {
            string quarry = "SELECT grantId FROM grantScienceTypes ";
            if (ScienceType.Count > 0)
            {
                quarry += " WHERE sciencetypesid = " + string.Join(" OR sciencetypesid = ", ScienceType.Select(x => x.Id).ToArray());
            }

            quarry += " ORDER BY grantId; ";
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetDepositorQuarryByIndex(int index)
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (Depositors != null)
            {
                if (Depositors[index] != null)
                {
                    bool isCondition = false;
                    quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose[index.ToString()] + "' ";
                    if (Depositors[index].LeftDate != null)
                    {
                        quarry += " AND receiptdate " + GetSignLiteral(Depositors[index].LeftDateSign) + "'" + Depositors[index].LeftDate?.ToString("yyyy-MM-dd") + "'";
                    }
                    if (Depositors[index].RightDate != null)
                    {
                        quarry += " AND receiptdate " + GetSignLiteral(Depositors[index].RightDateSign) + "'" + Depositors[index].RightDate?.ToString("yyyy-MM-dd") + "'";
                    }
                    isCondition = false;
                    quarry += " GROUP BY grantId ";
                    if (Depositors[index].LeftValue != null)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(Depositors[index].LeftSign) + Depositors[index].LeftValue.ToString();
                        isCondition = true;
                    }
                    if (Depositors[index].RightValue != null)
                    {
                        if (!isCondition)
                        {
                            quarry += " HAVING sum(partsum) " + GetSignLiteral(Depositors[index].RightSign) + Depositors[index].RightValue.ToString();
                        }
                        else
                        {
                            quarry += " AND sum(partsum) " + GetSignLiteral(Depositors[index].RightSign) + Depositors[index].RightValue.ToString();
                        }
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }
        public static string GetCustomersQuarry()
        {
            string quarry = "SELECT grant_id FROM grants_customers ";
            if (Customer.Count > 0)
            {
                quarry += " WHERE customer_id = " + string.Join(" OR customer_id = ", Customer.Select(x => x.Id).ToArray());
            }

            quarry += " ORDER BY grant_id; ";
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetExecutorsQuarry()
        {
            string quarry = "SELECT grantId FROM executors ";
            if (Executor.Count > 0)
            {
                quarry += " WHERE executorid = " + string.Join(" OR executorid = ", Executor.Select(x => x.Id).ToArray());
            }

            quarry += " ORDER BY grantId; ";
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetGrantQuarry()
        {
            string quarry = "SELECT id FROM grants ";

            List<string> quarryList = new List<string>();

            string tempQuarry;
            if (OKVED != null)
            {
                tempQuarry = "( ";

                for (int i = 0; i < OKVED.Count; i++)
                {
                    tempQuarry += " okved LIKE '%" + OKVED[i].Title + "%' ";
                    if (i != OKVED.Count - 1)
                    {
                        tempQuarry += " OR ";
                    }
                }
                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (GRNTI != null)
            {
                quarryList.Add("( grnti LIKE '%" + GRNTI + "%' )");
            }

            if (grantNumber != null)
            {
                quarryList.Add("( grantnumber LIKE '%" + grantNumber + "%' )");
            }

            if (LeadNIOKR != null)
            {
                tempQuarry = "( ";
                tempQuarry += " leadniokrid = " + string.Join(" OR leadniokrid = ", LeadNIOKR.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (NameNIOKR != null)
            {
                quarryList.Add("( nameniokr LIKE '%" + NameNIOKR + "%' )");
            }

            if (NIR != null)
            {
                quarryList.Add("( nir LIKE '%" + NIR + "%' )");
            }

            if (NOC != null)
            {
                if ((bool)NOC)
                {
                    quarryList.Add("( noc = true) ");
                }
                else
                {
                    quarryList.Add("( noc = false) ");
                }
            }

            if (StartDate != null)
            {
                quarryList.Add("( startdate >= '" + StartDate?.ToString("yyyy-MM-dd") + "') ");
            }

            if (EndDate != null)
            {
                quarryList.Add("( enddate <= '" + EndDate?.ToString("yyyy-MM-dd") + "') ");
            }

            if (IsNoNDS != null)
            {
                if ((bool)IsNoNDS)
                {
                    quarryList.Add("( is_with_nds = false )");
                }
                else
                {
                    quarryList.Add("( is_with_nds = true )");
                }
            }

            if (Price != null)
            {
                //if(IsNoNDS != null)
                //{
                //    if ((bool)IsNoNDS)
                //    {
                //        if (Price.LeftValue != null)
                //        {
                //            quarryList.Add("( pricenonds " + GetSignLiteral(Price.LeftSign) + " " + Price.LeftValue + ") ");
                //        }
                //        if (Price.RightValue != null)
                //        {
                //            quarryList.Add("( pricenonds " + GetSignLiteral(Price.RightSign) + " " + Price.RightValue + ") ");
                //        }
                //    }
                //    else
                //    {
                //        if (Price.LeftValue != null)
                //        {
                //            quarryList.Add("( price " + GetSignLiteral(Price.LeftSign) + " " + Price.LeftValue + ") ");
                //        }
                //        if (Price.RightValue != null)
                //        {
                //            quarryList.Add("( price " + GetSignLiteral(Price.RightSign) + " " + Price.RightValue + ") ");
                //        }
                //    }
                //}
                //else
                //{
                if (Price.LeftValue != null)
                {
                    quarryList.Add("( price " + GetSignLiteral(Price.LeftSign) + " " + Price.LeftValue + ") ");
                }
                if (Price.RightValue != null)
                {
                    quarryList.Add("( price " + GetSignLiteral(Price.RightSign) + " " + Price.RightValue + ") ");
                }
                //}

            }

            if (FirstNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " first_node_id = " + string.Join(" OR first_node_id = ", FirstNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (SecondNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " second_node_id = " + string.Join(" OR second_node_id = ", SecondNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (ThirdNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " third_node_id = " + string.Join(" OR third_node_id = ", ThirdNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }

            if (FourthNode != null)
            {
                tempQuarry = "( ";
                tempQuarry += " fourth_node_id = " + string.Join(" OR fourth_node_id = ", FourthNode.Select(x => x.Id).ToArray());

                tempQuarry += ") ";
                quarryList.Add(tempQuarry);
            }


            if (quarryList.Count > 0)
            {
                quarry += " WHERE " + string.Join(" AND ", quarryList);
            }
            quarry += " ORDER BY id; ";

            Console.WriteLine(quarry);
            return quarry;
        }
    }
}
