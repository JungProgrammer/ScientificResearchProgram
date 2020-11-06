using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Npgsql;

namespace ResearchProgram
{
    public class Grant
    {
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
        public string Depositor { get; set; }
        // руководитель
        public string LeadNIOKR { get; set; }
        // Исполнитель
        public string Executor { get; set; }
        // Кафедра
        public string Kafedra { get; set; }
        // Подразделение
        public string Unit { get; set; }
        // ГРНТИ
        public string GRNTI { get; set; }
        // Тип исследования
        public string ResearchType { get; set; }
        // Приоритетные направления
        public string PriorityTrand { get; set; }
        // Исполнители по договору
        public string ExecutorContract { get; set; }
        // Тип науки
        public string ScienceType { get; set; }
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



    public static class CRUDDataBase
    {
        public static string loginFromDB = Environment.GetEnvironmentVariable("PGUSER");
        public static string passwordFromDB = Environment.GetEnvironmentVariable("PGPASSWORD");

        private static NpgsqlConnection conn;

        public static void ConnectByDataBase()
        {
            conn = new NpgsqlConnection($"Server=localhost; Port=5432; User Id={loginFromDB}; Password={passwordFromDB}; Database=postgres");
            conn.Open();
        }

        public static void LoadTable(DataTable dataTable)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, OKVED, nameNIOKR, customerId, startDate, endDate, price, id, OKVED, nameNIOKR, customerId, startDate, endDate, price, leadNIOKRId, kafedraId, unitId, institutionId, GRNTI, NIR, NOC FROM grants;", conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();


            dataTable.Columns.Add("First");
            dataTable.Columns.Add("Second");
            while (dr.Read())
            {

                dataTable.Rows.Add("11", "12");
                dataTable.Rows.Add("21", "22");
            }
        }

        public static void CloseConnect()
        {
            conn.Close();
        }
    }
}
