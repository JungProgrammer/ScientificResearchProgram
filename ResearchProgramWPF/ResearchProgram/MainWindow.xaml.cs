using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace ResearchProgram
{
    public class Grant
    {
        public Grant()
        {
            Depositor = new List<string>();
            DepositorSum = new List<string>();
            Executor = new List<string>();
            ResearchType = new List<string>();
            PriorityTrand = new List<string>();
            ExecutorContract = new List<string>();
            ScienceType = new List<string>();
        }

        // Id
        public int Id { get; set; }
        // ОКВЕД
        public string OKVED { get; set; }
        // Наименование НИОКР
        public string NameNIOKR { get; set; }
        // Заказчик
        public string Customer { get; set; }
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
        public string LeadNIOKR { get; set; }
        // Исполнитель
        public List<string> Executor { get; set; }
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
        public List<string> ExecutorContract { get; set; }
        // Тип науки
        public List<string> ScienceType { get; set; }
        // НИР или услуга
        public string NIR { get; set; }
        // НОЦ
        public string NOC { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataTable GrantsDataTable { get; private set; }
        public DataTable PeopleDataTable { get; private set; }

        public MainWindow()
        {

            InitializeComponent();

            createGrantWindow w = new createGrantWindow();
            w.Show();

            // Загружаем данные в таблицу грантов
            LoadGrantsTable();
            // Загружаем данные в таблицу людей
            LoadPeopleTable();

            DataContext = this;
        }

        /// <summary>
        /// Загрузка данных в таблицу договоров
        /// </summary>
        private void LoadGrantsTable()
        {
            var ds = new DataSet("Grants");
            this.GrantsDataTable = ds.Tables.Add("GrantsTable");

            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.CreateHeaders(GrantsDataTable);
            CRUDDataBase.LoadTable(GrantsDataTable);
            CRUDDataBase.CloseConnect();
        }

        /// <summary>
        /// Загрузка данных в таблицу людей
        /// </summary>
        private void LoadPeopleTable()
        {
            var ds = new DataSet("Grants");
            this.PeopleDataTable = ds.Tables.Add("PeopleTable");
            this.PeopleDataTable.Columns.Add("First");
            this.PeopleDataTable.Columns.Add("Second");

            this.PeopleDataTable.Rows.Add("11", "12");
            this.PeopleDataTable.Rows.Add("21", "22");
        }

    }

}
