using DotNetKit.Windows.Controls;
using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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

        //Списки данных из БД

        public List<Person> _personsList;
        public List<Person> PersonsList {
            get{
                return _personsList;
            }
            set{
                _personsList = value;
                OnPropertyChanged("PersonsList");
            }
        }

        public List<Customer> CustomersList { get; set; }

        public List<Depositor> DepositsList { get; set; }
        public List<ScienceType> ScienceTypeList { get; set; }
        public List<ResearchType> ResearchTypesList { get; set; }
        public List<PriorityTrend> PriorityTrendList { get; set; }
        //Списки данных из формы
        public List<ComboBox> EnteredExecutorsList { get; set; }
        public List<Object[]> EnteredDepositsList { get; set; }
        public List<ComboBox> EnteredScienceTypesList { get; set; }

        MainWindow mainWindow;
        Grant grantToEdit;
        public string NirChecker;
        public string NOCChecker;


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
            mainWindow = Owner;

            this.grantToEdit = grantToEdit;
            // Подключение к базе данных
            CRUDDataBase.ConnectToDataBase();

            PersonsList = CRUDDataBase.GetPersons();

            //PersonsList = StaticProperties.PersonsList;
            
            CustomersList = CRUDDataBase.GetCustomers();
            DepositsList = CRUDDataBase.GetDeposits();
            ResearchTypesList = CRUDDataBase.GetResearchTypes();
            ScienceTypeList = CRUDDataBase.GetScienceTypes();
            PriorityTrendList = CRUDDataBase.GetPriorityTrends();

            EnteredDepositsList = new List<object[]>();
            EnteredScienceTypesList = new List<ComboBox>();
            EnteredExecutorsList = new List<ComboBox>();

            LeadNIOKRAutoCompleteComboBox.ItemsSource = new List<Person>(PersonsList);

            researchTypeComboBox.ItemsSource = new List<ResearchType>(ResearchTypesList);

            FirstNodeList = new ObservableCollection<UniversityStructureNode>();
            SecondNodeList = new ObservableCollection<UniversityStructureNode>();
            ThirdNodeList = new ObservableCollection<UniversityStructureNode>();
            FourthNodeList = new ObservableCollection<UniversityStructureNode>();


            priceTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            priceNoNDSTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

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
                createGrantButton.Content = "Сохранить";
                OKVEDTextBox.Text = grantToEdit.OKVED;
                grantNumberTextBox.Text = grantToEdit.grantNumber;
                NIOKRTextBox.Text = grantToEdit.NameNIOKR;

                for (int i = 0; i < grantToEdit.Customer.Count; i++)
                {
                    AutoCompleteComboBox customerComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5),
                        ItemsSource = new List<Customer>(CustomersList),
                        Width = 270
                    };

                    for (int j = 0; j < CustomersList.Count; j++)
                        if (CustomersList[j].Title == grantToEdit.Customer[i].Title)
                            customerComboBox.SelectedIndex = j;

                    customersVerticalListView.Items.Add(customerComboBox);
                }


                startDateDatePicker.SelectedDate = grantToEdit.StartDate;
                endDateDatePicker.SelectedDate = grantToEdit.EndDate;
                priceTextBox.Text = String.Format("{0:#,0.##}", grantToEdit.Price);
                priceNoNDSTextBox.Text = String.Format("{0:#,0.##}", grantToEdit.PriceNoNDS);
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
                        Text = String.Format("{0:#,0.##}", grantToEdit.DepositorSum[i]),
                        //IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked
                    };


                    sumTextBoxNoNDS = new TextBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        MinWidth = 110,
                        Text = String.Format("{0:#,0.##}", grantToEdit.DepositorSumNoNDS[i])
                    };
                    sumTextBoxNoNDS.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
                    sumTextBoxNoNDS.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;

                    sumTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
                    sumTextBox.TextChanged += sumTextBoxTextChangedEventHandler;
                    sumTextBox.PreviewKeyDown += priceNoNDSTextBox_PreviewKeyDown;


                    DateTime selectedDate;
                    DateTime.TryParse(grantToEdit.ReceiptDate[i], out selectedDate);
                    DatePicker dateComboBox = new DatePicker()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        Width = 110,
                        SelectedDate = selectedDate
                    };

                    horizontalStackPanel.Children.Add(depositorComboBox);
                    horizontalStackPanel.Children.Add(sumTextBox);
                    horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 0, 0), /*IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked*/ });
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
                        Margin = new Thickness(5),
                        ItemsSource = new List<Person>(PersonsList),
                        Width = 270
                    };
                    for (int j = 0; j < PersonsList.Count; j++)
                        if (PersonsList[j].FIO == grantToEdit.Executor[i].FIO)
                            executorComboBox.SelectedIndex = j;

                    executorsVerticalListView.Items.Add(executorComboBox);
                }

                //// Привязка для структуры университета
                //UniversityStructure.SelectedInstitution = UniversityStructure.FindInstitution(grantToEdit.Institution.Id);
                //if (UniversityStructure.SelectedInstitution != null)
                //{
                //    UniversityStructure.SelectedUnit = UniversityStructure.FindUnit(UniversityStructure.SelectedInstitution, grantToEdit.Unit.Id);
                //    if (UniversityStructure.SelectedUnit != null)
                //    {
                //        UniversityStructure.SelectedKafedra = UniversityStructure.FindKafedra(UniversityStructure.SelectedUnit, grantToEdit.Kafedra.Id);
                //        if (UniversityStructure.SelectedKafedra != null)
                //        {
                //            UniversityStructure.SelectedLaboratory = UniversityStructure.FindLaboratoryInKafedra(UniversityStructure.SelectedKafedra, grantToEdit.Laboratory.Id);
                //        }
                //        if (UniversityStructure.SelectedLaboratory == null)
                //        {
                //            UniversityStructure.SelectedLaboratory = UniversityStructure.FindLaboratoryInUnit(UniversityStructure.SelectedUnit, grantToEdit.Laboratory.Id);
                //        }
                //    }
                //}

                FirstNodeComboBox.SelectedIndex = -1;
                if (grantToEdit.FirstNode.Title != null)
                {
                    for (int i = 0; i < FirstNodeList.Count; i++)
                    {
                        if (grantToEdit.FirstNode.Id == FirstNodeList[i].Id)
                        {
                            FirstNodeComboBox.SelectedIndex = i;
                        }
                    }
                }

                SecondNodeComboBox.SelectedIndex = -1;
                if (grantToEdit.SecondNode.Title != null)
                {
                    for (int i = 0; i < SecondNodeList.Count; i++)
                    {
                        if (grantToEdit.SecondNode.Id == SecondNodeList[i].Id)
                        {
                            SecondNodeComboBox.SelectedIndex = i;
                        }
                    }
                }

                ThirdNodeComboBox.SelectedIndex = -1;
                if (grantToEdit.ThirdNode.Title != null)
                {
                    for (int i = 0; i < ThirdNodeList.Count; i++)
                    {
                        if (grantToEdit.ThirdNode.Id == ThirdNodeList[i].Id)
                        {
                            ThirdNodeComboBox.SelectedIndex = i;
                        }
                    }
                }

                FourthComboBox.SelectedIndex = -1;
                if (grantToEdit.FourthNode.Title != null)
                {
                    for (int i = 0; i < FourthNodeList.Count; i++)
                    {
                        if (grantToEdit.FourthNode.Id == FourthNodeList[i].Id)
                        {
                            FourthComboBox.SelectedIndex = i;
                        }
                    }
                }

                GRNTITextBox.Text = grantToEdit.GRNTI;

                if (grantToEdit.ResearchType.Count > 0)
                {
                    for (int j = 0; j < ResearchTypesList.Count; j++)
                        if (ResearchTypesList[j].Title == grantToEdit.ResearchType[0].Title)
                            researchTypeComboBox.SelectedIndex = j;
                }

                for (int i = 0; i < grantToEdit.PriorityTrands.Count; i++)
                {
                    AutoCompleteComboBox priorityTrendComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(5),
                        ItemsSource = new List<PriorityTrend>(PriorityTrendList),
                        Width = 270
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
                        Margin = new Thickness(5),
                        ItemsSource = new List<ScienceType>(ScienceTypeList),
                        Width = 270
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

            DatePicker datePicker = new DatePicker()
            {
                Margin = new Thickness(5, 0, 5, 10),
                Width = 110
            };

            horizontalStackPanel.Children.Add(depositorComboBox);
            horizontalStackPanel.Children.Add(sumTextBox);
            horizontalStackPanel.Children.Add(new Label() { Content = "руб.", FontSize = 12, Margin = new Thickness(-7, 0, 0, 0), /*IsEnabled = !(bool)GrantWithoutNDSCheckBox.IsChecked*/ });
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
                newGrant.StartDate = (DateTime)startDateDatePicker.SelectedDate;
            }

            if (endDateDatePicker.SelectedDate != null)
            {
                newGrant.EndDate = (DateTime)endDateDatePicker.SelectedDate;
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

                    MessageBox.Show("Информация о договоре успешно изменена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
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
                ((MainWindow)Owner).GrantsUpdateButton_Click(sender, e);
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
                    ((MainWindow)Owner).GrantsUpdateButton_Click(sender, e);
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

        private void LeadNIOKRAutoCompleteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutoCompleteComboBox autoCompleteComboBox = (AutoCompleteComboBox)sender;
            Person person = (Person)autoCompleteComboBox.SelectedItem;
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
    }
}
