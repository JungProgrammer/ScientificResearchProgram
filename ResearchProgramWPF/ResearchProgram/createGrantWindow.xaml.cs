using DotNetKit.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createGrantWindow.xaml
    /// </summary>
    public partial class createGrantWindow : Window
    {
        //Списки данных из БД
        public List<string> NIOKRList { get; set; }
        public List<Person> personsList { get; set; }
        public List<string> selectedItems { get; set; }
        public List<string> selectedValues { get; set; }

        public List<string> depositsList { get; set; }
        public List<string> scienceTypeList { get; set; }
        public List<string> kafedrasList { get; set; }
        public List<string> unitsList { get; set; }
        public List<string> instituionsList { get; set; }
        public List<string> researchTypesList { get; set; }
        //Списки данных из формы
        public List<ComboBox> enteredExecutorsList { get; set; }
        public List<Object[]> enteredDepositsList { get; set; }
        public List<ComboBox> enteredExecutorsContractList { get; set; }
        public List<ComboBox> enteredScienceTypesList { get; set; }

        public string selectedCustomer { get; set; }
        AutoCompleteComboBox customerAutoCompleteComboBox;


        public createGrantWindow()
        {
            InitializeComponent();

            //Заполнение списков
            NIOKRList = new List<string>();
            NIOKRList.Add("19");
            NIOKRList.Add("20");

            CRUDDataBase.ConnectByDataBase();
            personsList = CRUDDataBase.GetPersons();
            depositsList = CRUDDataBase.GetDeposits();
            kafedrasList = CRUDDataBase.GetKafedras();
            unitsList = CRUDDataBase.GetUnits();
            instituionsList = CRUDDataBase.GetInstitutions();
            researchTypesList = CRUDDataBase.GetResearchTypes();
            scienceTypeList = CRUDDataBase.GetScienceTypes();

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


            DataContext = this;
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
            AutoCompleteComboBox LeadNIOKRAutoCompleteComboBox = new AutoCompleteComboBox()
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
            AutoCompleteComboBox kafedraAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<string>(kafedrasList),
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
            AutoCompleteComboBox unitAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<string>(unitsList),
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
            AutoCompleteComboBox institutionAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<string>(instituionsList),
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
            AutoCompleteComboBox researchTypeAutoCompleteComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<string>(researchTypesList),
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
                ItemsSource = new List<string>(scienceTypeList),
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
            foreach (Button button in grantParametersButtonStackPanel.Children.OfType<Button>()){
                button.Background = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
            }
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 189, 189, 189));
            
        }

        private void createGrantButtonClick(object sender, RoutedEventArgs e)
        {
            Grant newGrant = new Grant();

            newGrant.OKVED = OKVEDTextBox.Text;
            
            if (NIOKRComboBox.SelectedItem != null)
                newGrant.NameNIOKR = NIOKRComboBox.SelectedItem.ToString();

            newGrant.Customer = new Person()
            {
                Id = ((Person)customerAutoCompleteComboBox.SelectedItem).Id,
                FIO = ((Person)customerAutoCompleteComboBox.SelectedItem).FIO
            };



        }
    }
}
