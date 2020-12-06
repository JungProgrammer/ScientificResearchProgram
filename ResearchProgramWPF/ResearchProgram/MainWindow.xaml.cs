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

        public String nDSShow = Settings.Default.NDSKey ? "Отображение с НДС" : "Отображение без НДС";

        public String NDSShow
        {
            get { return Settings.Default.NDSKey ? "Отображение с НДС" : "Отображение без НДС"; }
            set { }
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

        // Открытие окна с созданием кафедр
        private void CreateKafedraMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createKafedraWindow newKafedraWindow = new createKafedraWindow();
            newKafedraWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newKafedraWindow.Owner = this;

            newKafedraWindow.Show();
        }

        // Открытие окна с созданием средств
        private void CreateDepositsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createDepositsWindow newDepositWindow = new createDepositsWindow();
            newDepositWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newDepositWindow.Owner = this;

            newDepositWindow.Show();
        }

        // Открытие окна с созданием учреждения
        private void CreateUnitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createUnitWindow newUnitWindow = new createUnitWindow();
            newUnitWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newUnitWindow.Owner = this;

            newUnitWindow.Show();
        }

        // Открытие окна с созданием подразделения
        private void CreateInstitutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createInstitutionWindow newInstitutionWindow = new createInstitutionWindow();
            newInstitutionWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newInstitutionWindow.Owner = this;

            newInstitutionWindow.Show();
        }

        // Открытие окна с созданием типа исследования
        private void CreateResearchTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createResearchType newResearchTypeWindow = new createResearchType();
            newResearchTypeWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newResearchTypeWindow.Owner = this;

            newResearchTypeWindow.Show();
        }

        // Открытие окна с созданием приоритетных направлений
        private void CreatePriorityTrendsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createPriorityTrendWindow newPriorityTrendWindow = new createPriorityTrendWindow();
            newPriorityTrendWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newPriorityTrendWindow.Owner = this;

            newPriorityTrendWindow.Show();
        }

        // Открытие окна с созданием типов науки
        private void CreateScienceTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createScienceTypeWindow newScienceTypeWindow = new createScienceTypeWindow();
            newScienceTypeWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newScienceTypeWindow.Owner = this;

            newScienceTypeWindow.Show();
        }

        // Открытие окна настроек
        private void SettingsTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow newSettingWindow = new SettingWindow();
            newSettingWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newSettingWindow.Owner = this;

            newSettingWindow.Show();
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
