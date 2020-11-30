using DotNetKit.Windows.Controls;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createGrantWindow.xaml
    /// </summary>
    public partial class createGrantWindow : Window
    {
        // DataTable для грантов на главной таблице
        private DataTable grantsDataTable;


        //Списки данных из БД
        public ObservableCollection<string> NIOKRList { get; set; }
        public List<Person> personsList { get; set; }
        public List<string> selectedItems { get; set; }
        public List<string> selectedValues { get; set; }

        public List<Depositor> depositsList { get; set; }
        public List<ScienceType> scienceTypeList { get; set; }
        public List<Kafedra> kafedrasList { get; set; }
        public List<Unit> unitsList { get; set; }
        public List<Institution> instituionsList { get; set; }
        public List<ResearchType> researchTypesList { get; set; }
        public List<PriorityTrend> priorityTrendList { get; set; }
        //Списки данных из формы
        public List<ComboBox> enteredExecutorsList { get; set; }
        public List<Object[]> enteredDepositsList { get; set; }
        public List<ComboBox> enteredExecutorsContractList { get; set; }
        public List<ComboBox> enteredScienceTypesList { get; set; }

        public string NirChecker;

        // Combobox для заказчика
        AutoCompleteComboBox customerAutoCompleteComboBox;
        // Combobox для руководителя
        AutoCompleteComboBox LeadNIOKRAutoCompleteComboBox;
        // Combobox для кафедры
        AutoCompleteComboBox kafedraAutoCompleteComboBox;
        // Combobox для подразделения
        AutoCompleteComboBox unitAutoCompleteComboBox;
        // Combobox для подразделения
        AutoCompleteComboBox institutionAutoCompleteComboBox;
        // Combobox для типа исследования
        AutoCompleteComboBox researchTypeAutoCompleteComboBox;


        public createGrantWindow(DataTable grantsDataTable)
        {
            NIOKRList = new ObservableCollection<string>();
            NIOKRList.Add("19");
            NIOKRList.Add("20");

            InitializeComponent();

            this.grantsDataTable = grantsDataTable;


            // Подключение к базе данных
            CRUDDataBase.ConnectByDataBase();

            personsList = CRUDDataBase.GetPersons();
            depositsList = CRUDDataBase.GetDeposits();
            kafedrasList = CRUDDataBase.GetKafedras();
            unitsList = CRUDDataBase.GetUnits();
            instituionsList = CRUDDataBase.GetInstitutions();
            researchTypesList = CRUDDataBase.GetResearchTypes();
            scienceTypeList = CRUDDataBase.GetScienceTypes();
            priorityTrendList = CRUDDataBase.GetPriorityTrends();

            enteredDepositsList = new List<object[]>();
            enteredExecutorsContractList = new List<ComboBox>();
            enteredScienceTypesList = new List<ComboBox>();
            enteredExecutorsList = new List<ComboBox>();

            addCustomerAutoCompleteComboBox();
            addLeadNIOKRAutoCompleteComboBox();
            addInstitutionAutoCompleteComboBox();
            addUnitAutoCompleteComboBox();
            addKafedraAutoCompleteComboBox();
            addResearchTypeAutoCompleteComboBox();

            // Закрытие подключения к базе данных
            CRUDDataBase.CloseConnect();
        }
        /// <summary>
        /// Добавление комбо бокса к заказчику
        /// </summary>
        private void addCustomerAutoCompleteComboBox()
        {

            customerAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Person>(personsList),
                MinWidth = 300
            };
            customerGrid.Children.Add(customerAutoCompleteComboBox);
            Grid.SetRow(customerAutoCompleteComboBox, 1);
        }
        /// <summary>
        /// Добавление комбо бокса к руководителю НИОКР
        /// </summary>
        private void addLeadNIOKRAutoCompleteComboBox()
        {
            LeadNIOKRAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Person>(personsList),
                MinWidth = 300,

            };
            leadNIOKRGrid.Children.Add(LeadNIOKRAutoCompleteComboBox);
            Grid.SetRow(LeadNIOKRAutoCompleteComboBox, 1);
        }
        /// <summary>
        /// Добавление комбо бокса к кафедре
        /// </summary>
        private void addKafedraAutoCompleteComboBox()
        {
            kafedraAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Kafedra>(kafedrasList),
                MinWidth = 300
            };
            kafedraGrid.Children.Add(kafedraAutoCompleteComboBox);
            Grid.SetRow(kafedraAutoCompleteComboBox, 1);
        }
        /// <summary>
        /// Добавление комбо бокса к подразделению
        /// </summary>
        private void addUnitAutoCompleteComboBox()
        {
            unitAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Unit>(unitsList),
                MinWidth = 300
            };
            unitGrid.Children.Add(unitAutoCompleteComboBox);
            Grid.SetRow(unitAutoCompleteComboBox, 1);
        }
        /// <summary>
        /// Добавление комбо бокса к учреждению
        /// </summary>
        private void addInstitutionAutoCompleteComboBox()
        {
            institutionAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Institution>(instituionsList),
                MinWidth = 300
            };
            institutionGrid.Children.Add(institutionAutoCompleteComboBox);
            Grid.SetRow(institutionAutoCompleteComboBox, 1);
        }

        /// <summary>
        /// Добавление комбо бокса к типу
        /// </summary>
        private void addResearchTypeAutoCompleteComboBox()
        {
            researchTypeAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<ResearchType>(researchTypesList),
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
        private void depositsAddButton_Click_1(object sender, RoutedEventArgs e)
        {

            StackPanel horizontalStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            ComboBox depositorComboBox = new ComboBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                ItemsSource = depositsList,
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
        private void depositsDeleteButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }

        private void executorAddButton_Click(object sender, RoutedEventArgs e)
        {

            AutoCompleteComboBox executorComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Person>(personsList),
                MinWidth = 300
            };

            executorsVerticalListView.Items.Add(executorComboBox);
        }

        private void executorDeleteButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }

        private void priorityTrendAddButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteComboBox priorityTrendComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<PriorityTrend>(priorityTrendList),
                MinWidth = 300
            };


            priorityTrendsVerticalListView.Items.Add(priorityTrendComboBox);
        }

        private void priorityTrendDeleteButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }

        private void executorOnContractAddButton_Click(object sender, RoutedEventArgs e)
        {

            AutoCompleteComboBox executorOnContractComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Person>(personsList),
                MinWidth = 300
            };


            executorOnContractVerticalListView.Items.Add(executorOnContractComboBox);
        }

        private void executorOnContractDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = executorOnContractVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    executorOnContractVerticalListView.Items.Remove(executorOnContractVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }

        private void scienceTypeAddButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteComboBox scienceTypeComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<ScienceType>(scienceTypeList),
                MinWidth = 300
            };
            scienceTypeVerticalListView.Items.Add(scienceTypeComboBox);
        }

        private void scienceTypeDeleteButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }
        /// <summary>
        /// Смена вкладок по нажатию кнопки и изменение цвета фона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grantParametersButtonClick(object sender, RoutedEventArgs e)
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
        private void createGrantButtonClick(object sender, RoutedEventArgs e)
        {
            Grant newGrant = new Grant();

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

            if(grantNumberTextBox.Text.ToString() != "")
            {
                newGrant.grantNumber = grantNumberTextBox.Text;
            }
            else
            {
                MessageBox.Show("Необходимо указать номер договора");
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

            if ((Person)customerAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.Customer = new Person()
                {
                    Id = ((Person)customerAutoCompleteComboBox.SelectedItem).Id,
                    FIO = ((Person)customerAutoCompleteComboBox.SelectedItem).FIO
                };
            }
            else
            {
                MessageBox.Show("Необходимо указать заказчика");
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
                MessageBox.Show("Необходимо указать руководителя проекта");
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

            if (kafedraAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.Kafedra = new Kafedra()
                {
                    Id = ((Kafedra)kafedraAutoCompleteComboBox.SelectedItem).Id,
                    Title = kafedraAutoCompleteComboBox.SelectedItem.ToString()
                };
            }
            else
            {
                MessageBox.Show("Необходимо указать кафедру");
                isAllOkey = false;
            }


            if (unitAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.Unit = new Unit()
                {
                    Id = ((Unit)unitAutoCompleteComboBox.SelectedItem).Id,
                    Title = unitAutoCompleteComboBox.SelectedItem.ToString()
                };
            }
            else
            {
                MessageBox.Show("Необходимо указать подразделение");
                isAllOkey = false;
            }

            if (institutionAutoCompleteComboBox.SelectedItem != null)
            {
                newGrant.Institution = new Institution()
                {
                    Id = ((Institution)institutionAutoCompleteComboBox.SelectedItem).Id,
                    Title = institutionAutoCompleteComboBox.SelectedItem.ToString()
                };
            }
            else
            {
                MessageBox.Show("Необходимо указать учреждение");
                isAllOkey = false;
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

            if (executorOnContractVerticalListView.Items != null)
            {
                foreach (AutoCompleteComboBox cmb in executorOnContractVerticalListView.Items.OfType<AutoCompleteComboBox>())
                {
                    if (cmb.SelectedItem != null)
                    {
                        newGrant.ExecutorContract.Add(new Person() {
                            Id = ((Person)cmb.SelectedItem).Id,
                            FIO = cmb.SelectedItem.ToString()
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

            if (NOCTextBox.Text != "")
            {
                newGrant.NOC = NOCTextBox.Text;
            }
            else
            {
                newGrant.NOC = "";
            }


            // Если данные введены корректно
            if (isAllOkey)
            {
                // Подключаюсь к БД
                CRUDDataBase.ConnectByDataBase();

                CRUDDataBase.InsertNewGrantToDB(newGrant);

                // Закрываем соединение с БД
                CRUDDataBase.CloseConnect();

                WorkerWithTablesOnMainForm.AddRowToGrantTable(grantsDataTable, newGrant);

                MessageBox.Show("Договор успешно создан");
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            NirChecker = pressed.Content.ToString();
        }

    }
}
