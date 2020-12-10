using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ResearchProgram
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DataTable _grantsDataTable;
        // Таблица договоров
        public DataTable GrantsDataTable
        {
            get => _grantsDataTable;
            set
            {
                _grantsDataTable = value;
                OnPropertyChanged(nameof(GrantsDataTable));
            }
        }
        // Таблица людей
        public DataTable PeopleDataTable { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            GrantsFilters.ResetFilters();
            // Загружаем данные в таблицу грантов
            LoadGrantsTable();
            // Загружаем данные в таблицу людей
            LoadPeopleTable();

            DataContext = this;
        }

        private Settings settings
        {
            get { return (Settings)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("settings", typeof(Settings), typeof(MainWindow), new PropertyMetadata(Settings.Default));


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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
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

            newGrantWindow.ShowDialog();
        }

        // Открытие окна с созданием людей
        private void CreatePersonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createPersonWindow newPersonWindow = new createPersonWindow(PeopleDataTable); 
            newPersonWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newPersonWindow.Owner = this;
            
            newPersonWindow.ShowDialog();
        }

        // Открытие окна с созданием кафедр
        private void CreateKafedraMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createKafedraWindow newKafedraWindow = new createKafedraWindow();
            newKafedraWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newKafedraWindow.Owner = this;

            newKafedraWindow.ShowDialog();
        }

        // Открытие окна с созданием средств
        private void CreateDepositsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createDepositsWindow newDepositWindow = new createDepositsWindow();
            newDepositWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newDepositWindow.Owner = this;

            newDepositWindow.ShowDialog();
        }

        // Открытие окна с созданием учреждения
        private void CreateUnitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createUnitWindow newUnitWindow = new createUnitWindow();
            newUnitWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newUnitWindow.Owner = this;

            newUnitWindow.ShowDialog();
        }

        // Открытие окна с созданием подразделения
        private void CreateInstitutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createInstitutionWindow newInstitutionWindow = new createInstitutionWindow();
            newInstitutionWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newInstitutionWindow.Owner = this;

            newInstitutionWindow.ShowDialog();
        }

        // Открытие окна с созданием типа исследования
        private void CreateResearchTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createResearchType newResearchTypeWindow = new createResearchType();
            newResearchTypeWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newResearchTypeWindow.Owner = this;

            newResearchTypeWindow.ShowDialog();
        }

        // Открытие окна с созданием приоритетных направлений
        private void CreatePriorityTrendsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createPriorityTrendWindow newPriorityTrendWindow = new createPriorityTrendWindow();
            newPriorityTrendWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newPriorityTrendWindow.Owner = this;

            newPriorityTrendWindow.ShowDialog();
        }

        // Открытие окна с созданием типов науки
        private void CreateScienceTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createScienceTypeWindow newScienceTypeWindow = new createScienceTypeWindow();
            newScienceTypeWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newScienceTypeWindow.Owner = this;

            newScienceTypeWindow.ShowDialog();
        }

        // Открытие окна настроек
        private void SettingsTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow newSettingWindow = new SettingWindow();
            newSettingWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newSettingWindow.Owner = this;

            newSettingWindow.ShowDialog();
        }

        // Открытые окна фильтров
        private void grantsFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            FiltersWindow filtersWindow = new FiltersWindow(GrantsDataTable);
            filtersWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            filtersWindow.Owner = this;
            filtersWindow.ShowDialog();
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

        private void grantsUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnect();
        }
    }

}
