using ResearchProgram.Classes;
using System;
using System.Collections.Generic;

namespace ResearchProgram
{
    public class Grant
    {
        public Grant()
        {
            Depositors = new List<GrantDepositor>();
            Executor = new List<Person>();
            ResearchType = new List<ResearchType>();
            PriorityTrends = new List<PriorityTrend>();
            ScienceType = new List<ScienceType>();

            Id = 0;
            Price = 0;
            PriceNoNDS = 0;
            LeadNIOKR = new Person();
            Customer = new List<Customer>();
        }

        // Id
        public int Id { get; set; }
        // Номер договора
        public string grantNumber { get; set; }
        // ОКВЕД
        public string OKVED { get; set; }
        // Наименование НИОКР
        public string NameNIOKR { get; set; }
        // Заказчик
        public List<Customer> Customer { get; set; }
        // Дата начала договора
        public DateTime? StartDate { get; set; }
        // Дата окончания договора
        public DateTime? EndDate { get; set; }
        // Цена договора
        public double Price { get; set; }
        // Цена договора без НДС
        public double PriceNoNDS { get; set; }
        // Средства
        public List<GrantDepositor> Depositors { get; set; }
        // руководитель
        public Person LeadNIOKR { get; set; }
        // Исполнитель
        public List<Person> Executor { get; set; }
        // ГРНТИ
        public string GRNTI { get; set; }
        // Тип исследования
        public List<ResearchType> ResearchType { get; set; }
        // Приоритетные направления
        public List<PriorityTrend> PriorityTrends { get; set; }
        // Тип науки
        public List<ScienceType> ScienceType { get; set; }
        // НИР или услуга
        public string NIR { get; set; }
        // НОЦ
        public string NOC { get; set; }
        //Есть ли у договора НДС
        public bool isWIthNDS { get; set; }

        public UniversityStructureNode FirstNode { get; set; }
        public UniversityStructureNode SecondNode { get; set; }
        public UniversityStructureNode ThirdNode { get; set; }
        public UniversityStructureNode FourthNode { get; set; }
    }
}
