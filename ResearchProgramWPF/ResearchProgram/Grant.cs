using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Grant
    {
        public Grant()
        {
            Depositor = new List<Depositor>();
            DepositorSum = new List<float>();
            Executor = new List<Person>();
            ResearchType = new List<ResearchType>();
            PriorityTrands = new List<PriorityTrend>();
            ScienceType = new List<ScienceType>();

            Id = 0;
            //grantNumber = "Не указан";
            //OKVED = "Не указан";
            //NameNIOKR = "Не указан";
            Price = 0;
            Institution = new Institution();
            Unit = new Unit();
            Kafedra = new Kafedra();
            Laboratory = new Laboratory();
            //GRNTI = "Не указан";
            //NIR = "Не указан";
            //NOC = "Не указан";
            LeadNIOKR = new Person();
            Customer = new Customer();  
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
        public Customer Customer { get; set; }
        // Дата начала договора
        public DateTime StartDate { get; set; }
        // Дата окончания договора
        public DateTime EndDate { get; set; }
        // Цена договора
        public float Price { get; set; }
        // Средства
        public List<Depositor> Depositor { get; set; }
        // Часть средств
        public List<float> DepositorSum { get; set; }
        // руководитель
        public Person LeadNIOKR { get; set; }
        // Исполнитель
        public List<Person> Executor { get; set; }
        // Учреждение
        public Institution Institution { get; set; }
        // Подразделение
        public Unit Unit { get; set; }
        // Кафедра
        public Kafedra Kafedra { get; set; }
        // Лаборатория 
        public Laboratory Laboratory { get; set; }
        // ГРНТИ
        public string GRNTI { get; set; }
        // Тип исследования
        public List<ResearchType> ResearchType { get; set; }
        // Приоритетные направления
        public List<PriorityTrend> PriorityTrands { get; set; }
        // Тип науки
        public List<ScienceType> ScienceType { get; set; }
        // НИР или услуга
        public string NIR { get; set; }
        // НОЦ
        public string NOC { get; set; }
    }
}
