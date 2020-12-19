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

        public string selectedNameNIOKR;
        public string SelectedNameNIOKR
        {
            get { return selectedNameNIOKR; }
            set { selectedNameNIOKR = value; Console.WriteLine((string)value); }
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

        // Combobox для заказчика
        AutoCompleteComboBox customerAutoCompleteComboBox;
        // Combobox для руководителя
        AutoCompleteComboBox LeadNIOKRAutoCompleteComboBox;
        // Combobox для типа исследования
        AutoCompleteComboBox researchTypeAutoCompleteComboBox;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public CreateGrantWindow(DataTable grantsDataTable, Grant grantToEdit=null)
        {
            NIOKRList = new ObservableCollection<string>
            {
                "19",
                "20"
            };

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

            AddCustomerAutoCompleteComboBox();
            AddLeadNIOKRAutoCompleteComboBox();
            AddResearchTypeAutoCompleteComboBox();


            // Закрытие подключения к базе данных
            CRUDDataBase.CloseConnection();

            // Если открыта форма редактирования, то вставим в нее данные
            if (grantToEdit != null)
            {
                Title = "Редактирование договора";
                createGrantButton.Content = "Редактировать";
                OKVEDTextBox.Text = grantToEdit.OKVED;
                grantNumberTextBox.Text = grantToEdit.grantNumber;
                switch (grantToEdit.NameNIOKR)
                {
                    case "19":
                        NIOKRComboBox.SelectedIndex = 0;
                        break;
                    case "20":
                        NIOKRComboBox.SelectedIndex = 1;
                        break;
                }
                for (int i = 0; i < CustomersList.Count; i++)
                    if (CustomersList[i].Title == grantToEdit.Customer.Title)
                        customerAutoCompleteComboBox.SelectedIndex = i;

                startDateDatePicker.SelectedDate = grantToEdit.StartDate;
                endDateDatePicker.SelectedDate = grantToEdit.EndDate;
                priceTextBox.Text = grantToEdit.Price.ToString();

                for(int i = 0;i < grantToEdit.Depositor.Count; i++)
                {
                    StackPanel horizontalStackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                    };

                    ComboBox depositorComboBox = new ComboBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        ItemsSource = DepositsList,
                        Width = 240,
                    };
                    for (int j = 0; j < DepositsList.Count; j++)
                        if (DepositsList[j].Title == grantToEdit.Depositor[i].Title)
                            depositorComboBox.SelectedIndex = j;


                    TextBox sumTextBox = new TextBox()
                    {
                        Margin = new Thickness(5, 0, 5, 10),
                        MinWidth = 90,
                        Text = grantToEdit.DepositorSum[i].ToString()
                    };

                    horizontalStackPanel.Children.Add(depositorComboBox);
                    horizontalStackPanel.Children.Add(sumTextBox);


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
                //АРТЁМ СЮДА
                //АРТЁМ СЮДА
                //АРТЁМ СЮДА
                //АРТЁМ СЮДА
                //UniversityStructure.SelectedInstitution = grantToEdit.Institution;
                //АРТЁМ СЮДА
                //АРТЁМ СЮДА
                //АРТЁМ СЮДА

                GRNTITextBox.Text = grantToEdit.GRNTI;

                for (int j = 0; j < ResearchTypesList.Count; j++)
                    if (ResearchTypesList[j].Title == grantToEdit.ResearchType[0].Title)
                        researchTypeAutoCompleteComboBox.SelectedIndex = j;

                for (int i = 0;i<grantToEdit.PriorityTrands.Count; i++)
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
        /// <summary>
        /// Добавление комбо бокса к заказчику
        /// </summary>
        private void AddCustomerAutoCompleteComboBox()
        {

            customerAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Customer>(CustomersList),
                MinWidth = 300
            };
            customerGrid.Children.Add(customerAutoCompleteComboBox);
            Grid.SetRow(customerAutoCompleteComboBox, 1);
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
            leadNIOKRGrid.Children.Add(LeadNIOKRAutoCompleteComboBox);
            Grid.SetRow(LeadNIOKRAutoCompleteComboBox, 1);
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
        }

        /// <summary>
        /// Добавление строки в средства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DepositsAddButton_Click_1(object sender, RoutedEventArgs e)
        {

            StackPanel horizontalStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            ComboBox depositorComboBox = new ComboBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                ItemsSource = DepositsList,
                Width = 240
            };

            TextBox sumTextBox = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                MinWidth = 90
            };

            horizontalStackPanel.Children.Add(depositorComboBox);
            horizontalStackPanel.Children.Add(sumTextBox);


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
                MinWidth = 300
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
                MinWidth = 300
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
                MinWidth = 300
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
            foreach (Button button in grantParametersButtonStackPanel.Children.OfType<Button>()) {
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
                if (CRUDDataBase.IsGrantNumberAvailable(grantNumberTextBox.Text))
                    newGrant.grantNumber = grantNumberTextBox.Text;
                else
                {
                    incorrectDataString += "Догово с таким номером уже существует. Пожалуйста, укажите уникальный номер договора.\n\n";
                    isAllOkey = false;
                }
            }
            else
            {
                //MessageBox.Show("Необходимо указать номер договора");
                incorrectDataString += "Необходимо указать номер договора\n";
                isAllOkey = false;
            }

            if (NIOKRComboBox.SelectedItem != null)
            {
                newGrant.NameNIOKR = NIOKRComboBox.SelectedItem.ToString();
            }
            else
            {
                newGrant.NameNIOKR = "";
            }

            if ((Customer)customerAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.Customer = new Customer()
                {
                    Id = ((Customer)customerAutoCompleteComboBox.SelectedItem).Id,
                    Title = ((Customer)customerAutoCompleteComboBox.SelectedItem).Title
                };
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

            if (depositsVerticalListView.Items != null)
            {
                ComboBox cmb;
                TextBox partSum;

                foreach (StackPanel sp in depositsVerticalListView.Items.OfType<StackPanel>())
                {
                    cmb = (ComboBox)sp.Children[0];
                    partSum = (TextBox)sp.Children[1];

                    if (cmb.SelectedItem != null && partSum.Text.ToString() != "")
                    {
                        newGrant.Depositor.Add(new Depositor()
                        {
                            Id = ((Depositor)cmb.SelectedItem).Id,
                            Title = cmb.SelectedItem.ToString()
                        });
                        newGrant.DepositorSum.Add(Parser.ConvertToRightFloat(partSum.Text));
                    }
                }
            }

            if (LeadNIOKRAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.LeadNIOKR = new Person() {
                    Id = ((Person)LeadNIOKRAutoCompleteComboBox.SelectedItem).Id,
                    FIO = ((Person)LeadNIOKRAutoCompleteComboBox.SelectedItem).FIO
                };
            }
            else
            {
                incorrectDataString += "Необходимо указать руководителя НИОКР\n";
                //MessageBox.Show("Необходимо указать руководителя проекта");
                isAllOkey = false;
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

            // Если данные введены корректно
            if (isAllOkey)
            {
                // Подключаюсь к БД
                CRUDDataBase.ConnectToDataBase();

                CRUDDataBase.InsertNewGrantToDB(newGrant);

                // Закрываем соединение с БД
                CRUDDataBase.CloseConnection();

                MessageBox.Show("Договор успешно создан", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show(incorrectDataString, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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

    }
}
