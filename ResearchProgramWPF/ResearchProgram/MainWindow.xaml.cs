using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResearchProgram.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ResearchProgram.Classes;

namespace ResearchProgram
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        // Таблица договоров
        private DataTable _grantsDataTable;
        public DataTable GrantsDataTable { get => _grantsDataTable; set { _grantsDataTable = value; OnPropertyChanged(nameof(GrantsDataTable)); } }
        // Таблица людей
        public DataTable PeopleDataTable { get; set; }

        public DataTable CustomersDataTable { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public MainWindow()
        {
            InitializeComponent();

            CRUDDataBase.GetDepositsVerbose();

            Title = "Гранты НИЧ ";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            GrantsFilters.ResetFilters();

            // Загружаем данные в таблицу грантов
            LoadGrantsTable();
            // Загружаем данные в таблицу людей
            LoadPeopleTable();
            //Загружаем данные в таблицу заказчиков
            LoadCustomerTable();

            DataContext = this;
        }


        /// <summary>
        /// Загрузка данных в таблицу договоров
        /// </summary>
        public void LoadGrantsTable()
        {
            var ds = new DataSet("Grants");
            GrantsDataTable = ds.Tables.Add("GrantsTable");

            WorkerWithTablesOnMainForm.CreateHeaders(GrantsDataTable, WorkerWithTablesOnMainForm.GrantsHeaders);
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
        }

        /// <summary>
        /// Загрузка данных в таблицу людей
        /// </summary>
        private void LoadPeopleTable()
        {
            var ds = new DataSet("People");
            PeopleDataTable = ds.Tables.Add("PeopleTable");

            WorkerWithTablesOnMainForm.CreateHeaders(PeopleDataTable, WorkerWithTablesOnMainForm.PersonsHeaders);
            CRUDDataBase.LoadPersonsTable(PeopleDataTable);
        }

        private void LoadCustomerTable()
        {
            var ds = new DataSet("Customers");
            CustomersDataTable = ds.Tables.Add("CustomersTable");

            WorkerWithTablesOnMainForm.CreateHeaders(CustomersDataTable, WorkerWithTablesOnMainForm.CustomersHeaders);
            CRUDDataBase.LoadCustomersTable(CustomersDataTable);
        }

        // открытие окна с созданием договора
        private void CreateGrantMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateGrantWindow newGrantWindow = new CreateGrantWindow()
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
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newPersonWindow.Closing += (senders, args) => { newPersonWindow.Owner = null; };
            newPersonWindow.Show();
        }

        // Открытие окна с созданием средств
        private void CreateCustomerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateCustomerWindow newCusomerWindow = new CreateCustomerWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            newCusomerWindow.ShowDialog();
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

        private void GrantsFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            GrantFilterWindow newFilterWindow = new GrantFilterWindow(GrantsDataTable)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newFilterWindow.Closing += (senders, args) => { newFilterWindow.Owner = null; };
            newFilterWindow.Show();

        }

        private void PersonsFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            PersonFilterWindow newFilterWindow = new PersonFilterWindow(PeopleDataTable)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newFilterWindow.Closing += (senders, args) => { newFilterWindow.Owner = null; };
            newFilterWindow.Show();
        }

        /// <summary>
        /// Скрытие выделенных столбцов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideSelectedColumns(object sender, RoutedEventArgs e)
        {
            if (GrantsTable.SelectedCells != null)
            {
                int columnNumber;

                foreach (DataGridCellInfo selectedCell in GrantsTable.SelectedCells)
                {
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

        public void ReloadGrantsWithFilters()
        {
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnection();
        }
        public void GrantsUpdateButton_Click(object sender, EventArgs e)
        {
            GrantsDataTable.DefaultView.RowFilter = null;
            GrantsFilters.ResetFilters();

            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnection();
        }

        public void PersonsUpdateButton_Click(object sender, EventArgs e)
        {
            PeopleDataTable.DefaultView.RowFilter = null;
            PersonsFilters.ResetFilters();

            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.LoadPersonsTable(PeopleDataTable);
            CRUDDataBase.CloseConnection();
        }

        public void CustomersUpdateButton_Click(object sender, EventArgs e)
        {
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.LoadCustomersTable(CustomersDataTable);
            CRUDDataBase.CloseConnection();
        }

        public DataRowView selectedGrantRow;
        public DataRowView SelectedGrantRow { get { return selectedGrantRow; } set { selectedGrantRow = value; } }
        private void EditGrant(object sender, RoutedEventArgs e)
        {
            string grantId = "";
            try
            {
                grantId = SelectedGrantRow.Row.Field<string>("id");
            }
            catch
            { return; }
            Console.WriteLine(grantId);

            Grant grant = StaticData.GetGrantById(Convert.ToInt32(grantId));

            CreateGrantWindow newGrantWindow = new CreateGrantWindow(grant)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newGrantWindow.Closing += (senders, args) => { newGrantWindow.Owner = null; };
            newGrantWindow.Show();
        }

        public DataRowView selectedPersonRow;
        public DataRowView SelectedPersonRow { get { return selectedPersonRow; } set { selectedPersonRow = value; } }
        private void EditPeople(object sender, RoutedEventArgs e)
        {
            string personId = "";
            try
            {
                personId = SelectedPersonRow.Row.Field<String>("id");
            }
            catch { return; }
            Console.WriteLine(personId);
            Person person = StaticData.GetPersonById(Convert.ToInt32(personId));

            createPersonWindow newPersonWindow = new createPersonWindow(GrantsDataTable, person)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newPersonWindow.Closing += (senders, args) => { newPersonWindow.Owner = null; };

            newPersonWindow.Show();
        }

        public DataRowView selectedCustomerRow;
        public DataRowView SelectedCustomerRow { get { return selectedPersonRow; }  set { selectedPersonRow = value; } }
        private void EditCustomer(object sender, RoutedEventArgs e)
        {
            string CustomerId = "";
            try
            {
                CustomerId = SelectedCustomerRow.Row.Field<String>("id");
            }
            catch { return; }
            Console.WriteLine(CustomerId);
            Customer customer = StaticData.GetCustomerById(Convert.ToInt32(CustomerId));

            CreateCustomerWindow newCustomerWindow = new CreateCustomerWindow(customer)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            newCustomerWindow.Closing += (senders, args) => { newCustomerWindow.Owner = null; };

            newCustomerWindow.Show();
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
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx",
                FileName = "Отчёт " + DateTime.Now.ToString("dd-MM-yyyy hh-mm")
            };

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
            worksheet.DeleteColumn(1, 1);
            //удаляем колонки со средствами
            worksheet.DeleteColumn(EXCEL_DEPOSITS_START_COLUMN, 2);
            //получаем список средств
            CRUDDataBase.ConnectToDataBase();
            List<Depositor> depositors = StaticData.GetAllDeposits();
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
                string[] depositString = row.ItemArray[9].ToString().Split('\n');
                string[] depositSummString = row.ItemArray[10].ToString().Split('\n');
                //проходим по каждому виду средств
                for (int i = 0; i < depositString.Length; i++)
                {
                    //если в словаре средств текущего договора уже есть такое средство, то суммируем их значения
                    if (depositDict.ContainsKey(depositString[i]))
                        depositDict[depositString[i]] += Convert.ToDouble(depositSummString[i]);
                    else
                        if (depositString[i] != String.Empty && depositSummString[i] != String.Empty)
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
            worksheet.Cells[2, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.WrapText = true;
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

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }


        private void ShowFullInformation(object sender, RoutedEventArgs e)
        {
            string grantId = SelectedGrantRow.Row.Field<string>("id");
            Grant grant = StaticData.GetGrantById(Convert.ToInt32(grantId));

            FullGrantInfo fullGrantInfonewGrantWindow = new FullGrantInfo(grant)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            // Эта штука нужна чтобы родительское окно не скрывалось, когда дочернее закрывается
            fullGrantInfonewGrantWindow.Closing += (senders, args) => { fullGrantInfonewGrantWindow.Owner = null; };
            fullGrantInfonewGrantWindow.Show();
        }

        private void PeopleTable_GotFocus(object sender, RoutedEventArgs e)
        {
            PeopleTable.Columns[0].Visibility = Visibility.Collapsed;
        }

        private void CustomerTable_GotFocus(object sender, RoutedEventArgs e)
        {
            CustomerTable.Columns[0].Visibility = Visibility.Collapsed;
        }

        private void GrantsTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            GrantsTable.Columns[0].Visibility = Visibility.Collapsed;

        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            JobsWindow jobsWindow = new JobsWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            jobsWindow.ShowDialog();
        }

        private void Degree_Click(object sender, RoutedEventArgs e)
        {
            DegreesWindow degreesWindow = new DegreesWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            degreesWindow.ShowDialog();
        }

        private void Rank_Click(object sender, RoutedEventArgs e)
        {
            RanksWindow ranksWindow = new RanksWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            ranksWindow.ShowDialog();
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            categoryWindow.ShowDialog();
        }

        private void GrantsTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var col = e.Column as DataGridTextColumn;

            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));

            col.ElementStyle = style;

            col.MaxWidth = 400;
        }


        private void PeopleTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var col = e.Column as DataGridTextColumn;

            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));

            col.ElementStyle = style;

            col.MaxWidth = 400;
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            if (File.Exists("changelog.txt"))
            {
                using (StreamReader sr = new StreamReader("changelog.txt"))
                {
                    System.Windows.MessageBox.Show(sr.ReadToEnd(), "Список изменений", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                File.Delete(Path.GetFileName("changelog.txt"));
            }
        }

        private void grantsAggregateButton_Click(object sender, RoutedEventArgs e)
        {
            GrantAggregationWindow grantAggregationWindow = new GrantAggregationWindow(GrantsDataTable)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            grantAggregationWindow.ShowDialog();
        }
    }
}
