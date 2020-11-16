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
        // Таблица людей
        public DataTable PeopleDataTable { get; set; }

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
            CRUDDataBase.CreateGrantsHeaders(GrantsDataTable);
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnect();
        }

        /// <summary>
        /// Загрузка данных в таблицу людей
        /// </summary>
        private void LoadPeopleTable()
        {
            var ds = new DataSet("Grants");
            this.PeopleDataTable = ds.Tables.Add("PeopleTable");

            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.CreatePersonsHeaders(PeopleDataTable);
            CRUDDataBase.LoadPersonsTable(PeopleDataTable);
            CRUDDataBase.CloseConnect();
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
