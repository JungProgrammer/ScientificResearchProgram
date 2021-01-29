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
using OfficeOpenXml;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml.Style;
using ResearchProgram.Forms;

namespace ResearchProgram
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DataTable _grantsDataTable;

        // Выбранная начальная дата оплат
        private DateTime _selectedStartDate;
        public DateTime SelectedStartDate
        {
            get => _selectedStartDate;
            set
            {
                _selectedStartDate = value;
                OnPropertyChanged(nameof(SelectedStartDate));
            }
        }

        // Выбранная конечная дата оплат
        private DateTime _selectedEndDate;
        public DateTime SelectedEndDate
        {
            get => _selectedEndDate;
            set
            {
                _selectedEndDate = value;
                OnPropertyChanged(nameof(SelectedEndDate));
            }
        }
        
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

        // Окно фильтров
        FiltersWindow filtersWindow;

        public MainWindow()
        {
            SplashScreen splash = new SplashScreen("Images\\splashscreen.png");
            splash.Show(false, true);
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            GrantsFilters.ResetFilters();

            // Установка изначальных настроек
            SetSettings();

            // Загружаем данные в таблицу грантов
            LoadGrantsTable();
            // Загружаем данные в таблицу людей
            LoadPeopleTable();

            // Загрука окна фильтров без его открытия
            LoadFilterWindow();

            DataContext = this;
            splash.Close(TimeSpan.FromMilliseconds(0));
        }

        private void SetSettings()
        {
            SelectedStartDate = DateTime.MinValue;
            SelectedEndDate = DateTime.Now;

            GrantsFilters.StartDepositDate = new FilterElement() { Data = _selectedStartDate.ToString() };
            GrantsFilters.EndDepositDate = new FilterElement() { Data = _selectedEndDate.ToString() };
        }

        private void LoadFilterWindow()
        {
            filtersWindow = new FiltersWindow(GrantsDataTable)
            {

                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
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
                System.Windows.MessageBox.Show("Выделите ячейки с нужными столбцами");
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
            GrantsFilters.StartDepositDate.Data = _selectedStartDate.ToString();
            GrantsFilters.EndDepositDate.Data = _selectedEndDate.ToString();

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
            string grantNumber = SelectedGrantRow.Row.Field<String>("Номер договора");
            Grant grant = CRUDDataBase.GetGrantByGrantNumber(grantNumber);

            CreateGrantWindow newGrantWindow = new CreateGrantWindow(GrantsDataTable, grant)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newGrantWindow.Closing += (senders, args) => { newGrantWindow.Owner = null; };

            newGrantWindow.Show();
        }

        public DataRowView selectedPersonRow;
        public DataRowView SelectedPersonRow
        {
            get { return selectedPersonRow; }
            set { selectedPersonRow = value; }
        }
        private void EditPeople(object sender, RoutedEventArgs e)
        {
            string personId = SelectedPersonRow.Row.Field<String>("id");
            Console.WriteLine(personId);
            Person person = CRUDDataBase.GetPersonByPersonId(personId);

            createPersonWindow newPersonWindow = new createPersonWindow(GrantsDataTable, person)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newPersonWindow.Closing += (senders, args) => { newPersonWindow.Owner = null; };

            newPersonWindow.Show();
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

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel |*.xlsx";
            saveFileDialog1.FileName = "Отчёт " + DateTime.Now.ToString("dd-MM-yyyy hh-mm");
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;

            //номер столбца с которого начнут выводиться средства
            const int EXCEL_DEPOSITS_START_COLUMN = 9;

            //указываем что используем пакет EPPlus в некоммерческих целях, иначе исключение бросит 
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            //инициализируем эксель
            ExcelPackage excelPackage = new ExcelPackage();
            //инициализируем страницу
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Лист 1");
            //грузим все данные из таблицы грантов одним махом
            worksheet.Cells["A1"].LoadFromDataTable(GrantsDataTable, PrintHeaders: true);
            //удаляем колонки со средствами
            worksheet.DeleteColumn(EXCEL_DEPOSITS_START_COLUMN, 2);
            //получаем список средств
            CRUDDataBase.ConnectToDataBase();
            List<Depositor> depositors = CRUDDataBase.GetDeposits();
            CRUDDataBase.CloseConnection();
            //вставлям колонки по количеству средств
            worksheet.InsertColumn(EXCEL_DEPOSITS_START_COLUMN, depositors.Count);
            //счетчик для добавления заголовков средств в эксель
            int depositorColumnCount = EXCEL_DEPOSITS_START_COLUMN;
            //словарь, чтобы запомнить какое средство в каком столбце сидит
            Dictionary<string, int> depositsColumnId = new Dictionary<string, int>();
            // перебираем все средства и указываем соответствующие заголовки в таблице
            foreach (Depositor depositor in depositors)
            {
                worksheet.Cells[1, depositorColumnCount].Value = depositor.Title;
                //запоминаем в каком столбце сидит это средство
                depositsColumnId.Add(depositor.Title, depositorColumnCount);
                depositorColumnCount++;
            }
            //обнуляем все средства одним махом
            worksheet.Cells[2, EXCEL_DEPOSITS_START_COLUMN, worksheet.Dimension.End.Row, EXCEL_DEPOSITS_START_COLUMN + depositors.Count].Value = 0;
            
            //перебираем все строки в таблице грантов
            int row_count = 2;
            foreach (DataRow row in GrantsDataTable.Rows)
            {
                //словарь для хранения средства и его значения
                Dictionary<string, double> depositDict = new Dictionary<string, double>();
                //получаем строку средств и значений, сразу же делим на массивы
                string[] depositString = row.ItemArray[8].ToString().Split('\n');
                string[] depositSummString = row.ItemArray[9].ToString().Split('\n');
                //проходим по каждому виду средств
                for (int i = 0; i < depositString.Length; i++)
                {
                    //если в словаре средств текущего договора уже есть такое средство, то суммируем их значения
                    if (depositDict.ContainsKey(depositString[i]))
                        depositDict[depositString[i]] += Convert.ToDouble(depositSummString[i]);
                    else
                        if(depositString[i] != String.Empty && depositSummString[i] != String.Empty)
                        {
                            //если такого средства в словаре еще нет, то добавим
                            depositDict.Add(depositString[i], Convert.ToDouble(depositSummString[i]));
                        }
                }
                //перебираем словарь со средствами текущей строки
                foreach (KeyValuePair<string, double> entry in depositDict)
                {
                    //добавляем значение средства в соответствующую колонку
                    worksheet.Cells[row_count, depositsColumnId[entry.Key]].Value = entry.Value;
                }
                row_count++;
            }


            worksheet.Cells["A:AA"].AutoFitColumns();
            worksheet.Cells["A1:AA1"].Style.Font.Bold = true;
            worksheet.Cells[2,1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.WrapText = true;
            worksheet.View.FreezePanes(2, 1);
            worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Border.Left.Style = ExcelBorderStyle.Thin;


            FileInfo fi = new FileInfo(filename);
            try
            {
                excelPackage.SaveAs(fi);
            }
            catch (System.InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Сохранение не удалось. Пожалуйста, закройте файл, подлежащий перезаписи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            System.Windows.MessageBox.Show("Отчёт успешно сохранён", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            filtersWindow.WindowCanToBeClose = true;
            filtersWindow.Close();
        }

        private void ShowFullInformation(object sender, RoutedEventArgs e)
        {
            string grantNumber = SelectedGrantRow.Row.Field<String>("Номер договора");
            Grant grant = CRUDDataBase.GetGrantByGrantNumber(grantNumber);

            FullGrantInfo fullGrantInfonewGrantWindow = new FullGrantInfo(grant)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            fullGrantInfonewGrantWindow.Closing += (senders, args) => { fullGrantInfonewGrantWindow.Owner = null; };

            fullGrantInfonewGrantWindow.Show();
        }
    }
}
