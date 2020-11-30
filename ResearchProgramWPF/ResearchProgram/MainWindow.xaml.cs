using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Controls;

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

        // открытие окна с созданием договора
        private void CreateGrantMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createGrantWindow newGrantWindow = new createGrantWindow(GrantsDataTable);
            newGrantWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newGrantWindow.Owner = this;

            newGrantWindow.Show();
        }

        // Открытие окна с созданием людей
        private void CreatePersonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createPersonWindow newPersonWindow = new createPersonWindow(PeopleDataTable); 
            newPersonWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newPersonWindow.Owner = this;
            
            newPersonWindow.Show();
        }

        // Открытые окна фильтров
        private void grantsFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            FiltersWindow filtersWindow = new FiltersWindow();
            filtersWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            filtersWindow.Owner = this;
            filtersWindow.Show();
        }

        /// <summary>
        /// Скрытие выделенных столбцов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideSelectedColumns(object sender, RoutedEventArgs e)
        {
            //selectedCells = GrantsTable.SelectedCells;
            if(GrantsTable.SelectedCells != null)
            {
                int columnNumber;

                foreach(DataGridCellInfo selectedCell in GrantsTable.SelectedCells) {
                    columnNumber = selectedCell.Column.DisplayIndex;
                    GrantsTable.Columns[columnNumber].Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MessageBox.Show("Выделите ячейки с нужными столбцами");
            }
        }

        /// <summary>
        /// Показ скрытых столбцов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showHiddenColumns(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn column in GrantsTable.Columns)
            {
                column.Visibility = Visibility.Visible;
            }
        }
    }

}
