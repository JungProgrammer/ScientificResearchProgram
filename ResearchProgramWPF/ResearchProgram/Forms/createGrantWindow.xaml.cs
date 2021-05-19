using DotNetKit.Windows.Controls;
using Npgsql;
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
        private ObservableCollection<Person> _selectedLeadNIOKR;
        public ObservableCollection<Person> SelectedLeadNIOKR { get { return _selectedLeadNIOKR; } set { _selectedLeadNIOKR = value; OnPropertyChanged("SelectedLeadNIOKR"); } }

        //Списки данных из БД

        public ObservableCollection<Person> _personsList;
        public ObservableCollection<Person> PersonsList { get { return _personsList; } set { _personsList = value; OnPropertyChanged("PersonsList"); } }

        public ObservableCollection<Person> _leadNiokrSource;
        public ObservableCollection<Person> LeadNiokrSource { get { return _leadNiokrSource; } set { _leadNiokrSource = value; OnPropertyChanged("LeadNiokrSource"); } }

        public ObservableCollection<Customer> CustomersList { get; set; }

        public List<Depositor> DepositsList { get; set; }
        public ObservableCollection<ScienceType> ScienceTypeList { get; set; }
        public ObservableCollection<ResearchType> ResearchTypesList { get; set; }
        public ObservableCollection<PriorityTrend> PriorityTrendList { get; set; }
        //Списки данных из формы
        public List<ComboBox> EnteredExecutorsList { get; set; }
        public List<Object[]> EnteredDepositsList { get; set; }
        public List<ComboBox> EnteredScienceTypesList { get; set; }

        Grant grantToEdit;
        public string NirChecker;
        public string NOCChecker;
        public MultiSelectComboBox LeadNIOKRMultiSelectComboBox;


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
            PersonsList = CRUDDataBase.GetPersonsInNewThread();
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
                //for (int i = 0; i < PersonsList.Count; i++)
                //{
                //    if (PersonsList[i].Id == selectedLead.Id)
                //    {
                //    }
                //}
                Dispatcher.Invoke(() => SelectedLeadNIOKR = new ObservableCollection<Person>() { selectedLead });

            }



            // Обновление комбобоксов для заказчиков
            Customer selectedCustomer = null;
            foreach (AutoCompleteComboBox cmb in customersVerticalListView.Items.OfType<AutoCompleteComboBox>())
            {
                bool isCmbItemSelected = false;
                Dispatcher.Invoke(() => isCmbItemSelected = cmb.SelectedItem != null);

                if (isCmbItemSelected)
                {
                    Dispatcher.Invoke(() => selectedCustomer = new Customer()
                    {
                        Id = ((Customer)cmb.SelectedItem).Id,
                        Title = ((Customer)cmb.SelectedItem).Title
                    });
                }


                Dispatcher.Invoke(() => cmb.ItemsSource = CustomersList);


                if (isCmbItemSelected)
                {
                    for (int i = 0; i < CustomersList.Count; i++)
                    {
                        if (CustomersList[i].Id == selectedCustomer.Id)
                        {
                            Dispatcher.Invoke(() => cmb.SelectedItem = CustomersList[i]);
                        }
                    }
                }
            }


            // Обновление комбобоксов у исполнителей
            Person selectedExecutor = null;
            foreach (AutoCompleteComboBox cmb in executorsVerticalListView.Items.OfType<AutoCompleteComboBox>())
            {
                bool isCmbItemSelected = false;
                Dispatcher.Invoke(() => isCmbItemSelected = cmb.SelectedItem != null);

                if (isCmbItemSelected)
                {
                    Dispatcher.Invoke(() => selectedExecutor = new Person()
                    {
                        Id = ((Person)cmb.SelectedItem).Id,
                        FIO = ((Person)cmb.SelectedItem).FIO
                    });
                }

                Dispatcher.Invoke(() => cmb.ItemsSource = PersonsList);

                if (isCmbItemSelected)
                {
                    for (int i = 0; i < PersonsList.Count; i++)
                    {
                        if (PersonsList[i].Id == selectedExecutor.Id)
                        {
                            Dispatcher.Invoke(() => cmb.SelectedItem = PersonsList[i]);
                        }
                    }
                }
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
            PersonsList = CRUDDataBase.GetPersons();
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

            Dispatcher.Invoke(() => LeadNIOKRMultiSelectComboBox = new MultiSelectComboBox()
            {
                SelectionMode = MultiSelectComboBox.SelectionModes.Single,
                ItemsSource = LeadNiokrSource,
                Margin = new Thickness(5),
                Height = 30,
                OpenDropDownListAlsoWhenNotInEditMode = true,
            });

            Binding myBinding = new Binding("SelectedLeadNIOKR");
            myBinding.Source = this;
            Dispatcher.Invoke(() => LeadNIOKRMultiSelectComboBox.SetBinding(MultiSelectComboBox.SelectedItemsProperty, myBinding));
            LeadNIOKRMultiSelectComboBox.SelectedItemsChanged += LeadNIOKRMultiSelectComboBox_SelectedItemsChanged; ;
            Dispatcher.Invoke(() => Grid.SetColumn(LeadNIOKRMultiSelectComboBox, 1));
            Dispatcher.Invoke(() => Grid.SetRow(LeadNIOKRMultiSelectComboBox, 3));
            Dispatcher.Invoke(() => CommonInfoGrid.Children.Add(LeadNIOKRMultiSelectComboBox));

            foreach (Person p in PersonsList)
            {
                Dispatcher.Invoke(() => LeadNiokrSource.Add(p));
            }
            Dispatcher.Invoke(() => researchTypeComboBox.ItemsSource = new ObservableCollection<ResearchType>(ResearchTypesList));

            FirstNodeList = new ObservableCollection<UniversityStructureNode>();
            SecondNodeList = new ObservableCollection<UniversityStructureNode>();
            ThirdNodeList = new ObservableCollection<UniversityStructureNode>();
            FourthNodeList = new ObservableCollection<UniversityStructureNode>();


            priceTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            priceNoNDSTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;



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

                for (int i = 0; i < grantToEdit.Customer.Count; i++)
                {
                    AutoCompleteComboBox customerComboBox = null;
                    Dispatcher.Invoke(() => customerComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5),
                        ItemsSource = new List<Customer>(CustomersList),
                        Width = 270
                    });

                    for (int j = 0; j < CustomersList.Count; j++)
                        if (CustomersList[j].Title == grantToEdit.Customer[i].Title)
                            Dispatcher.Invoke(() => customerComboBox.SelectedIndex = j);

                    Dispatcher.Invoke(() => customersVerticalListView.Items.Add(customerComboBox));
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
                                // запомнить, где текущий индекс сейчас курсора в текстбоксе
                                int index = sumTextBox.CaretIndex;
                                sumTextBox.Text = Convert.ToDouble(sumTextBox.Text) < 0.0000001 ? "" : String.Format("{0:#,0.#####}", Convert.ToDouble(sumTextBox.Text));
                                sumTextBox.SelectionStart = index;
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


                //for (int i = 0; i < PersonsList.Count; i++)
                //    if (PersonsList[i].FIO == grantToEdit.LeadNIOKR.FIO)
                //        Dispatcher.Invoke(() => LeadNIOKRAutoCompleteComboBox.SelectedIndex = i);
                if (grantToEdit.LeadNIOKR.FIO != "")
                {
                    SelectedLeadNIOKR = new ObservableCollection<Person>();
                    SelectedLeadNIOKR.Add(grantToEdit.LeadNIOKR);
                }


                for (int i = 0; i < grantToEdit.Executor.Count; i++)
                {
                    AutoCompleteComboBox executorComboBox = null;
                    Dispatcher.Invoke(() => executorComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5),
                        ItemsSource = new List<Person>(PersonsList),
                        Width = 270
                    });

                    for (int j = 0; j < PersonsList.Count; j++)
                        if (PersonsList[j].FIO == grantToEdit.Executor[i].FIO)
                            Dispatcher.Invoke(() => executorComboBox.SelectedIndex = j);

                    Dispatcher.Invoke(() => executorsVerticalListView.Items.Add(executorComboBox));
                }

                Dispatcher.Invoke(() => FirstNodeComboBox.SelectedIndex = -1);
                if (grantToEdit.FirstNode != null)
                {
                    if (grantToEdit.FirstNode.Title != null)
                    {
                        for (int i = 0; i < FirstNodeList.Count; i++)
                        {
                            if (grantToEdit.FirstNode.Id == FirstNodeList[i].Id)
                            {
                                Dispatcher.Invoke(() => FirstNodeComboBox.SelectedIndex = i);
                            }
                        }
                    }
                }

                Dispatcher.Invoke(() => SecondNodeComboBox.SelectedIndex = -1);
                if (grantToEdit.SecondNode != null)
                {
                    if (grantToEdit.SecondNode.Title != null)
                    {
                        for (int i = 0; i < SecondNodeList.Count; i++)
                        {
                            if (grantToEdit.SecondNode.Id == SecondNodeList[i].Id)
                            {
                                Dispatcher.Invoke(() => SecondNodeComboBox.SelectedIndex = i);
                            }
                        }
                    }
                }

                Dispatcher.Invoke(() => ThirdNodeComboBox.SelectedIndex = -1);
                if (grantToEdit.ThirdNode != null)
                {
                    if (grantToEdit.ThirdNode.Title != null)
                    {
                        for (int i = 0; i < ThirdNodeList.Count; i++)
                        {
                            if (grantToEdit.ThirdNode.Id == ThirdNodeList[i].Id)
                            {
                                Dispatcher.Invoke(() => ThirdNodeComboBox.SelectedIndex = i);
                            }
                        }
                    }
                }

                Dispatcher.Invoke(() => FourthComboBox.SelectedIndex = -1);
                if (grantToEdit.FourthNode != null)
                {
                    if (grantToEdit.FourthNode.Title != null)
                    {
                        for (int i = 0; i < FourthNodeList.Count; i++)
                        {
                            if (grantToEdit.FourthNode.Id == FourthNodeList[i].Id)
                            {
                                Dispatcher.Invoke(() => FourthComboBox.SelectedIndex = i);
                            }
                        }
                    }
                }

                Dispatcher.Invoke(() => GRNTITextBox.Text = grantToEdit.GRNTI);

                if (grantToEdit.ResearchType.Count > 0)
                {
                    for (int j = 0; j < ResearchTypesList.Count; j++)
                        if (ResearchTypesList[j].Title == grantToEdit.ResearchType[0].Title)
                            Dispatcher.Invoke(() => researchTypeComboBox.SelectedIndex = j);
                }

                for (int i = 0; i < grantToEdit.PriorityTrands.Count; i++)
                {
                    AutoCompleteComboBox priorityTrendComboBox = null;
                    Dispatcher.Invoke(() => priorityTrendComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5),
                        ItemsSource = new List<PriorityTrend>(PriorityTrendList),
                        Width = 270
                    });
                    for (int j = 0; j < PriorityTrendList.Count; j++)
                        if (PriorityTrendList[j].Title == grantToEdit.PriorityTrands[i].Title)
                            Dispatcher.Invoke(() => priorityTrendComboBox.SelectedIndex = j);


                    Dispatcher.Invoke(() => priorityTrendsVerticalListView.Items.Add(priorityTrendComboBox));
                }

                for (int i = 0; i < grantToEdit.ScienceType.Count; i++)
                {
                    AutoCompleteComboBox scienceTypeComboBox = null;
                    Dispatcher.Invoke(() => scienceTypeComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5),
                        ItemsSource = new List<ScienceType>(ScienceTypeList),
                        Width = 270
                    });

                    for (int j = 0; j < ScienceTypeList.Count; j++)
                        if (ScienceTypeList[j].Title == grantToEdit.ScienceType[i].Title)
                            Dispatcher.Invoke(() => scienceTypeComboBox.SelectedIndex = j);

                    Dispatcher.Invoke(() => scienceTypeVerticalListView.Items.Add(scienceTypeComboBox));
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

            Dispatcher.Invoke(() => Title = oldTitle);
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
                Margin = new Thickness(5),
                ItemsSource = new List<Customer>(CustomersList),
                Width = 270
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
                        // запомнить, где текущий индекс сейчас курсора в текстбоксе
                        int index = sumTextBox.CaretIndex;
                        sumTextBox.Text = Convert.ToDouble(sumTextBox.Text) < 0.0000001 ? "" : String.Format("{0:#,0.#####}", Convert.ToDouble(sumTextBox.Text));
                        sumTextBox.SelectionStart = index;
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

        private void ExecutorAddButton_Click(object sender, RoutedEventArgs e)
        {

            AutoCompleteComboBox executorComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5),
                ItemsSource = new List<Person>(PersonsList),
                Width = 270
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
                //MessageBox.Show("Выделите нужный для удаления элемент", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PriorityTrendAddButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteComboBox priorityTrendComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5),
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
                Margin = new Thickness(5),
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
                    // запомнить, где текущий индекс сейчас курсора в текстбоксе
                    int index = priceTextBox.CaretIndex;
                    priceTextBox.Text = Convert.ToDouble(priceTextBox.Text) < 0.0000001 ? "" : String.Format("{0:#,0.#####}", Convert.ToDouble(priceTextBox.Text));
                    priceTextBox.SelectionStart = index;
                }
            }
            else
                priceNoNDSTextBox.Text = "";
        }

        private void LeadNIOKRMultiSelectComboBox_SelectedItemsChanged(object sender, Sdl.MultiSelectComboBox.EventArgs.SelectedItemsChangedEventArgs e)
        {
            if (SelectedLeadNIOKR.Count == 0) return;

            Person person = SelectedLeadNIOKR[0];
            CRUDDataBase.ConnectToDataBase();

            HashSet<String> set = new HashSet<String>();
            if (grantToEdit != null)
            {
                if (grantToEdit.FirstNode.Title != null)
                {
                    FirstNodeList.Add(grantToEdit.FirstNode);
                    set.Add(grantToEdit.FirstNode.Title);
                }
            }
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllFirstNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    FirstNodeList.Add(u);
                    set.Add(u.Title);
                }
                if (u.IsMainWorkPlace)
                {
                    for (int i = 0; i < FirstNodeList.Count; i++)
                    {
                        if (u.Id == FirstNodeList[i].Id)
                            FirstNodeComboBox.SelectedIndex = i;
                    }
                }
            }
            set.Clear();

            if (grantToEdit != null)
            {
                if (grantToEdit.SecondNode.Title != null)
                {
                    SecondNodeList.Add(grantToEdit.SecondNode);
                    set.Add(grantToEdit.SecondNode.Title);
                }
            }
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllSecondNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    SecondNodeList.Add(u);
                    set.Add(u.Title);
                }
                if (u.IsMainWorkPlace)
                {
                    for (int i = 0; i < SecondNodeList.Count; i++)
                    {
                        if (u.Id == SecondNodeList[i].Id)
                            SecondNodeComboBox.SelectedIndex = i;
                    }
                }
            }
            set.Clear();
            if (grantToEdit != null)
            {
                if (grantToEdit.ThirdNode.Title != null)
                {
                    ThirdNodeList.Add(grantToEdit.ThirdNode);
                    set.Add(grantToEdit.ThirdNode.Title);
                }
            }
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllThirdNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    ThirdNodeList.Add(u);
                    set.Add(u.Title);
                }
                if (u.IsMainWorkPlace)
                {
                    for (int i = 0; i < ThirdNodeList.Count; i++)
                    {
                        if (u.Id == ThirdNodeList[i].Id)
                            ThirdNodeComboBox.SelectedIndex = i;
                    }
                }
            }
            set.Clear();
            if (grantToEdit != null)
            {
                if (grantToEdit.FourthNode.Title != null)
                {
                    FourthNodeList.Add(grantToEdit.FourthNode);
                    set.Add(grantToEdit.FourthNode.Title);
                }
            }
            foreach (UniversityStructureNode u in CRUDDataBase.GetAllFourthNodesByPerson(person))
            {
                if (!set.Contains(u.Title))
                {
                    FourthNodeList.Add(u);
                    set.Add(u.Title);
                }
                if (u.IsMainWorkPlace)
                {
                    for (int i = 0; i < FourthNodeList.Count; i++)
                    {
                        if (u.Id == FourthNodeList[i].Id)
                            FourthComboBox.SelectedIndex = i;
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
