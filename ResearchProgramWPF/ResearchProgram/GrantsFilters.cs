using ResearchProgram.Classes;
using System;
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
        // Иностранные Средства
        public static FilterRange FirstDepositor;
        // Собственные средства
        public static FilterRange SecondDepositor;
        // Средства бюджета субъекта Федерации
        public static FilterRange ThirdDepositor;
        // Средства Российских фондов поддержки науки
        public static FilterRange FourthDepositor;
        // Средства хозяйствующих субъектов
        public static FilterRange FifthDepositor;
        // Физ. лица
        public static FilterRange SixthDepositor;
        // ФЦП мин обра или иные источники госзаказа(бюджет)
        public static FilterRange SeventhDepositor;

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
            FirstDepositor = null;
            SecondDepositor = null;
            ThirdDepositor = null;
            FourthDepositor = null;
            FifthDepositor = null;
            SixthDepositor = null;
            SeventhDepositor = null;
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
            string quarry = "SELECT grantId FROM grantResearchType " +
                "JOIN researchTypes rT on grantResearchType.researchTypeId = rT.id ";
            if (ResearchType.Count > 0)
            {
                quarry += " WHERE rT.id = " + string.Join(" OR rT.id = ", ResearchType.Select(x => x.Id).ToArray());
                quarry.Replace("WHERE OR", "WHERE");
            }

            quarry += " ORDER BY grantId; ";
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetPriorityTrendsQuarry()
        {
            string quarry = "SELECT grantId FROM grantPriorityTrends " +
                "JOIN priorityTrends pT on grantPriorityTrends.priorityTrendsId = pT.id ";
            if (PriorityTrend.Count > 0)
            {
                quarry += " WHERE pT.id = " + string.Join(" OR pT.id = ", PriorityTrend.Select(x => x.Id).ToArray());
                quarry.Replace("WHERE OR", "WHERE");
            }

            quarry += " ORDER BY grantId; ";
            Console.WriteLine(quarry);
            return quarry;
        }
        public static string GetScienceTypesQuarry()
        {
            string quarry = "SELECT grantId FROM grantScienceTypes " +
                "JOIN scienceTypes sT on grantScienceTypes.scienceTypesId = sT.id ";
            if (ScienceType.Count > 0)
            {
                quarry += " WHERE sT.id = " + string.Join(" OR sT.id = ", ScienceType.Select(x => x.Id).ToArray());
                quarry.Replace("WHERE OR", "WHERE");
            }

            quarry += " ORDER BY grantId; ";
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetFirstDepositorsQuarry()
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (FirstDepositor != null)
            {
                bool isCondition = false;
                quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose["first"] + "' ";
                if (FirstDepositor.LeftDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(FirstDepositor.LeftDateSign) + "'" + FirstDepositor.LeftDate?.ToString("yyyy-MM-dd") + "'";
                }
                if (FirstDepositor.RightDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(FirstDepositor.RightDateSign) + "'" + FirstDepositor.RightDate?.ToString("yyyy-MM-dd") + "'";
                }
                isCondition = false;
                quarry += " GROUP BY grantId ";
                if (FirstDepositor.LeftValue != null)
                {
                    quarry += " HAVING sum(partsum) " + GetSignLiteral(FirstDepositor.LeftSign) + FirstDepositor.LeftValue.ToString();
                    isCondition = true;
                }
                if (FirstDepositor.RightValue != null)
                {
                    if (!isCondition)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(FirstDepositor.RightSign) + FirstDepositor.RightValue.ToString();
                    }
                    else
                    {
                        quarry += " AND sum(partsum) " + GetSignLiteral(FirstDepositor.RightSign) + FirstDepositor.RightValue.ToString();
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetSecondDepositorsQuarry()
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (SecondDepositor != null)
            {
                bool isCondition = false;
                quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose["second"] + "' ";
                if (SecondDepositor.LeftDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(SecondDepositor.LeftDateSign) + "'" + SecondDepositor.LeftDate?.ToString("yyyy-MM-dd") + "'";
                }
                if (SecondDepositor.RightDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(SecondDepositor.RightDateSign) + "'" + SecondDepositor.RightDate?.ToString("yyyy-MM-dd") + "'";
                }
                isCondition = false;
                quarry += " GROUP BY grantId ";
                if (SecondDepositor.LeftValue != null)
                {
                    quarry += " HAVING sum(partsum) " + GetSignLiteral(SecondDepositor.LeftSign) + SecondDepositor.LeftValue.ToString();
                    isCondition = true;
                }
                if (SecondDepositor.RightValue != null)
                {
                    if (!isCondition)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(SecondDepositor.RightSign) + SecondDepositor.RightValue.ToString();
                    }
                    else
                    {
                        quarry += " AND sum(partsum) " + GetSignLiteral(SecondDepositor.RightSign) + SecondDepositor.RightValue.ToString();
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetThirdDepositorsQuarry()
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (ThirdDepositor != null)
            {
                bool isCondition = false;
                quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose["third"] + "' ";
                if (ThirdDepositor.LeftDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(ThirdDepositor.LeftDateSign) + "'" + ThirdDepositor.LeftDate?.ToString("yyyy-MM-dd") + "'";
                }
                if (ThirdDepositor.RightDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(ThirdDepositor.RightDateSign) + "'" + ThirdDepositor.RightDate?.ToString("yyyy-MM-dd") + "'";
                }
                isCondition = false;
                quarry += " GROUP BY grantId ";
                if (ThirdDepositor.LeftValue != null)
                {
                    quarry += " HAVING sum(partsum) " + GetSignLiteral(ThirdDepositor.LeftSign) + ThirdDepositor.LeftValue.ToString();
                    isCondition = true;
                }
                if (ThirdDepositor.RightValue != null)
                {
                    if (!isCondition)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(ThirdDepositor.RightSign) + ThirdDepositor.RightValue.ToString();
                    }
                    else
                    {
                        quarry += " AND sum(partsum) " + GetSignLiteral(ThirdDepositor.RightSign) + ThirdDepositor.RightValue.ToString();
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetFourthDepositorsQuarry()
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (FourthDepositor != null)
            {
                bool isCondition = false;
                quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose["fourth"] + "' ";
                if (FourthDepositor.LeftDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(FourthDepositor.LeftDateSign) + "'" + FourthDepositor.LeftDate?.ToString("yyyy-MM-dd") + "'";
                }
                if (FourthDepositor.RightDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(FourthDepositor.RightDateSign) + "'" + FourthDepositor.RightDate?.ToString("yyyy-MM-dd") + "'";
                }
                isCondition = false;
                quarry += " GROUP BY grantId ";
                if (FourthDepositor.LeftValue != null)
                {
                    quarry += " HAVING sum(partsum) " + GetSignLiteral(FourthDepositor.LeftSign) + FourthDepositor.LeftValue.ToString();
                    isCondition = true;
                }
                if (FourthDepositor.RightValue != null)
                {
                    if (!isCondition)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(FourthDepositor.RightSign) + FourthDepositor.RightValue.ToString();
                    }
                    else
                    {
                        quarry += " AND sum(partsum) " + GetSignLiteral(FourthDepositor.RightSign) + FourthDepositor.RightValue.ToString();
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetFifthDepositorsQuarry()
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (FifthDepositor != null)
            {
                bool isCondition = false;
                quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose["fifth"] + "' ";
                if (FifthDepositor.LeftDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(FifthDepositor.LeftDateSign) + "'" + FifthDepositor.LeftDate?.ToString("yyyy-MM-dd") + "'";
                }
                if (FifthDepositor.RightDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(FifthDepositor.RightDateSign) + "'" + FifthDepositor.RightDate?.ToString("yyyy-MM-dd") + "'";
                }
                isCondition = false;
                quarry += " GROUP BY grantId ";
                if (FifthDepositor.LeftValue != null)
                {
                    quarry += " HAVING sum(partsum) " + GetSignLiteral(FifthDepositor.LeftSign) + FifthDepositor.LeftValue.ToString();
                    isCondition = true;
                }
                if (FifthDepositor.RightValue != null)
                {
                    if (!isCondition)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(FifthDepositor.RightSign) + FifthDepositor.RightValue.ToString();
                    }
                    else
                    {
                        quarry += " AND sum(partsum) " + GetSignLiteral(FifthDepositor.RightSign) + FifthDepositor.RightValue.ToString();
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetSixthDepositorsQuarry()
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (SixthDepositor != null)
            {
                bool isCondition = false;
                quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose["sixth"] + "' ";
                if (SixthDepositor.LeftDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(SixthDepositor.LeftDateSign) + "'" + SixthDepositor.LeftDate?.ToString("yyyy-MM-dd") + "'";
                }
                if (SixthDepositor.RightDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(SixthDepositor.RightDateSign) + "'" + SixthDepositor.RightDate?.ToString("yyyy-MM-dd") + "'";
                }
                isCondition = false;
                quarry += " GROUP BY grantId ";
                if (SixthDepositor.LeftValue != null)
                {
                    quarry += " HAVING sum(partsum) " + GetSignLiteral(SixthDepositor.LeftSign) + SixthDepositor.LeftValue.ToString();
                    isCondition = true;
                }
                if (SixthDepositor.RightValue != null)
                {
                    if (!isCondition)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(SixthDepositor.RightSign) + SixthDepositor.RightValue.ToString();
                    }
                    else
                    {
                        quarry += " AND sum(partsum) " + GetSignLiteral(SixthDepositor.RightSign) + SixthDepositor.RightValue.ToString();
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }

        public static string GetSeventhDepositorsQuarry()
        {
            string quarry = "SELECT DISTINCT grantId FROM grantDeposits " +
                "JOIN depositors d on grantDeposits.sourceId = d.id ";
            if (SeventhDepositor != null)
            {
                bool isCondition = false;
                quarry += "WHERE d.title = '" + StaticDataTemp.DepositsVerbose["seventh"] + "' ";
                if (SeventhDepositor.LeftDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(SeventhDepositor.LeftDateSign) + "'" + SeventhDepositor.LeftDate?.ToString("yyyy-MM-dd") + "'";
                }
                if (SeventhDepositor.RightDate != null)
                {
                    quarry += " AND receiptdate " + GetSignLiteral(SeventhDepositor.RightDateSign) + "'" + SeventhDepositor.RightDate?.ToString("yyyy-MM-dd") + "'";
                }
                isCondition = false;
                quarry += " GROUP BY grantId ";
                if (SeventhDepositor.LeftValue != null)
                {
                    quarry += " HAVING sum(partsum) " + GetSignLiteral(SeventhDepositor.LeftSign) + SeventhDepositor.LeftValue.ToString();
                    isCondition = true;
                }
                if (SeventhDepositor.RightValue != null)
                {
                    if (!isCondition)
                    {
                        quarry += " HAVING sum(partsum) " + GetSignLiteral(SeventhDepositor.RightSign) + SeventhDepositor.RightValue.ToString();
                    }
                    else
                    {
                        quarry += " AND sum(partsum) " + GetSignLiteral(SeventhDepositor.RightSign) + SeventhDepositor.RightValue.ToString();
                    }
                }
            }
            Console.WriteLine(quarry);
            return quarry;
        }


    }
}
