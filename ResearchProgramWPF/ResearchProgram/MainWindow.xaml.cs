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

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Таблица договоров
        public DataTable GrantsDataTable { get; set; }
        public DataTable PeopleDataTable { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

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
            GrantsDataTable = ds.Tables.Add("GrantsTable");

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

        private void CreateGrantMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createGrantWindow newGrantWindow = new createGrantWindow(GrantsDataTable);
            newGrantWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newGrantWindow.Owner = this;

            newGrantWindow.Show();
        }
    }

}
