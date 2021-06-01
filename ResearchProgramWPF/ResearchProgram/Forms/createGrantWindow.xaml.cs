using ResearchProgram.Classes;
using Sdl.MultiSelectComboBox.Themes.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createGrantWindow.xaml
    /// </summary>
    public partial class CreateGrantWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<UniversityStructureNode> _firstNodeList;
        private ObservableCollection<UniversityStructureNode> _secondNodeList;
        private ObservableCollection<UniversityStructureNode> _thirdNodeList;
        private ObservableCollection<UniversityStructureNode> _fourthNodeList;
        public ObservableCollection<UniversityStructureNode> FirstNodeList { get { return _firstNodeList; } set { _firstNodeList = value; OnPropertyChanged("FirstNodeList"); } }
        public ObservableCollection<UniversityStructureNode> SecondNodeList { get { return _secondNodeList; } set { _secondNodeList = value; OnPropertyChanged("SecondNodeList"); } }
        public ObservableCollection<UniversityStructureNode> ThirdNodeList { get { return _thirdNodeList; } set { _thirdNodeList = value; OnPropertyChanged("ThirdNodeList"); } }
        public ObservableCollection<UniversityStructureNode> FourthNodeList { get { return _fourthNodeList; } set { _fourthNodeList = value; OnPropertyChanged("FourthNodeList"); } }

        private UniversityStructureNode _selectedFirstNode;
        private UniversityStructureNode _selectedSecondNode;
        private UniversityStructureNode _selectedThirdNode;
        private UniversityStructureNode _selectedFourthNode;
        public UniversityStructureNode SelectedFirstNode { get { return _selectedFirstNode; } set { _selectedFirstNode = value; OnPropertyChanged(nameof(SelectedFirstNode)); } }
        public UniversityStructureNode SelectedSecondNode { get { return _selectedSecondNode; } set { _selectedSecondNode = value; OnPropertyChanged(nameof(SelectedSecondNode)); } }
        public UniversityStructureNode SelectedThirdNode { get { return _selectedThirdNode; } set { _selectedThirdNode = value; OnPropertyChanged(nameof(SelectedThirdNode)); } }
        public UniversityStructureNode SelectedFourthNode { get { return _selectedFourthNode; } set { _selectedFourthNode = value; OnPropertyChanged(nameof(SelectedFourthNode)); } }

        //Списки данных из БД

        public ObservableCollection<Person> _personsList;
        public ObservableCollection<Person> PersonsList { get { return _personsList; } set { _personsList = value; OnPropertyChanged("PersonsList"); } }

        public ObservableCollection<Person> _leadNiokrSource;
        public ObservableCollection<Person> LeadNiokrSource { get { return _leadNiokrSource; } set { _leadNiokrSource = value; OnPropertyChanged("LeadNiokrSource"); } }
        private ObservableCollection<Person> _selectedLeadNIOKR;
        public ObservableCollection<Person> SelectedLeadNIOKR { get { return _selectedLeadNIOKR; } set { _selectedLeadNIOKR = value; OnPropertyChanged("SelectedLeadNIOKR"); } }

        private ObservableCollection<Person> _selectedExecutor;
        public ObservableCollection<Person> SelectedExecutor { get { return _selectedExecutor; } set { _selectedExecutor = value; OnPropertyChanged("SelectedExecutor"); } }
        public ObservableCollection<Person> _executorSource;
        public ObservableCollection<Person> ExecutorSource { get { return _executorSource; } set { _executorSource = value; OnPropertyChanged("ExecutorSource"); } }

        private ObservableCollection<Customer> _selectedCustomer;
        public ObservableCollection<Customer> SelectedCustomer { get { return _selectedCustomer; } set { _selectedCustomer = value; OnPropertyChanged("SelectedCustomer"); } }
        public ObservableCollection<Customer> _customerSource;
        public ObservableCollection<Customer> CustomerSource { get { return _customerSource; } set { _customerSource = value; OnPropertyChanged("CustomerSource"); } }

        private ObservableCollection<PriorityTrend> _selectedPriorityTrend;
        public ObservableCollection<PriorityTrend> SelectedPriorityTrend { get { return _selectedPriorityTrend; } set { _selectedPriorityTrend = value; OnPropertyChanged("SelectedPriorityTrend"); } }
        public ObservableCollection<PriorityTrend> _priorityTrendSource;
        public ObservableCollection<PriorityTrend> PriorityTrendSource { get { return _priorityTrendSource; } set { _priorityTrendSource = value; OnPropertyChanged("PriorityTrendSource"); } }

        private ObservableCollection<ScienceType> _selectedScienceType;
        public ObservableCollection<ScienceType> SelectedScienceType { get { return _selectedScienceType; } set { _selectedScienceType = value; OnPropertyChanged("SelectedScienceType"); } }
        public ObservableCollection<ScienceType> _scienceTypeSource;
        public ObservableCollection<ScienceType> ScienceTypeSource { get { return _scienceTypeSource; } set { _scienceTypeSource = value; OnPropertyChanged("ScienceTypeSource"); } }

        public ObservableCollection<Customer> CustomersList { get; set; }
        public List<Depositor> DepositsList { get; set; }
        public ObservableCollection<ScienceType> ScienceTypeList { get; set; }
        public ObservableCollection<ResearchType> ResearchTypesList { get; set; }
        public ObservableCollection<PriorityTrend> PriorityTrendList { get; set; }
        //Списки данных из формы
        public List<ComboBox> EnteredExecutorsList { get; set; }
        public List<object[]> EnteredDepositsList { get; set; }
        public List<ComboBox> EnteredScienceTypesList { get; set; }

        Grant grantToEdit;
        public string NirChecker;
        public string NOCChecker;
        public MultiSelectComboBox LeadNIOKRMultiSelectComboBox;
        public MultiSelectComboBox ExecutorMultiSelectComboBox;
        public MultiSelectComboBox CustomerMultiSelectComboBox;
        public MultiSelectComboBox ScienceTypeMultiSelectComboBox;
        public MultiSelectComboBox PriorityTrendMultiSelectComboBox;


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

        public CreateGrantWindow(DataTable grantsDataTable, Grant grantToEdit = null, MainWindow Owner = null)
        {
            InitializeComponent();

            FormsManager.CreateGrantWindow = this;

            this.grantToEdit = grantToEdit;


            LeadNiokrSource = new ObservableCollection<Person>();
            ExecutorSource = new ObservableCollection<Person>();
            CustomerSource = new ObservableCollection<Customer>();
            PriorityTrendSource = new ObservableCollection<PriorityTrend>();
            ScienceTypeSource = new ObservableCollection<ScienceType>();


            LoadDataAsync();

            DataContext = this;
        }


        /// <summary>
        /// Асинхронный метод обновления комбобоксов
        /// </summary>
        public async void UpdateDataAsync()
        {
            await Task.Run(() => UpdateData());
        }

        private void UpdateData()
        {
            PersonsList = new ObservableCollection<Person>(StaticData.GetAllPersons());
            CustomersList = CRUDDataBase.GetCustomersInNewThread();

            // Обновление комбобокса для руководителя
            Person selectedLead = null;
            bool leadIsSelected = false;
            Dispatcher.Invoke(() => leadIsSelected = SelectedLeadNIOKR.Count > 0);
            if (leadIsSelected)
            {
                Dispatcher.Invoke(() => selectedLead = new Person()
                {
                    Id = SelectedLeadNIOKR[0].Id,
                    FIO = SelectedLeadNIOKR[0].FIO
                });
            }
            Dispatcher.Invoke(() => LeadNIOKRMultiSelectComboBox.ItemsSource = PersonsList);
            if (leadIsSelected)
            {
                Dispatcher.Invoke(() => SelectedLeadNIOKR = new ObservableCollection<Person>() { selectedLead });

            }



            // Обновление комбобоксов для заказчиков
            ObservableCollection<Customer> tempCustomer = SelectedCustomer;
            SelectedCustomer.Clear();
            Dispatcher.Invoke(() => CustomerSource.Clear());
            foreach (Customer c in CustomersList)
            {
                Dispatcher.Invoke(() => CustomerSource.Add(c));
            }

            foreach (Customer c in tempCustomer)
            {
                SelectedCustomer.Add(c);
            }


            // Обновление комбобоксов у исполнителей
            ObservableCollection<Person> tempPerson = SelectedExecutor;
            SelectedExecutor.Clear();
            Dispatcher.Invoke(() => ExecutorSource.Clear());
            foreach (Person p in PersonsList)
            {
                Dispatcher.Invoke(() => ExecutorSource.Add(p));
            }

            foreach (Person p in tempPerson)
            {
                SelectedExecutor.Add(p);
            }

            // Обновление комбобокса LeadNIOKR
            ObservableCollection<Person> tempLead = SelectedLeadNIOKR;
            SelectedLeadNIOKR.Clear();
            Dispatcher.Invoke(() => LeadNiokrSource.Clear());
            foreach (Person p in PersonsList)
            {
                Dispatcher.Invoke(() => LeadNiokrSource.Add(p));
            }

            foreach (Person p in tempLead)
            {
                SelectedLeadNIOKR.Add(p);
            }
        }


        /// <summary>
        /// Асинхронный метод обновления
        /// </summary>
        private async void LoadDataAsync()
        {
            await Task.Run(() => LoadData());
        }


        /// <summary>
        /// Изначальная загрузка данных при открытии окна
        /// </summary>
        private void LoadData()
        {
            if (grantToEdit != null) Dispatcher.Invoke(() => Title = "Редактирование договора");
            string oldTitle = "";
            Dispatcher.Invoke(() => oldTitle = Title);
            Dispatcher.Invoke(() => Title = String.Format("{0} (Загрузка данных...)", Title));

            // Подключение к базе данных
            CRUDDataBase.ConnectToDataBase();
            PersonsList = new ObservableCollection<Person>(StaticData.GetAllPersons());
            //PersonsList = StaticProperties.PersonsList;
            CustomersList = CRUDDataBase.GetCustomers();
            DepositsList = CRUDDataBase.GetDeposits();
            ResearchTypesList = CRUDDataBase.GetResearchTypes();
            ScienceTypeList = CRUDDataBase.GetScienceTypes();
            PriorityTrendList = CRUDDataBase.GetPriorityTrends();
            // Закрытие подключения к базе данных
            CRUDDataBase.CloseConnection();

            // Список инвесторов
            EnteredDepositsList = new List<object[]>();
            // Список типов наук
            EnteredScienceTypesList = new List<ComboBox>();
            // Список исполнителей
            EnteredExecutorsList = new List<ComboBox>();
            SelectedLeadNIOKR = new ObservableCollection<Person>();
            SelectedExecutor = new ObservableCollection<Person>();
            SelectedFirstNode = new UniversityStructureNode();
            SelectedSecondNode = new UniversityStructureNode();
            SelectedThirdNode = new UniversityStructureNode();
            SelectedFourthNode = new UniversityStructureNode();

            Dispatcher.Invoke(() => LeadNIOKRMultiSelectComboBox = new MultiSelectComboBox()
            {
                SelectionMode = MultiSelectComboBox.SelectionModes.Single,
                ItemsSource = LeadNiokrSource,
                Margin = new Thickness(5),
                Height = 30,
                OpenDropDownListAlsoWhenNotInEditMode = true,
            });
            LeadNIOKRMultiSelectComboBox.SelectedItemsChanged += LeadNIOKRMultiSelectComboBox_SelectedItemsChanged;
            Dispatcher.Invoke(() => Grid.SetColumn(LeadNIOKRMultiSelectComboBox, 1));
            Dispatcher.Invoke(() => Grid.SetRow(LeadNIOKRMultiSelectComboBox, 3));
            Dispatcher.Invoke(() => CommonInfoGrid.Children.Add(LeadNIOKRMultiSelectComboBox));

            foreach (Person p in PersonsList)
            {
                Dispatcher.Invoke(() => LeadNiokrSource.Add(p));
                Dispatcher.Invoke(() => ExecutorSource.Add(p));
            }

            Dispatcher.Invoke(() => ExecutorMultiSelectComboBox = new MultiSelectComboBox()
            {
                ItemsSource = ExecutorSource,
                Margin = new Thickness(5),
                Height = 420,
                OpenDropDownListAlsoWhenNotInEditMode = true,
                VerticalAlignment = VerticalAlignment.Top,
            });
            Dispatcher.Invoke(() => Grid.SetColumn(ExecutorMultiSelectComboBox, 1));
            Dispatcher.Invoke(() => Grid.SetRow(ExecutorMultiSelectComboBox, 1));
            Dispatcher.Invoke(() => customersAndExecutorsGrid.Children.Add(ExecutorMultiSelectComboBox));

            Dispatcher.Invoke(() => CustomerMultiSelectComboBox = new MultiSelectComboBox()
            {
                ItemsSource = CustomerSource,
                Margin = new Thickness(5),
                Height = 420,
                OpenDropDownListAlsoWhenNotInEditMode = true,
                VerticalAlignment = VerticalAlignment.Top,
            });
            Dispatcher.Invoke(() => Grid.SetColumn(CustomerMultiSelectComboBox, 0));
            Dispatcher.Invoke(() => Grid.SetRow(CustomerMultiSelectComboBox, 1));
            Dispatcher.Invoke(() => customersAndExecutorsGrid.Children.Add(CustomerMultiSelectComboBox));

            foreach (Customer c in CustomersList)
            {
                Dispatcher.Invoke(() => CustomerSource.Add(c));
            }

            Dispatcher.Invoke(() => ScienceTypeMultiSelectComboBox = new MultiSelectComboBox()
            {
                ItemsSource = ScienceTypeSource,
                Margin = new Thickness(5),
                Height = 350,
                OpenDropDownListAlsoWhenNotInEditMode = true,
                VerticalAlignment = VerticalAlignment.Top,
            });

            Dispatcher.Invoke(() => Grid.SetColumn(ScienceTypeMultiSelectComboBox, 1));
            Dispatcher.Invoke(() => Grid.SetRow(ScienceTypeMultiSelectComboBox, 3));
            Dispatcher.Invoke(() => researchTypesGrid.Children.Add(ScienceTypeMultiSelectComboBox));

            foreach (ScienceType s in ScienceTypeList)
            {
                Dispatcher.Invoke(() => ScienceTypeSource.Add(s));
            }

            Dispatcher.Invoke(() => PriorityTrendMultiSelectComboBox = new MultiSelectComboBox()
            {
                ItemsSource = PriorityTrendSource,
                Margin = new Thickness(5),
                Height = 350,
                OpenDropDownListAlsoWhenNotInEditMode = true,
                VerticalAlignment = VerticalAlignment.Top,
            });
            Dispatcher.Invoke(() => Grid.SetColumn(PriorityTrendMultiSelectComboBox, 0));
            Dispatcher.Invoke(() => Grid.SetRow(PriorityTrendMultiSelectComboBox, 3));
            Dispatcher.Invoke(() => researchTypesGrid.Children.Add(PriorityTrendMultiSelectComboBox));

            foreach (PriorityTrend p in PriorityTrendList)
            {
                Dispatcher.Invoke(() => PriorityTrendSource.Add(p));
            }

            Dispatcher.Invoke(() => researchTypeComboBox.ItemsSource = new ObservableCollection<ResearchType>(ResearchTypesList));

            FirstNodeList = new ObservableCollection<UniversityStructureNode>();
            SecondNodeList = new ObservableCollection<UniversityStructureNode>();
            ThirdNodeList = new ObservableCollection<UniversityStructureNode>();
            FourthNodeList = new ObservableCollection<UniversityStructureNode>();

            priceTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            priceNoNDSTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;


            SelectedCustomer = new ObservableCollection<Customer>();
            SelectedLeadNIOKR = new ObservableCollection<Person>();
            SelectedExecutor = new ObservableCollection<Person>();
            SelectedPriorityTrend = new ObservableCollection<PriorityTrend>();
            SelectedScienceType = new ObservableCollection<ScienceType>();
            //Dispatcher.Invoke(() => BindingOperations.EnableCollectionSynchronization(SelectedExecutor, _collectionOfObjectsSync));

            // Если открыта форма редактирования, то вставим в нее данные
            if (grantToEdit != null)
            {
                _isEditGrant = true;
                grantEditId = grantToEdit.Id;
                grantNumber = grantToEdit.grantNumber;

                Dispatcher.Invoke(() => DeleteGrantButton.Visibility = System.Windows.Visibility.Visible);
                Dispatcher.Invoke(() => createGrantButton.Content = "Сохранить");
                Dispatcher.Invoke(() => OKVEDTextBox.Text = grantToEdit.OKVED);
                Dispatcher.Invoke(() => grantNumberTextBox.Text = grantToEdit.grantNumber);
                Dispatcher.Invoke(() => NIOKRTextBox.Text = grantToEdit.NameNIOKR);

                foreach (Customer c in grantToEdit.Customer)
                {
                    foreach (Customer c1 in CustomerSource)
                        if (c1.Id == c.Id)
                        {
                            SelectedCustomer.Add(c1);
                            break;
                        }
                }

                Dispatcher.Invoke(() => startDateDatePicker.SelectedDate = grantToEdit.StartDate);
                Dispatcher.Invoke(() => endDateDatePicker.SelectedDate = grantToEdit.EndDate);
                Dispatcher.Invoke(() => priceTextBox.Text = String.Format("{0:#,0.##}", grantToEdit.Price));
                Dispatcher.Invoke(() => priceNoNDSTextBox.Text = String.Format("{0:#,0.##}", grantToEdit.PriceNoNDS));
                Dispatcher.Invoke(() => GrantWithoutNDSCheckBox.IsChecked = !grantToEdit.isWIthNDS);

                // Добавление источников финансирования
                for (int i = 0; i < grantToEdit.Depositor.Count; i++)
                {
                    StackPanel horizontalStackPanel = null;
                    Dispatcher.Invoke(() => horizontalStackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                    });

                    TextBox sumTextBox = null;
                    TextBox sumTextBoxNoNDS = null;
                    void sumTextBoxTextChangedEventHandler(object senderr, TextChangedEventArgs args)
                    {
                        if (sumTextBox.Text.Length > 0)
                        {
                            sumTextBoxNoNDS.Text = String.Format("{0:#,0.##}", Math.Round(Convert.ToDouble(sumTextBox.Text) * 1 / ((bool)GrantWithoutNDSCheckBox.IsChecked ? 1 : Settings.Default.NDSValue), 2));

                            //Если пользователь поставил запятую, то чтобы она не сбрасывалась
                            if (sumTextBox.Text[sumTextBox.Text.Length - 1] != ',' && !sumTextBox.Text.Contains(",0"))
                            {
                                int lengthToComma = 0;
                                int commaIndex = sumTextBox.Text.ToString().IndexOf(',');
                                lengthToComma = commaIndex > 0 ? commaIndex : 0;

                                // запомнить, где текущий индекс сейчас курсора в текстбоксе
                                int indexCursor = lengthToComma % 4 == 0 ? sumTextBox.CaretIndex + 1 : sumTextBox.CaretIndex;
                                sumTextBox.Text = Convert.ToDouble(sumTextBox.Text) < 0.0000001 ? "" : String.Format("{0:#,0.#####}", Convert.ToDouble(sumTextBox.Text));
                                sumTextBox.SelectionStart = indexCursor;
                            }
                        }
                        else
                            sumTextBoxNoNDS.Text = "";

                        CalculateDepositorsSum();
                        CalculateDepositorsSumNoNDS();
                    }

                    void sumTextBoxNoNDSTextChangedEventHandler(object senderr, TextChangedEventArgs args)
                    {
                        CalculateDepositorsSumNoNDS();
                    }


                    ComboBox depositorComboBox = null;
                    Dispatcher.Invoke(() => depositorComboBox = new ComboBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        ItemsSource = DepositsList,
                        Width = 160,
                    });
                    for (int j = 0; j < DepositsList.Count; j++)
                        if (DepositsList[j].Title == grantToEdit.Depositor[i].Title)
                            Dispatcher.Invoke(() => depositorComboBox.SelectedIndex = j);

                    Dispatcher.Invoke(() => sumTextBox = new TextBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        Width = 110,
                        Text = String.Format("{0:#,0.##}", grantToEdit.DepositorSum[i]),
                    });


                    Dispatcher.Invoke(() => sumTextBoxNoNDS = new TextBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        Width = 110,
                        Text = String.Format("{0:#,0.##}", grantToEdit.DepositorSumNoNDS[i])
                    });
                    Dispatcher.Invoke(() => sumTextBoxNoNDS.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput);
                    Dispatcher.Invoke(() => sumTextBoxNoNDS.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown);
                    Dispatcher.Invoke(() => sumTextBoxNoNDS.TextChanged += sumTextBoxNoNDSTextChangedEventHandler);

                    Dispatcher.Invoke(() => sumTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput);
                    Dispatcher.Invoke(() => sumTextBox.TextChanged += sumTextBoxTextChangedEventHandler);
                    Dispatcher.Invoke(() => sumTextBox.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown);


                    DateTime selectedDate;
                    DateTime.TryParse(grantToEdit.ReceiptDate[i], out selectedDate);

                    DatePicker dateComboBox = null;
                    Dispatcher.Invoke(() => dateComboBox = new DatePicker()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        Width = 110,
                        SelectedDate = selectedDate
                    });

                    Dispatcher.Invoke(() => horizontalStackPanel.Children.Add(depositorComboBox));
                    Dispatcher.Invoke(() => horizontalStackPanel.Children.Add(sumTextBox));
                    Dispatcher.Invoke(() => horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 0, 0) }));
                    Dispatcher.Invoke(() => horizontalStackPanel.Children.Add(sumTextBoxNoNDS));
                    Dispatcher.Invoke(() => horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 5, 0) }));
                    Dispatcher.Invoke(() => horizontalStackPanel.Children.Add(dateComboBox));


                    Dispatcher.Invoke(() => depositsVerticalListView.Items.Add(horizontalStackPanel));
                }
                Dispatcher.Invoke(() => CalculateDepositorsSum());
                Dispatcher.Invoke(() => CalculateDepositorsSumNoNDS());

                if (grantToEdit.LeadNIOKR != null)
                {
                    SelectedLeadNIOKR.Add(grantToEdit.LeadNIOKR);
                }

                foreach (Person p in grantToEdit.Executor)
                {
                    foreach (Person p1 in ExecutorSource)
                        if (p.Id == p1.Id)
                        {
                            Console.WriteLine("Добавили " + p.FIO);
                            SelectedExecutor.Add(p1);
                            break;
                        }
                }


                Dispatcher.Invoke(() => GRNTITextBox.Text = grantToEdit.GRNTI);

                if (grantToEdit.ResearchType.Count > 0)
                {
                    for (int j = 0; j < ResearchTypesList.Count; j++)
                        if (ResearchTypesList[j].Title == grantToEdit.ResearchType[0].Title)
                            Dispatcher.Invoke(() => researchTypeComboBox.SelectedIndex = j);
                }

                foreach (PriorityTrend p in grantToEdit.PriorityTrands)
                {
                    foreach (PriorityTrend p1 in PriorityTrendSource)
                        if (p1.Id == p.Id)
                        {
                            SelectedPriorityTrend.Add(p1);
                            break;
                        }
                }


                foreach (ScienceType s in grantToEdit.ScienceType)
                {
                    foreach (ScienceType s1 in ScienceTypeSource)
                        if (s.Id == s1.Id)
                        {
                            SelectedScienceType.Add(s1);
                            break;
                        }
                }

                switch (grantToEdit.NIR)
                {
                    case "НИР":
                        Dispatcher.Invoke(() => NIR.IsChecked = true);
                        break;
                    case "УСЛУГА":
                        Dispatcher.Invoke(() => USLUGA.IsChecked = true);
                        break;
                }

                switch (grantToEdit.NOC)
                {
                    case "True":
                        Dispatcher.Invoke(() => NOC.IsChecked = true);
                        break;
                    case "False":
                        Dispatcher.Invoke(() => NotNOC.IsChecked = true);
                        break;
                }
            }

            Binding executorBinding = new Binding("SelectedExecutor");
            executorBinding.Source = this;
            Dispatcher.Invoke(() => ExecutorMultiSelectComboBox.SetBinding(MultiSelectComboBox.SelectedItemsProperty, executorBinding));

            Binding priorityTrendBinding = new Binding("SelectedPriorityTrend");
            priorityTrendBinding.Source = this;
            Dispatcher.Invoke(() => PriorityTrendMultiSelectComboBox.SetBinding(MultiSelectComboBox.SelectedItemsProperty, priorityTrendBinding));

            Binding scienceTypeBinding = new Binding("SelectedScienceType");
            scienceTypeBinding.Source = this;
            Dispatcher.Invoke(() => ScienceTypeMultiSelectComboBox.SetBinding(MultiSelectComboBox.SelectedItemsProperty, scienceTypeBinding));

            Binding customerBinding = new Binding("SelectedCustomer");
            customerBinding.Source = this;
            Dispatcher.Invoke(() => CustomerMultiSelectComboBox.SetBinding(MultiSelectComboBox.SelectedItemsProperty, customerBinding));

            Binding LeadNIOKRBinding = new Binding("SelectedLeadNIOKR");
            LeadNIOKRBinding.Source = this;
            Dispatcher.Invoke(() => LeadNIOKRMultiSelectComboBox.SetBinding(MultiSelectComboBox.SelectedItemsProperty, LeadNIOKRBinding));

            Dispatcher.Invoke(() => Title = oldTitle);
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
                {
                    sumTextBoxNoNDS.Text = String.Format("{0:#,0.##}", Math.Round(Convert.ToDouble(sumTextBox.Text) * 1 / ((bool)GrantWithoutNDSCheckBox.IsChecked ? 1 : Settings.Default.NDSValue), 2));

                    ///Если пользователь поставил запятую, то чтобы она не сбрасывалась
                    if (sumTextBox.Text[sumTextBox.Text.Length - 1] != ',' && !sumTextBox.Text.Contains(",0"))
                    {
                        int lengthToComma = 0;
                        int commaIndex = sumTextBox.Text.ToString().IndexOf(',');
                        lengthToComma = commaIndex > 0 ? commaIndex : 0;

                        // запомнить, где текущий индекс сейчас курсора в текстбоксе
                        int indexCursor = lengthToComma % 4 == 0 ? sumTextBox.CaretIndex + 1 : sumTextBox.CaretIndex;
                        sumTextBox.Text = Convert.ToDouble(sumTextBox.Text) < 0.0000001 ? "" : String.Format("{0:#,0.#####}", Convert.ToDouble(sumTextBox.Text));
                        sumTextBox.SelectionStart = indexCursor;
                    }
                }
                else
                    sumTextBoxNoNDS.Text = "";

                CalculateDepositorsSum();
                CalculateDepositorsSumNoNDS();
            }

            void sumTextBoxNoNDSTextChangedEventHandler(object senderr, TextChangedEventArgs args)
            {
                CalculateDepositorsSumNoNDS();
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
            };

            sumTextBoxNoNDS = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                Width = 110,
                Padding = new Thickness(0, 2, 0, 2)
            };
            sumTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            sumTextBox.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;
            sumTextBox.TextChanged += sumTextBoxTextChangedEventHandler;

            sumTextBoxNoNDS.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            sumTextBoxNoNDS.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;
            sumTextBoxNoNDS.TextChanged += sumTextBoxNoNDSTextChangedEventHandler;

            DatePicker datePicker = new DatePicker()
            {
                Margin = new Thickness(5, 0, 5, 10),
                Width = 110
            };

            horizontalStackPanel.Children.Add(depositorComboBox);
            horizontalStackPanel.Children.Add(sumTextBox);
            horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 0, 0) });
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
                //MessageBox.Show("Выделите нужный для удаления элемент", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            foreach (Customer c in SelectedCustomer)
            {
                newGrant.Customer.Add(c);
            }

            if (startDateDatePicker.SelectedDate != null)
            {
                newGrant.StartDate = startDateDatePicker.SelectedDate;
            }

            if (endDateDatePicker.SelectedDate != null)
            {
                newGrant.EndDate = endDateDatePicker.SelectedDate;
            }

            if (priceTextBox.Text.ToString() != "")
            {
                newGrant.Price = Double.Parse(priceTextBox.Text);
            }

            if (priceNoNDSTextBox.Text.ToString() != "")
            {
                newGrant.PriceNoNDS = Double.Parse(priceNoNDSTextBox.Text);
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
                    {
                        if (partSumNoNDS.Text.ToString() != "" && partSum.Text.ToString() != "")
                        {
                            newGrant.Depositor.Add(new Depositor()
                            {
                                Id = ((Depositor)cmb.SelectedItem).Id,
                                Title = cmb.SelectedItem.ToString(),
                            });
                            newGrant.DepositorSum.Add(Double.Parse(partSum.Text));
                            newGrant.DepositorSumNoNDS.Add(Double.Parse(partSumNoNDS.Text));
                            newGrant.ReceiptDate.Add(selectedDate.ToShortDateString());
                        }
                    }
                }
            }

            if (SelectedLeadNIOKR.Count > 0)
            {
                newGrant.LeadNIOKR = new Person()
                {
                    Id = SelectedLeadNIOKR[0].Id,
                    FIO = SelectedLeadNIOKR[0].FIO
                };
            }
            else
            {
                newGrant.LeadNIOKR = null;
            }

            foreach (Person p in SelectedExecutor)
            {
                newGrant.Executor.Add(p);
            }

            if (FirstNodeComboBox.SelectedItem != null)
            {
                newGrant.FirstNode = (UniversityStructureNode)FirstNodeComboBox.SelectedItem;
            }
            else
            {
                newGrant.FirstNode = new UniversityStructureNode();
            }

            if (SecondNodeComboBox.SelectedItem != null)
            {
                newGrant.SecondNode = (UniversityStructureNode)SecondNodeComboBox.SelectedItem;
            }
            else
            {
                newGrant.SecondNode = new UniversityStructureNode();
            }

            if (ThirdNodeComboBox.SelectedItem != null)
            {
                newGrant.ThirdNode = (UniversityStructureNode)ThirdNodeComboBox.SelectedItem;
            }
            else
            {
                newGrant.ThirdNode = new UniversityStructureNode();
            }

            if (FourthComboBox.SelectedItem != null)
            {
                newGrant.FourthNode = (UniversityStructureNode)FourthComboBox.SelectedItem;
            }
            else
            {
                newGrant.FourthNode = new UniversityStructureNode();
            }


            if (GRNTITextBox.Text != "")
            {
                newGrant.GRNTI = GRNTITextBox.Text;
            }
            else
            {
                newGrant.GRNTI = "";
            }

            if (researchTypeComboBox.SelectedItem != null)
            {
                newGrant.ResearchType.Add(new ResearchType()
                {
                    Id = ((ResearchType)researchTypeComboBox.SelectedItem).Id,
                    Title = researchTypeComboBox.SelectedItem.ToString()
                });
            }

            foreach (PriorityTrend p in SelectedPriorityTrend)
            {
                newGrant.PriorityTrands.Add(p);
            }

            foreach (ScienceType s in SelectedScienceType)
            {
                newGrant.ScienceType.Add(s);
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
                    CRUDDataBase.UpdateWorkPlace(newGrant);
                    CRUDDataBase.UpdateExecutors(newGrant);
                    CRUDDataBase.UpdateGRNTI(newGrant);
                    CRUDDataBase.UpdateResearchType(newGrant);
                    CRUDDataBase.UpdatePriorityTrends(newGrant);
                    CRUDDataBase.UpdateScienceTypes(newGrant);
                    CRUDDataBase.UpdateNIR(newGrant);
                    CRUDDataBase.UpdateNOC(newGrant);
                    CRUDDataBase.UpdateIsWithNDS(newGrant);

                    // Закрываем соединение с БД
                    CRUDDataBase.CloseConnection();

                    ((MainWindow)Owner).ReloadGrantsWithFilters();
                    MessageBox.Show("Информация о договоре успешно изменена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                // Если создание нового договора
                else
                {
                    // Подключаюсь к БД
                    CRUDDataBase.ConnectToDataBase();

                    if (CRUDDataBase.IsGrantAlreadyExists(newGrant))
                    {
                        MessageBoxResult sure = MessageBox.Show("Договор с такими номером и наименованием НИОКР уже существует.\nВсё равно добавить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        switch (sure)
                        {
                            case MessageBoxResult.Yes:
                                CRUDDataBase.InsertNewGrantToDB(newGrant);
                                MessageBox.Show("Договор успешно создан", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                                ((MainWindow)Owner).ReloadGrantsWithFilters();
                                Close();
                                break;
                            case MessageBoxResult.No:
                                break;
                        }
                    }
                    else
                    {
                        CRUDDataBase.InsertNewGrantToDB(newGrant);
                        // Закрываем соединение с БД

                        MessageBox.Show("Договор успешно создан", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        ((MainWindow)Owner).ReloadGrantsWithFilters();
                        Close();
                    }
                    CRUDDataBase.CloseConnection();
                }
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
                    ((MainWindow)Owner).ReloadGrantsWithFilters();
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
            if (priceTextBox.Text.Length > 0)
            {
                priceNoNDSTextBox.Text = String.Format("{0:#,0.##}", Math.Round(Convert.ToDouble(priceTextBox.Text) * 1 / ((bool)GrantWithoutNDSCheckBox.IsChecked ? 1 : Settings.Default.NDSValue), 2));

                //Если пользователь поставил запятую, то чтобы она не сбрасывалась
                if (priceTextBox.Text[priceTextBox.Text.Length - 1] != ',' && !priceTextBox.Text.Contains(",0") && !priceTextBox.Text.Contains(",00"))
                {
                    // запомнить, где текущий индекс сейчас курсора в текстбоксе
                    int index = priceTextBox.CaretIndex;
                    priceTextBox.Text = Convert.ToDouble(priceTextBox.Text) < 0.0000001 ? "" : String.Format("{0:#,0.#####}", Convert.ToDouble(priceTextBox.Text));
                    priceTextBox.SelectionStart = index;
                }
            }
            else
                priceNoNDSTextBox.Text = "";

            TextBox partSum;
            TextBox partSumNoNDS;

            foreach (StackPanel sp in depositsVerticalListView.Items.OfType<StackPanel>())
            {
                partSum = (TextBox)sp.Children[1];
                partSumNoNDS = (TextBox)sp.Children[3];
                if (partSumNoNDS.Text.Length > 0)
                {
                    partSumNoNDS.Text = String.Format("{0:#,0.##}", Math.Round(Convert.ToDouble(partSum.Text) * 1 / ((bool)GrantWithoutNDSCheckBox.IsChecked ? 1 : Settings.Default.NDSValue), 2));
                }
                else
                {
                    partSumNoNDS.Text = "";
                }
            }

            CalculateDepositorsSumNoNDS();
        }

        private void priceNoNDSTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Изменение значения в textbox цена договора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void priceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (priceTextBox.Text.Length > 0)
            {
                priceNoNDSTextBox.Text = String.Format("{0:#,0.##}", Math.Round(Convert.ToDouble(priceTextBox.Text) * 1 / ((bool)GrantWithoutNDSCheckBox.IsChecked ? 1 : Settings.Default.NDSValue) /*Settings.Default.NDSValue*/, 2));

                //Если пользователь поставил запятую, то чтобы она не сбрасывалась
                if (priceTextBox.Text[priceTextBox.Text.Length - 1] != ',' && !priceTextBox.Text.Contains(",0") && !priceTextBox.Text.Contains(",00"))
                {
                    int lengthToComma = 0;
                    int commaIndex = priceTextBox.Text.ToString().IndexOf(',');
                    lengthToComma = commaIndex > 0 ? commaIndex : 0;

                    // запомнить, где текущий индекс сейчас курсора в текстбоксе
                    int indexCursor = lengthToComma % 4 == 0 ? priceTextBox.CaretIndex + 1 : priceTextBox.CaretIndex;
                    priceTextBox.Text = Convert.ToDouble(priceTextBox.Text) < 0.0000001 ? "" : String.Format("{0:#,0.#####}", Convert.ToDouble(priceTextBox.Text));
                    priceTextBox.SelectionStart = indexCursor;
                }
            }
            else
                priceNoNDSTextBox.Text = "";
        }

        private void LeadNIOKRMultiSelectComboBox_SelectedItemsChanged(object sender, Sdl.MultiSelectComboBox.EventArgs.SelectedItemsChangedEventArgs e)
        {
            if (SelectedLeadNIOKR.Count == 0) return;
            FirstNodeList = new ObservableCollection<UniversityStructureNode>();
            SecondNodeList = new ObservableCollection<UniversityStructureNode>();
            ThirdNodeList = new ObservableCollection<UniversityStructureNode>();
            FourthNodeList = new ObservableCollection<UniversityStructureNode>();
            Person person = SelectedLeadNIOKR[0];
            CRUDDataBase.ConnectToDataBase();

            HashSet<String> set = new HashSet<String>();
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllFirstNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    FirstNodeList.Add(u);
                    set.Add(u.Title);
                }
                if ((u.IsMainWorkPlace && u.Id != -1) || (grantToEdit != null && grantToEdit.FirstNode.Id == u.Id))
                {
                    for (int i = 0; i < FirstNodeList.Count; i++)
                    {
                        if (u.Id == FirstNodeList[i].Id)
                            SelectedFirstNode = FirstNodeList[i];
                    }
                }
            }
            set.Clear();
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllSecondNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    SecondNodeList.Add(u);
                    set.Add(u.Title);
                }
                if ((u.IsMainWorkPlace && u.Id != -1) || (grantToEdit != null && grantToEdit.SecondNode.Id == u.Id))
                {
                    for (int i = 0; i < SecondNodeList.Count; i++)
                    {
                        if (u.Id == SecondNodeList[i].Id)
                            SelectedSecondNode = SecondNodeList[i];
                    }
                }
            }
            set.Clear();
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllThirdNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    ThirdNodeList.Add(u);
                    set.Add(u.Title);
                }
                if ((u.IsMainWorkPlace && u.Id != -1) || (grantToEdit != null && grantToEdit.ThirdNode.Id == u.Id))
                {
                    for (int i = 0; i < ThirdNodeList.Count; i++)
                    {
                        if (u.Id == ThirdNodeList[i].Id)
                            SelectedThirdNode = ThirdNodeList[i];
                    }
                }
            }
            set.Clear();
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllFourthNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    FourthNodeList.Add(u);
                    set.Add(u.Title);
                }
                if ((u.IsMainWorkPlace && u.Id != -1) || (grantToEdit !=null && grantToEdit.FourthNode.Id == u.Id))
                {
                    for (int i = 0; i < FourthNodeList.Count; i++)
                    {
                        if (u.Id == FourthNodeList[i].Id)
                            SelectedFourthNode = FourthNodeList[i];
                    }
                }
            }
            set.Clear();
            CRUDDataBase.CloseConnection();
        }

        /// <summary>
        /// Подсчет общей суммы вкладчиков. Считается c НДС
        /// Результат выводится в окно под договорами.
        /// </summary>
        private void CalculateDepositorsSum()
        {
            Double sumDeposits = 0;

            foreach (StackPanel sp in depositsVerticalListView.Items.OfType<StackPanel>())
            {
                TextBox partSum = (TextBox)sp.Children[1];

                sumDeposits += partSum.Text != "" ? Double.Parse(partSum.Text) : 0;
            }

            sumDepositsTextBox.Text = String.Format("{0:#,0.##}", sumDeposits);
        }


        /// <summary>
        /// Подсчет общей суммы вкладчиков. Считается без НДС
        /// Результат выводится в окно под договорами.
        /// </summary>
        private void CalculateDepositorsSumNoNDS()
        {
            Double sumDeposits = 0;

            foreach (StackPanel sp in depositsVerticalListView.Items.OfType<StackPanel>())
            {
                TextBox partSum = (TextBox)sp.Children[3];

                sumDeposits += partSum.Text != "" ? Double.Parse(partSum.Text) : 0;
            }

            sumDepositsNoNDSTextBox.Text = String.Format("{0:#,0.##}", sumDeposits);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            FormsManager.CreateGrantWindow = null;
        }
    }
}
