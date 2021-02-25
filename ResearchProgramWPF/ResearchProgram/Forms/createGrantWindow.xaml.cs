using DotNetKit.Windows.Controls;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createGrantWindow.xaml
    /// </summary>
    public partial class CreateGrantWindow : Window, INotifyPropertyChanged
    {
        // Класс для выбора параметров, характеризующих структуру вуза
        private WorkerWithUniversityStructure _universityStructure;
        public WorkerWithUniversityStructure UniversityStructure
        {
            get => _universityStructure;
            set
            {
                _universityStructure = value;
                OnPropertyChanged(nameof(UniversityStructure));
            }
        }

        //Списки данных из БД
        public ObservableCollection<string> NIOKRList { get; set; }
        public List<Person> PersonsList { get; set; }
        public List<Customer> CustomersList { get; set; }
        public List<string> SelectedItems { get; set; }
        public List<string> SelectedValues { get; set; }

        public List<Depositor> DepositsList { get; set; }
        public List<ScienceType> ScienceTypeList { get; set; }
        public List<Kafedra> KafedrasList { get; set; }
        public List<Unit> UnitsList { get; set; }
        public List<Institution> InstituionsList { get; set; }
        public List<ResearchType> ResearchTypesList { get; set; }
        public List<PriorityTrend> PriorityTrendList { get; set; }
        //Списки данных из формы
        public List<ComboBox> EnteredExecutorsList { get; set; }
        public List<Object[]> EnteredDepositsList { get; set; }
        public List<ComboBox> EnteredScienceTypesList { get; set; }

        public string NirChecker;
        public string NOCChecker;

        // Combobox для руководителя
        AutoCompleteComboBox LeadNIOKRAutoCompleteComboBox;
        // Combobox для типа исследования
        AutoCompleteComboBox researchTypeAutoCompleteComboBox;


        // Если это окно отрыто для редактирования.
        private bool _isEditGrant = false;
        // id гранта, который получен для редактирования.
        private int grantEditId = 0;
        private string grantNumber;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public CreateGrantWindow(DataTable grantsDataTable, Grant grantToEdit = null)
        {
            InitializeComponent();


            // Подключение к базе данных
            CRUDDataBase.ConnectToDataBase();

            PersonsList = CRUDDataBase.GetPersons();
            UniversityStructure = CRUDDataBase.GetUniversityStructure();
            CustomersList = CRUDDataBase.GetCustomers();
            DepositsList = CRUDDataBase.GetDeposits();
            KafedrasList = CRUDDataBase.GetKafedras();
            UnitsList = CRUDDataBase.GetUnits();
            InstituionsList = CRUDDataBase.GetInstitutions();
            ResearchTypesList = CRUDDataBase.GetResearchTypes();
            ScienceTypeList = CRUDDataBase.GetScienceTypes();
            PriorityTrendList = CRUDDataBase.GetPriorityTrends();

            EnteredDepositsList = new List<object[]>();
            EnteredScienceTypesList = new List<ComboBox>();
            EnteredExecutorsList = new List<ComboBox>();

            AddLeadNIOKRAutoCompleteComboBox();
            AddResearchTypeAutoCompleteComboBox();

            // Закрытие подключения к базе данных
            CRUDDataBase.CloseConnection();


            // Если открыта форма редактирования, то вставим в нее данные
            if (grantToEdit != null)
            {
                DeleteGrantButton.Visibility = System.Windows.Visibility.Visible;
                _isEditGrant = true;
                grantEditId = grantToEdit.Id;
                grantNumber = grantToEdit.grantNumber;
                Title = "Редактирование договора";
                createGrantButton.Content = "Редактировать";
                OKVEDTextBox.Text = grantToEdit.OKVED;
                grantNumberTextBox.Text = grantToEdit.grantNumber;
                NIOKRTextBox.Text = grantToEdit.NameNIOKR;

                for (int i = 0; i < grantToEdit.Customer.Count; i++)
                {
                    AutoCompleteComboBox customerComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5, 0, 5, 0),
                        ItemsSource = new List<Customer>(CustomersList),
                        MinWidth = 300
                    };

                    for (int j = 0; j < CustomersList.Count; j++)
                        if (CustomersList[j].Title == grantToEdit.Customer[i].Title)
                            customerComboBox.SelectedIndex = j;

                    customersVerticalListView.Items.Add(customerComboBox);
                }


                startDateDatePicker.SelectedDate = grantToEdit.StartDate;
                endDateDatePicker.SelectedDate = grantToEdit.EndDate;
                if (grantToEdit.isWIthNDS)
                    priceTextBox.Text = grantToEdit.Price.ToString();
                priceNoNDSTextBox.Text = grantToEdit.PriceNoNDS.ToString();
                GrantWithoutNDSCheckBox.IsChecked = !grantToEdit.isWIthNDS;
                for (int i = 0; i < grantToEdit.Depositor.Count; i++)
                {
                    StackPanel horizontalStackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                    };
                    TextBox sumTextBox;
                    TextBox sumTextBoxNoNDS;
                    void sumTextBoxTextChangedEventHandler(object senderr, TextChangedEventArgs args)
                    {
                        if (sumTextBox.Text.Length > 0)
                            sumTextBoxNoNDS.Text = (Math.Round(Convert.ToDouble(sumTextBox.Text) * 1 / Settings.Default.NDSValue, 2)).ToString();
                        else
                            sumTextBoxNoNDS.Text = "";
                    }

                    ComboBox depositorComboBox = new ComboBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        ItemsSource = DepositsList,
                        Width = 160,
                    };
                    for (int j = 0; j < DepositsList.Count; j++)
                        if (DepositsList[j].Title == grantToEdit.Depositor[i].Title)
                            depositorComboBox.SelectedIndex = j;

                    sumTextBox = new TextBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        MinWidth = 110,
                        Text = grantToEdit.DepositorSum[i].ToString(),
                        IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked
                    };


                    sumTextBoxNoNDS = new TextBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        MinWidth = 110,
                        Text = grantToEdit.DepositorSumNoNDS[i].ToString()
                    };
                    sumTextBoxNoNDS.PreviewTextInput += TextBoxNumbersPreviewInput;
                    sumTextBoxNoNDS.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;

                    sumTextBox.PreviewTextInput += TextBoxNumbersPreviewInput;
                    sumTextBox.TextChanged += sumTextBoxTextChangedEventHandler;
                    sumTextBox.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;


                    DateTime selectedDate;
                    DateTime.TryParse(grantToEdit.ReceiptDate[i], out selectedDate);
                    DatePicker dateComboBox = new DatePicker()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        MinWidth = 110,
                        SelectedDate = selectedDate
                    };

                    horizontalStackPanel.Children.Add(depositorComboBox);
                    horizontalStackPanel.Children.Add(sumTextBox);
                    horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 0, 0), IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked });
                    horizontalStackPanel.Children.Add(sumTextBoxNoNDS);
                    horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 5, 0) });
                    horizontalStackPanel.Children.Add(dateComboBox);


                    depositsVerticalListView.Items.Add(horizontalStackPanel);
                }
                for (int i = 0; i < PersonsList.Count; i++)
                    if (PersonsList[i].FIO == grantToEdit.LeadNIOKR.FIO)
                        LeadNIOKRAutoCompleteComboBox.SelectedIndex = i;
                for (int i = 0; i < grantToEdit.Executor.Count; i++)
                {
                    AutoCompleteComboBox executorComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5, 0, 5, 0),
                        ItemsSource = new List<Person>(PersonsList),
                        MinWidth = 300
                    };
                    for (int j = 0; j < PersonsList.Count; j++)
                        if (PersonsList[j].FIO == grantToEdit.Executor[i].FIO)
                            executorComboBox.SelectedIndex = j;

                    executorsVerticalListView.Items.Add(executorComboBox);
                }

                // Привязка для структуры университета
                UniversityStructure.SelectedInstitution = UniversityStructure.FindInstitution(grantToEdit.Institution.Id);
                if (UniversityStructure.SelectedInstitution != null)
                {
                    UniversityStructure.SelectedUnit = UniversityStructure.FindUnit(UniversityStructure.SelectedInstitution, grantToEdit.Unit.Id);
                    if (UniversityStructure.SelectedUnit != null)
                    {
                        UniversityStructure.SelectedKafedra = UniversityStructure.FindKafedra(UniversityStructure.SelectedUnit, grantToEdit.Kafedra.Id);
                        if (UniversityStructure.SelectedKafedra != null)
                        {
                            UniversityStructure.SelectedLaboratory = UniversityStructure.FindLaboratoryInKafedra(UniversityStructure.SelectedKafedra, grantToEdit.Laboratory.Id);
                        }
                        if (UniversityStructure.SelectedLaboratory == null)
                        {
                            UniversityStructure.SelectedLaboratory = UniversityStructure.FindLaboratoryInUnit(UniversityStructure.SelectedUnit, grantToEdit.Laboratory.Id);
                        }
                    }
                }

                GRNTITextBox.Text = grantToEdit.GRNTI;

                if (grantToEdit.ResearchType.Count > 0)
                {
                    for (int j = 0; j < ResearchTypesList.Count; j++)
                        if (ResearchTypesList[j].Title == grantToEdit.ResearchType[0].Title)
                            researchTypeAutoCompleteComboBox.SelectedIndex = j;
                }

                for (int i = 0; i < grantToEdit.PriorityTrands.Count; i++)
                {
                    AutoCompleteComboBox priorityTrendComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5, 0, 5, 0),
                        ItemsSource = new List<PriorityTrend>(PriorityTrendList),
                        MinWidth = 300
                    };
                    for (int j = 0; j < PriorityTrendList.Count; j++)
                        if (PriorityTrendList[j].Title == grantToEdit.PriorityTrands[i].Title)
                            priorityTrendComboBox.SelectedIndex = j;


                    priorityTrendsVerticalListView.Items.Add(priorityTrendComboBox);
                }

                for (int i = 0; i < grantToEdit.ScienceType.Count; i++)
                {
                    AutoCompleteComboBox scienceTypeComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5, 0, 5, 0),
                        ItemsSource = new List<ScienceType>(ScienceTypeList),
                        MinWidth = 300
                    };

                    for (int j = 0; j < ScienceTypeList.Count; j++)
                        if (ScienceTypeList[j].Title == grantToEdit.ScienceType[i].Title)
                            scienceTypeComboBox.SelectedIndex = j;

                    scienceTypeVerticalListView.Items.Add(scienceTypeComboBox);
                }

                switch (grantToEdit.NIR)
                {
                    case "НИР":
                        NIR.IsChecked = true;
                        break;
                    case "УСЛУГА":
                        USLUGA.IsChecked = true;
                        break;
                }

                switch (grantToEdit.NOC)
                {
                    case "True":
                        NOC.IsChecked = true;
                        break;
                    case "False":
                        NotNOC.IsChecked = true;
                        break;
                }
            }

            DataContext = this;
        }


        //Функция для ввода в текст бокс только чисел с одним разделителем
        private void TextBoxNumbersPreviewInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (DS_Count(((TextBox)sender).Text) < 1)));
        }

        // функция подсчета разделителя
        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }

        /// <summary>
        /// Кнопка добавления у заказчика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerAddButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteComboBox customerComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Customer>(CustomersList),
                MinWidth = 270
            };

            customersVerticalListView.Items.Add(customerComboBox);
        }

        /// <summary>
        /// Кнопка удаления у заказчика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = customersVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    customersVerticalListView.Items.Remove(customersVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Добавление комбо бокса к руководителю НИОКР
        /// </summary>
        private void AddLeadNIOKRAutoCompleteComboBox()
        {
            LeadNIOKRAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Person>(PersonsList),
                MinWidth = 300,

            };
            CommonInfoGrid.Children.Add(LeadNIOKRAutoCompleteComboBox);
            Grid.SetRow(LeadNIOKRAutoCompleteComboBox, 9);
        }

        /// <summary>
        /// Добавление комбо бокса к типу
        /// </summary>

        private void AddResearchTypeAutoCompleteComboBox()
        {
            researchTypeAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<ResearchType>(ResearchTypesList),
                MinWidth = 300
            };
            researchTypesGrid.Children.Add(researchTypeAutoCompleteComboBox);
            Grid.SetRow(researchTypeAutoCompleteComboBox, 1);
            Grid.SetColumn(researchTypeAutoCompleteComboBox, 0);
            Grid.SetColumnSpan(researchTypeAutoCompleteComboBox, 2);
        }

        /// <summary>
        /// Добавление строки в средства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DepositsAddButton_Click_1(object sender, RoutedEventArgs e)
        {
            TextBox sumTextBox;
            TextBox sumTextBoxNoNDS;

            void sumTextBoxTextChangedEventHandler(object senderr, TextChangedEventArgs args)
            {
                if (sumTextBox.Text.Length > 0)
                    sumTextBoxNoNDS.Text = (Math.Round(Convert.ToDouble(sumTextBox.Text) * 1 / Settings.Default.NDSValue, 2)).ToString();
                else
                    sumTextBoxNoNDS.Text = "";
            }

            StackPanel horizontalStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 0)
            };

            ComboBox depositorComboBox = new ComboBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                ItemsSource = DepositsList,
                Width = 160,
            };

            sumTextBox = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                Width = 110,
                Padding = new Thickness(0, 2, 0, 2),
                IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked,
            };

            sumTextBoxNoNDS = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                Width = 110,
                Padding = new Thickness(0, 2, 0, 2)
            };
            sumTextBox.PreviewTextInput += TextBoxNumbersPreviewInput;
            sumTextBox.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;
            sumTextBox.TextChanged += sumTextBoxTextChangedEventHandler;

            sumTextBoxNoNDS.PreviewTextInput += TextBoxNumbersPreviewInput;
            sumTextBoxNoNDS.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;

            DatePicker datePicker = new DatePicker()
            {
                Margin = new Thickness(5, 0, 5, 10),
                Width = 110
            };

            horizontalStackPanel.Children.Add(depositorComboBox);
            horizontalStackPanel.Children.Add(sumTextBox);
            horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 0, 0), IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked });
            horizontalStackPanel.Children.Add(sumTextBoxNoNDS);
            horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 5, 0) });
            horizontalStackPanel.Children.Add(datePicker);

            depositsVerticalListView.Items.Add(horizontalStackPanel);
        }

        /// <summary>
        /// Метод удаления строки из средств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DepositsDeleteButton_Click(object sender, RoutedEventArgs e)
        {

            int countSelectedElement = depositsVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    depositsVerticalListView.Items.Remove(depositsVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecutorAddButton_Click(object sender, RoutedEventArgs e)
        {

            AutoCompleteComboBox executorComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Person>(PersonsList),
                MinWidth = 270
            };

            executorsVerticalListView.Items.Add(executorComboBox);
        }

        private void ExecutorDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = executorsVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    executorsVerticalListView.Items.Remove(executorsVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PriorityTrendAddButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteComboBox priorityTrendComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<PriorityTrend>(PriorityTrendList),
                Width = 270
            };


            priorityTrendsVerticalListView.Items.Add(priorityTrendComboBox);
        }

        private void PriorityTrendDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = priorityTrendsVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    priorityTrendsVerticalListView.Items.Remove(priorityTrendsVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ScienceTypeAddButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteComboBox scienceTypeComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<ScienceType>(ScienceTypeList),
                Width = 270
            };
            scienceTypeVerticalListView.Items.Add(scienceTypeComboBox);
        }

        private void ScienceTypeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = scienceTypeVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    scienceTypeVerticalListView.Items.Remove(scienceTypeVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Смена вкладок по нажатию кнопки и изменение цвета фона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrantParametersButtonClick(object sender, RoutedEventArgs e)
        {
            createGrantTabControl.SelectedItem = createGrantTabControl.Items.OfType<TabItem>().SingleOrDefault(n => n.Name == ((Button)sender).Tag.ToString());
            foreach (Button button in grantParametersButtonStackPanel.Children.OfType<Button>())
            {
                button.Background = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
            }
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 189, 189, 189));

        }
        /// <summary>
        /// Создает новый экземпляр договора и загружает его в базу данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateGrantButtonClick(object sender, RoutedEventArgs e)
        {
            Grant newGrant = new Grant();
            newGrant.Id = grantEditId;

            string incorrectDataString = "";
            // Булевская переменная, которая отвечает за правильное создание договора. Если все необходимые данные были внесены, то договор создается
            bool isAllOkey = true;

            if (OKVEDTextBox.Text.ToString() != "")
            {
                newGrant.OKVED = OKVEDTextBox.Text;
            }
            else
            {
                newGrant.OKVED = "";
            }

            if (grantNumberTextBox.Text.ToString() != "")
            {
                newGrant.grantNumber = grantNumberTextBox.Text;

                if (!CRUDDataBase.IsGrantNumberAvailable(newGrant))
                {
                    incorrectDataString += "Договор с таким номером уже существует. Пожалуйста, укажите уникальный номер договора.\n\n";
                    isAllOkey = false;
                }
            }
            else
            {
                //MessageBox.Show("Необходимо указать номер договора");
                incorrectDataString += "Необходимо указать номер договора\n";
                isAllOkey = false;
            }

            if (NIOKRTextBox.Text.ToString() != "")
            {
                newGrant.NameNIOKR = NIOKRTextBox.Text.ToString();
            }
            else
            {
                newGrant.NameNIOKR = "";
            }

            if (customersVerticalListView.Items != null)
            {
                foreach (AutoCompleteComboBox cmb in customersVerticalListView.Items.OfType<AutoCompleteComboBox>())
                {
                    if (cmb.SelectedItem != null)
                    {
                        newGrant.Customer.Add(new Customer()
                        {
                            Id = ((Customer)cmb.SelectedItem).Id,
                            Title = ((Customer)cmb.SelectedItem).Title
                        });
                    }
                }
            }
            else
            {
                //MessageBox.Show("Необходимо указать заказчика");
                incorrectDataString += "Необходимо указать заказчика\n";
                isAllOkey = false;
            }

            if (startDateDatePicker.SelectedDate != null)
            {
                newGrant.StartDate = (DateTime)startDateDatePicker.SelectedDate;
            }

            if (endDateDatePicker.SelectedDate != null)
            {
                newGrant.EndDate = (DateTime)endDateDatePicker.SelectedDate;
            }

            if (priceTextBox.Text.ToString() != "")
            {
                newGrant.Price = Parser.ConvertToRightFloat(priceTextBox.Text);
            }

            if (priceNoNDSTextBox.Text.ToString() != "")
            {
                newGrant.PriceNoNDS = Parser.ConvertToRightFloat(priceNoNDSTextBox.Text);
            }

            if (depositsVerticalListView.Items != null)
            {
                ComboBox cmb;
                TextBox partSum;
                TextBox partSumNoNDS;
                DatePicker datePicker;

                foreach (StackPanel sp in depositsVerticalListView.Items.OfType<StackPanel>())
                {
                    Console.WriteLine("111");
                    cmb = (ComboBox)sp.Children[0];
                    partSum = (TextBox)sp.Children[1];
                    partSumNoNDS = (TextBox)sp.Children[3];
                    datePicker = (DatePicker)sp.Children[5];
                    partSum.Text.Replace(" ", "");
                    partSumNoNDS.Text.Replace(" ", "");
                    DateTime selectedDate;
                    DateTime.TryParse(datePicker.SelectedDate.ToString(), out selectedDate);

                    if (cmb.SelectedItem == null)
                    {
                        isAllOkey = false;
                        incorrectDataString += "Не указаны источники финансирования.\n";
                    }

                    else
                    if ((bool)GrantWithoutNDSCheckBox.IsChecked)
                    {
                        //БЕЗ НДС
                        if(partSumNoNDS.Text.ToString() != "")
                        {
                            //ПОЛЯ ЗАПОЛНЕНЫ ПРАВИЛЬНО
                            newGrant.Depositor.Add(new Depositor()
                            {
                                Id = ((Depositor)cmb.SelectedItem).Id,
                                Title = cmb.SelectedItem.ToString(),
                            });
                            newGrant.DepositorSum.Add(0);
                            newGrant.DepositorSumNoNDS.Add(Parser.ConvertToRightFloat(partSumNoNDS.Text));
                            newGrant.ReceiptDate.Add(selectedDate.ToShortDateString());
                        }
                        else
                        {
                            isAllOkey = false;
                            incorrectDataString += "Не указаны суммы финансирования.\n";
                        }
                    }
                    else
                    {
                        // С НДС
                        if (partSum.Text.ToString() != "" && partSumNoNDS.Text.ToString() != "")
                        {
                            //ПОЛЯ ЗАПОЛНЕНЫ ПРАВИЛЬНО
                            newGrant.Depositor.Add(new Depositor()
                            {
                                Id = ((Depositor)cmb.SelectedItem).Id,
                                Title = cmb.SelectedItem.ToString(),
                            });
                            newGrant.DepositorSum.Add(Parser.ConvertToRightFloat(partSum.Text));
                            newGrant.DepositorSumNoNDS.Add(Parser.ConvertToRightFloat(partSumNoNDS.Text));
                            newGrant.ReceiptDate.Add(selectedDate.ToShortDateString());
                        }
                        else
                        {
                            isAllOkey = false;
                            incorrectDataString += "Не указаны суммы финансирования.\n";
                        }
                    }
                }
            }

            if (LeadNIOKRAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.LeadNIOKR = new Person()
                {
                    Id = ((Person)LeadNIOKRAutoCompleteComboBox.SelectedItem).Id,
                    FIO = ((Person)LeadNIOKRAutoCompleteComboBox.SelectedItem).FIO
                };
            }
            else
            {
                newGrant.LeadNIOKR = null;
            }

            if (executorsVerticalListView.Items != null)
            {
                foreach (AutoCompleteComboBox cmb in executorsVerticalListView.Items.OfType<AutoCompleteComboBox>())
                {
                    if (cmb.SelectedItem != null)
                    {
                        newGrant.Executor.Add(new Person()
                        {
                            Id = ((Person)cmb.SelectedItem).Id,
                            FIO = ((Person)cmb.SelectedItem).FIO
                        });
                    }
                }
            }

            // Добавление учреждения в новый договор
            if (UniversityStructure.SelectedInstitution != null)
            {
                newGrant.Institution = new Institution()
                {
                    Id = UniversityStructure.SelectedInstitution.Id,
                    Title = UniversityStructure.SelectedInstitution.Title
                };
            }
            else
            {
                newGrant.Institution = null;
            }

            // Добавление подразделения в договор
            if (UniversityStructure.SelectedUnit != null)
            {
                newGrant.Unit = new Unit()
                {
                    Id = UniversityStructure.SelectedUnit.Id,
                    Title = UniversityStructure.SelectedUnit.Title
                };
            }
            else
            {
                newGrant.Unit = null;
            }

            // Добавление кафедры в договор
            if (UniversityStructure.SelectedKafedra != null)
            {
                newGrant.Kafedra = new Kafedra()
                {
                    Id = UniversityStructure.SelectedKafedra.Id,
                    Title = UniversityStructure.SelectedKafedra.Title
                };
            }
            else
            {
                newGrant.Kafedra = null;
            }

            // Добавление лаборатории в договор
            if (UniversityStructure.SelectedLaboratory != null)
            {
                newGrant.Laboratory = new Laboratory()
                {
                    Id = UniversityStructure.SelectedLaboratory.Id,
                    Title = UniversityStructure.SelectedLaboratory.Title
                };
            }
            else
            {
                newGrant.Laboratory = null;
            }


            if (GRNTITextBox.Text != "")
            {
                newGrant.GRNTI = GRNTITextBox.Text;
            }
            else
            {
                newGrant.GRNTI = "";
            }

            if (researchTypeAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.ResearchType.Add(new ResearchType()
                {
                    Id = ((ResearchType)researchTypeAutoCompleteComboBox.SelectedItem).Id,
                    Title = researchTypeAutoCompleteComboBox.SelectedItem.ToString()
                });
            }
            if (priorityTrendsVerticalListView.Items != null)
            {
                foreach (AutoCompleteComboBox cmb in priorityTrendsVerticalListView.Items.OfType<AutoCompleteComboBox>())
                {
                    if (cmb.SelectedItem != null)
                    {
                        newGrant.PriorityTrands.Add(new PriorityTrend()
                        {
                            Id = ((PriorityTrend)cmb.SelectedItem).Id,
                            Title = cmb.SelectedItem.ToString()
                        });
                    }
                }
            }

            if (scienceTypeVerticalListView.Items != null)
            {
                foreach (AutoCompleteComboBox cmb in scienceTypeVerticalListView.Items.OfType<AutoCompleteComboBox>())
                {
                    if (cmb.SelectedItem != null)
                    {
                        newGrant.ScienceType.Add(new ScienceType()
                        {
                            Id = ((ScienceType)cmb.SelectedItem).Id,
                            Title = cmb.SelectedItem.ToString()
                        });
                    }
                }
            }

            if (NirChecker != null)
            {
                newGrant.NIR = NirChecker;
            }
            else
            {
                newGrant.NIR = "";
            }

            if (NOCChecker != null)
            {
                newGrant.NOC = NOCChecker;
            }
            else
            {
                newGrant.NOC = "";
            }

            newGrant.isWIthNDS = !(bool)GrantWithoutNDSCheckBox.IsChecked;

            // Если данные введены корректно
            if (isAllOkey)
            {
                // Если редактирование договора
                if (_isEditGrant)
                {
                    // Подключаюсь к БД
                    CRUDDataBase.ConnectToDataBase();

                    CRUDDataBase.UpdateGrantNumber(newGrant);
                    CRUDDataBase.UpdateOKVED(newGrant);
                    CRUDDataBase.UpdateNameNIOKR(newGrant);
                    CRUDDataBase.UpdateCustomers(newGrant);
                    CRUDDataBase.UpdateStartDate(newGrant);
                    CRUDDataBase.UpdateEndDate(newGrant);
                    CRUDDataBase.UpdatePrice(newGrant);
                    CRUDDataBase.UpdatePriceNoNDS(newGrant);
                    CRUDDataBase.UpdateDeposits(newGrant);
                    CRUDDataBase.UpdateLeadNiokr(newGrant);
                    CRUDDataBase.UpdateExecutors(newGrant);
                    CRUDDataBase.UpdateInstitution(newGrant);
                    CRUDDataBase.UpdateUnit(newGrant);
                    CRUDDataBase.UpdateKafedra(newGrant);
                    CRUDDataBase.UpdateLaboratory(newGrant);
                    CRUDDataBase.UpdateGRNTI(newGrant);
                    CRUDDataBase.UpdateResearchType(newGrant);
                    CRUDDataBase.UpdatePriorityTrends(newGrant);
                    CRUDDataBase.UpdateScienceTypes(newGrant);
                    CRUDDataBase.UpdateNIR(newGrant);
                    CRUDDataBase.UpdateNOC(newGrant);
                    CRUDDataBase.UpdateIsWithNDS(newGrant);

                    // Закрываем соединение с БД
                    CRUDDataBase.CloseConnection();

                    MessageBox.Show("Договор успешно изменен", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                // Если создание нового договора
                else
                {
                    // Подключаюсь к БД
                    CRUDDataBase.ConnectToDataBase();

                    CRUDDataBase.InsertNewGrantToDB(newGrant);

                    // Закрываем соединение с БД
                    CRUDDataBase.CloseConnection();

                    MessageBox.Show("Договор успешно создан", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Close();
            }
            else
            {
                MessageBox.Show(incorrectDataString, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteGrantButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult sure = MessageBox.Show("Удалить договор с номером " + grantNumber + "?", "Удаление договора", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            switch (sure)
            {
                case MessageBoxResult.Yes:
                    CRUDDataBase.DeleteGrant(grantNumber);
                    MessageBox.Show("Удаление успешно", "Удаление договора", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                    break;
            }
        }

        private void NIRRadioChecked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            NirChecker = pressed.Content.ToString();
        }

        private void NOCRadioChecked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            NOCChecker = pressed.Content.ToString();
        }

        private void GrantWithoutNDSCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            priceTextBox.IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked;
            PriceLabel.IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked;
            PriceRubLabel.IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked;
            SummLabel.IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked;
            if ((bool)GrantWithoutNDSCheckBox.IsChecked)
                priceTextBox.Text = "";
            foreach (StackPanel sp in depositsVerticalListView.Items.OfType<StackPanel>())
            {
                sp.Children[1].IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked;
                if ((bool)GrantWithoutNDSCheckBox.IsChecked)
                    ((TextBox)sp.Children[1]).Text = "";
                sp.Children[2].IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked;
            }
        }

        private void priceNoNDSTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        
    }
}
