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

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            GrantsFilters.ResetFilters();
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


            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.CreateGrantsHeaders(GrantsDataTable);
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnection();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Загрузка данных в таблицу людей
        /// </summary>
        private void LoadPeopleTable()
        {
            var ds = new DataSet("Grants");
            this.PeopleDataTable = ds.Tables.Add("PeopleTable");

            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.CreatePersonsHeaders(PeopleDataTable);
            CRUDDataBase.LoadPersonsTable(PeopleDataTable);
            CRUDDataBase.CloseConnection();
        }

        // открытие окна с созданием договора
        private void CreateGrantMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateGrantWindow newGrantWindow = new CreateGrantWindow(GrantsDataTable)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newGrantWindow.Closing += (senders, args) => { newGrantWindow.Owner = null; };

            newGrantWindow.Show();
        }

        // Открытие окна с созданием людей
        private void CreatePersonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createPersonWindow newPersonWindow = new createPersonWindow(PeopleDataTable)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newPersonWindow.ShowDialog();
        }

        // Открытие окна с созданием кафедр
        private void CreateKafedraMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createKafedraWindow newKafedraWindow = new createKafedraWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newKafedraWindow.ShowDialog();
        }

        // Открытие окна с созданием средств
        private void CreateDepositsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createDepositsWindow newDepositWindow = new createDepositsWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newDepositWindow.ShowDialog();
        }

        // Открытие окна с созданием учреждения
        private void CreateUnitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createUnitWindow newUnitWindow = new createUnitWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newUnitWindow.ShowDialog();
        }

        // Открытие окна с созданием подразделения
        private void CreateInstitutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createInstitutionWindow newInstitutionWindow = new createInstitutionWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newInstitutionWindow.ShowDialog();
        }

        // Открытие окна с созданием типа исследования
        private void CreateResearchTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createResearchType newResearchTypeWindow = new createResearchType
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newResearchTypeWindow.ShowDialog();
        }

        // Открытие окна с созданием приоритетных направлений
        private void CreatePriorityTrendsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createPriorityTrendWindow newPriorityTrendWindow = new createPriorityTrendWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newPriorityTrendWindow.ShowDialog();
        }

        // Открытие окна с созданием типов науки
        private void CreateScienceTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            createScienceTypeWindow newScienceTypeWindow = new createScienceTypeWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newScienceTypeWindow.ShowDialog();
        }

        // Открытие окна настроек
        private void SettingsTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow newSettingWindow = new SettingWindow();
            newSettingWindow.ReloadGrantsTable += new EventHandler(GrantsUpdateButton_Click);
            newSettingWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newSettingWindow.Owner = this;

            newSettingWindow.ShowDialog();
        }

        // Открытые окна фильтров
        private void GrantsFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            FiltersWindow filtersWindow = new FiltersWindow(GrantsDataTable)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            filtersWindow.Show();
        }

        /// <summary>
        /// Скрытие выделенных столбцов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideSelectedColumns(object sender, RoutedEventArgs e)
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
        private void ShowHiddenColumns(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn column in GrantsTable.Columns)
            {
                column.Visibility = Visibility.Visible;
            }
        }

        private void GrantsUpdateButton_Click(object sender, EventArgs e)
        {
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnection();
        }

        public DataRowView selectedGrantRow;
        public DataRowView SelectedGrantRow
        {
            get { return selectedGrantRow; }
            set {selectedGrantRow = value;}
        }
        private void EditGrant(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(SelectedGrantRow.Row.Field<String>("Номер договора"));
        }

        private void StructureOfUniversity_Click(object sender, RoutedEventArgs e)
        {
            UniversityStructureWindow universityStructureWindow = new UniversityStructureWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            universityStructureWindow.Closing += (senders, args) => { universityStructureWindow.Owner = null; };
            universityStructureWindow.Owner = this;
            universityStructureWindow.Show();

        }
    }

}
