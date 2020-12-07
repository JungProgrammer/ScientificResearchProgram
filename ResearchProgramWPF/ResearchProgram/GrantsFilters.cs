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
        // ГРНТИ
        public static ObservableCollection<FilterElement> GRNTI;
        // Тип исследования
        public static ObservableCollection<FilterElement> ResearchType;
        // Приоритетные направления
        public static ObservableCollection<FilterElement> PriorityTrands;
        // Исполнители по договору
        public static ObservableCollection<FilterElement> ExecutorContract;
        // Тип науки
        public static ObservableCollection<FilterElement> ScienceType;
        // НИР или услуга
        public static ObservableCollection<FilterElement> NIR;
        // НОЦ
        public static ObservableCollection<FilterElement> NOC;


        /// <summary>
        /// Обнуляет все фильтры
        /// </summary>
        public static void ResetFilters()
        {
            grantNumber = new ObservableCollection<FilterElement>();
            OKVED = null;
            NameNIOKR = new ObservableCollection<FilterElement>();
            Customer = new ObservableCollection<FilterElement>();
            StartDate = new ObservableCollection<FilterElement>();
            EndDate = new ObservableCollection<FilterElement>();
            Price = new ObservableCollection<FilterElement>();
            Depositor = new ObservableCollection<FilterElement>();
            LeadNIOKR = new ObservableCollection<FilterElement>();
            Executor = new ObservableCollection<FilterElement>();
            Kafedra = new ObservableCollection<FilterElement>();
            Unit = new ObservableCollection<FilterElement>();
            Institution = new ObservableCollection<FilterElement>();
            GRNTI = new ObservableCollection<FilterElement>();
            ResearchType = new ObservableCollection<FilterElement>();
            PriorityTrands = new ObservableCollection<FilterElement>();
            ExecutorContract = new ObservableCollection<FilterElement>();
            ScienceType = new ObservableCollection<FilterElement>();
            NIR = new ObservableCollection<FilterElement>();
            NOC = new ObservableCollection<FilterElement>();
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
            //___________________________________________________________________________________________________________СДЕЛАТЬ



            // Проверка ОКВЕД
            if (OKVED != null && IsAllOkey)
            {
                IsAllOkey = false;
                foreach (FilterElement okved in OKVED)
                {
                    if (okved.Data == grant.OKVED) IsAllOkey = true;
                }
            }





            return IsAllOkey;
        }
    }
}
