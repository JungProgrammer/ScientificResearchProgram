using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public static class GrantsFilters
    {
        // Номер договора
        public static ObservableCollection<FilterElement> grantNumber;
        // ОКВЕД
        public static ObservableCollection<FilterElement> OKVED;
        // Наименование НИОКР
        public static ObservableCollection<FilterElement> NameNIOKR;
        // Заказчик
        public static ObservableCollection<FilterElement> Customer;
        // Дата начала договора
        public static ObservableCollection<FilterElement> StartDate;
        // Дата окончания договора
        public static ObservableCollection<FilterElement> EndDate;
        // Цена договора
        public static ObservableCollection<FilterElement> Price;
        // Средства
        public static ObservableCollection<FilterElement> Depositor;
        // Дата начала поступлений средств
        public static FilterElement StartDepositDate;
        // Дата окончания поступлений средств
        public static FilterElement EndDepositDate;
        // руководитель
        public static ObservableCollection<FilterElement> LeadNIOKR;
        // Исполнитель
        public static ObservableCollection<FilterElement> Executor;
        // Кафедра
        public static ObservableCollection<FilterElement> Kafedra;
        // Подразделение
        public static ObservableCollection<FilterElement> Unit;
        // Учреждение
        public static ObservableCollection<FilterElement> Institution;
        // Лаборатория
        public static ObservableCollection<FilterElement> Laboratory;
        // ГРНТИ
        public static ObservableCollection<FilterElement> GRNTI;
        // Тип исследования
        public static ObservableCollection<FilterElement> ResearchTypes;
        // Приоритетные направления
        public static ObservableCollection<FilterElement> PriorityTrands;
        // Тип науки
        public static ObservableCollection<FilterElement> ScienceTypes;
        // НИР или услуга
        public static ObservableCollection<FilterElement> NIR;
        // НОЦ
        public static ObservableCollection<FilterElement> NOC;


        /// <summary>
        /// Обнуляет все фильтры
        /// </summary>
        public static void ResetFilters()
        {
            grantNumber = null;
            OKVED = null;
            NameNIOKR = null;
            Customer = null;
            StartDate = null;
            EndDate = null;
            Price = null;
            Depositor = null;
            LeadNIOKR = null;
            Executor = null;
            Kafedra = null;
            Unit = null;
            Institution = null;
            Laboratory = null;
            GRNTI = null;
            ResearchTypes = null;
            PriorityTrands = null;
            ScienceTypes = null;
            NIR = null;
            NOC = null;
        }


        /// <summary>
        /// Функция возвращает истину, если договор подходит под текущие фильтры
        /// </summary>
        /// <param name="grant"></param>
        /// <returns></returns>
        public static bool CheckGrantOnCurFilter(Grant grant)
        {
            bool IsAllOkey = true;

            // Проверка номера договора
            if (grantNumber != null && grantNumber.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;
                foreach (FilterElement _grantNumber in grantNumber)
                {
                    if (_grantNumber.Data == grant.grantNumber) IsAllOkey = true;
                }
            }


            // Проверка ОКВЕД
            if (OKVED != null && OKVED.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;
                foreach (FilterElement _okved in OKVED)
                {
                    if (_okved.Data == grant.OKVED) IsAllOkey = true;
                }
            }


            // Проверка наименования НИОКР
            if (NameNIOKR != null && NameNIOKR.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;
                foreach (FilterElement _nameNIOKR in NameNIOKR)
                {
                    if (_nameNIOKR.Data == grant.NameNIOKR) IsAllOkey = true;
                }
            }



            // Проверка заказчика
            if (Customer != null && Customer.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(Customer customer in grant.Customer)
                {
                    foreach(FilterElement customerFilter in Customer)
                    {
                        if (customer.Title == customerFilter.Data) IsAllOkey = true;
                    }
                }
            }


            // Проверка даты
            // Если указана дата начала
            if (StartDate != null && StartDate.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                // Если дата окончания не выбрана
                if (EndDate == null || EndDate.Count == 0)
                {
                    foreach (FilterElement date in StartDate)
                    {
                        if (grant.StartDate >= DateTime.Parse(date.Data))
                        {
                            IsAllOkey = true;
                        }
                    }
                }
                // Если выбрана и дата начала, и дата конца
                else
                {
                    foreach (FilterElement date in StartDate)
                    {
                        if (grant.StartDate >= DateTime.Parse(date.Data))
                        {
                            IsAllOkey = true;
                        }
                    }

                    if (IsAllOkey)
                    {
                        IsAllOkey = false;

                        foreach (FilterElement date in EndDate)
                        {
                            if (grant.EndDate <= DateTime.Parse(date.Data))
                            {
                                IsAllOkey = true;
                            }
                        }
                    }
                }
            }
            // Если указана только дата окончания
            else if(EndDate != null && EndDate.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(FilterElement date in EndDate)
                {
                    if(grant.EndDate <= DateTime.Parse(date.Data))
                    {
                        IsAllOkey = true;
                    }
                }
            }


            // Проверка общей суммы договора
            if(Price != null && Price.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(FilterElement price in Price)
                {
                    if (Utilities.ComparativeOperator(price.Sign, grant.Price.ToString(), price.Data)) IsAllOkey = true;
                }
            }


            // Проверка средств
            if (Depositor != null && Depositor.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(Depositor grantDepositor in grant.Depositor)
                {
                    foreach(FilterElement depositor in Depositor)
                    {
                        if (depositor.Data == grantDepositor.Title) IsAllOkey = true;
                    }
                }
            }


            // Проверка руководителя НИОКР
            if(LeadNIOKR != null && LeadNIOKR.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(FilterElement leadNIOKR in LeadNIOKR)
                {
                    if (leadNIOKR.Data == grant.LeadNIOKR.FIO) IsAllOkey = true;
                }
            }


            // Проверка исполнителей
            if(Executor != null && Executor.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach (Person executor in grant.Executor)
                {
                    foreach (FilterElement person in Executor)
                    {
                        if (person.Data == executor.FIO) IsAllOkey = true;
                    }
                }
            }

            // Проверка учерждения
            if (Institution != null && Institution.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach (FilterElement institution in Institution)
                {
                    if (institution.Data == grant.Institution.Title) IsAllOkey = true;
                }
            }

            // Проверка подразделений
            if (Unit != null && Unit.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach (FilterElement unit in Unit)
                {
                    if (unit.Data == grant.Unit.Title) IsAllOkey = true;
                }
            }

            // Проверка кафедр
            if (Kafedra != null && Kafedra.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach (FilterElement kafedra in Kafedra)
                {
                    if (kafedra.Data == grant.Kafedra.Title) IsAllOkey = true;
                }
            }

            // Проверка лабораторий
            if(Laboratory != null && Laboratory.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(FilterElement laboratory in Laboratory)
                {
                    if (laboratory.Data == grant.Laboratory.Title) IsAllOkey = true;
                }
            }



            // Проверка ГРНТИ
            if (GRNTI != null && GRNTI.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(FilterElement grnti in GRNTI)
                {
                    if (grnti.Data == grant.GRNTI) IsAllOkey = true;
                }
            }


            // Проверка типов исследования
            if(ResearchTypes != null && ResearchTypes.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(ResearchType researchType in grant.ResearchType)
                {
                    foreach(FilterElement _researchType in ResearchTypes)
                    {
                        if (_researchType.Data == researchType.Title) IsAllOkey = true;
                    }
                }
            }


            // Проверка приоритетных направлений
            if(PriorityTrands != null && PriorityTrands.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(PriorityTrend priorityTrend in grant.PriorityTrands)
                {
                    foreach (FilterElement _priorityTrend in PriorityTrands)
                    {
                        if (_priorityTrend.Data == priorityTrend.Title) IsAllOkey = true;
                    }
                }
            }


            // Проверка типов наук
            if (ScienceTypes != null && ScienceTypes.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;
                
                foreach(ScienceType scienceType in grant.ScienceType)
                {
                    foreach(FilterElement _scienceType in ScienceTypes)
                    {
                        if (_scienceType.Data == scienceType.Title) IsAllOkey = true;
                    }
                }
            }


            // Проверка Нир или Услуга
            if (NIR != null && NIR.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(FilterElement nir in NIR)
                {
                    if (nir.Data == grant.NIR) IsAllOkey = true;
                }
            }


            // Проверка НОЦ
            if (NOC != null && NOC.Count > 0 && IsAllOkey)
            {
                IsAllOkey = false;

                foreach(FilterElement noc in NOC)
                {
                    if (noc.Data == grant.NOC) IsAllOkey = true;
                }
            }


            return IsAllOkey;
        }

        public static bool CheckReceiptDate(string receiptDate)
        {
            DateTime convertedReceiptDate;
            DateTime startDepositDate;
            DateTime endDepositDate;

            DateTime.TryParse(receiptDate, out convertedReceiptDate);
            DateTime.TryParse(StartDepositDate.Data, out startDepositDate);
            DateTime.TryParse(EndDepositDate.Data, out endDepositDate);

            if (convertedReceiptDate >= startDepositDate && convertedReceiptDate <= endDepositDate)
            {
                return true;
            }
            else return false;
        }
    }
}
