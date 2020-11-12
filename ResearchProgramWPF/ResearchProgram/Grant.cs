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
            Depositor = new List<string>();
            DepositorSum = new List<string>();
            Executor = new List<Person>();
            ResearchType = new List<string>();
            PriorityTrand = new List<string>();
            ExecutorContract = new List<Person>();
            ScienceType = new List<string>();
        }

        // Id
        public int Id { get; set; }
        // ОКВЕД
        public string OKVED { get; set; }
        // Наименование НИОКР
        public string NameNIOKR { get; set; }
        // Заказчик
        public Person Customer { get; set; }
        // Дата начала договора
        public DateTime StartDate { get; set; }
        // Дата окончания договора
        public DateTime EndDate { get; set; }
        // Цена договора
        public float Price { get; set; }
        // Средства
        public List<string> Depositor { get; set; }
        // Часть средств
        public List<string> DepositorSum { get; set; }
        // руководитель
        public Person LeadNIOKR { get; set; }
        // Исполнитель
        public List<Person> Executor { get; set; }
        // Кафедра
        public string Kafedra { get; set; }
        // Подразделение
        public string Unit { get; set; }
        // Учреждение
        public string Institution { get; set; }
        // ГРНТИ
        public string GRNTI { get; set; }
        // Тип исследования
        public List<string> ResearchType { get; set; }
        // Приоритетные направления
        public List<string> PriorityTrand { get; set; }
        // Исполнители по договору
        public List<Person> ExecutorContract { get; set; }
        // Тип науки
        public List<string> ScienceType { get; set; }
        // НИР или услуга
        public string NIR { get; set; }
        // НОЦ
        public string NOC { get; set; }
    }
}
