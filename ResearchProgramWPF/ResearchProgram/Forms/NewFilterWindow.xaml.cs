using ResearchProgram.Classes;
using Sdl.MultiSelectComboBox.Themes.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для NewFilterWindow.xaml
    /// </summary>
    public partial class NewFilterWindow : Window, INotifyPropertyChanged
    {

        private const int SIMPLE_SEARCH_HEIGHT = 150;
        private const int EXTENDED_SEARCH_HEIGHT = 700;
        private const int SIMPLE_SEARCH_WIDTH = 600;
        private const int EXTENDED_SEARCH_WIDTH = 1100;

        private readonly double ScreenWidth = SystemParameters.PrimaryScreenWidth;
        private readonly double ScreenHeight = SystemParameters.PrimaryScreenHeight;
        private double WindowWidth;
        private double WindowHeight;

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People { get { return _people; } set { _people = value; OnPropertyChanged(nameof(People)); } }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers { get { return _customers; } set { _customers = value; OnPropertyChanged(nameof(Customers)); } }

        private ObservableCollection<UniversityStructureNode> _firstNodeList;
        private ObservableCollection<UniversityStructureNode> _secondNodeList;
        private ObservableCollection<UniversityStructureNode> _thirdNodeList;
        private ObservableCollection<UniversityStructureNode> _fourthNodeList;
        public ObservableCollection<UniversityStructureNode> FirstNodeList { get { return _firstNodeList; } set { _firstNodeList = value; OnPropertyChanged(nameof(FirstNodeList)); } }
        public ObservableCollection<UniversityStructureNode> SecondNodeList { get { return _secondNodeList; } set { _secondNodeList = value; OnPropertyChanged(nameof(SecondNodeList)); } }
        public ObservableCollection<UniversityStructureNode> ThirdNodeList { get { return _thirdNodeList; } set { _thirdNodeList = value; OnPropertyChanged(nameof(ThirdNodeList)); } }
        public ObservableCollection<UniversityStructureNode> FourthNodeList { get { return _fourthNodeList; } set { _fourthNodeList = value; OnPropertyChanged(nameof(FourthNodeList)); } }


        private ObservableCollection<ResearchType> _researchTypes;
        private ObservableCollection<ScienceType> _scienceTypes;
        private ObservableCollection<PriorityTrend> _priorityTrends;
        public ObservableCollection<ResearchType> ResearchTypes { get { return _researchTypes; } set { _researchTypes = value; OnPropertyChanged(nameof(ResearchTypes)); } }
        public ObservableCollection<ScienceType> ScienceTypes { get { return _scienceTypes; } set { _scienceTypes = value; OnPropertyChanged(nameof(ScienceTypes)); } }
        public ObservableCollection<PriorityTrend> PriorityTrends { get { return _priorityTrends; } set { _priorityTrends = value; OnPropertyChanged(nameof(PriorityTrends)); } }

        private ObservableCollection<OKVED> _okved;
        public ObservableCollection<OKVED> Okved { get { return _okved; } set { _okved = value; OnPropertyChanged(nameof(Okved)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        // Таблица, которая отвечает за гранты
        DataTable GrantsDataTable { get; set; }

        private ObservableCollection<OKVED> _selectedOkved;
        public ObservableCollection<OKVED> SelectedOkved { get { return _selectedOkved; } set { _selectedOkved = value; OnPropertyChanged(nameof(SelectedOkved)); } }

        private ObservableCollection<Person> _selectedLeadNIOKR;
        public ObservableCollection<Person> SelectedLeadNIOKR { get { return _selectedLeadNIOKR; } set { _selectedLeadNIOKR = value; OnPropertyChanged(nameof(SelectedLeadNIOKR)); } }

        private ObservableCollection<Customer> _selectedCustomer;
        public ObservableCollection<Customer> SelectedCustomer { get { return _selectedCustomer; } set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); } }

        private ObservableCollection<Person> _selectedExecutor;
        public ObservableCollection<Person> SelectedExecutor { get { return _selectedExecutor; } set { _selectedExecutor = value; OnPropertyChanged(nameof(SelectedExecutor)); } }

        private ObservableCollection<UniversityStructureNode> _selectedFirstNode;
        private ObservableCollection<UniversityStructureNode> _selectedSecondNode;
        private ObservableCollection<UniversityStructureNode> _selectedThirdNode;
        private ObservableCollection<UniversityStructureNode> _selectedFourthNode;
        public ObservableCollection<UniversityStructureNode> SelectedFirstNode { get { return _selectedFirstNode; } set { _selectedFirstNode = value; OnPropertyChanged(nameof(SelectedFirstNode)); } }
        public ObservableCollection<UniversityStructureNode> SelectedSecondNode { get { return _selectedSecondNode; } set { _selectedSecondNode = value; OnPropertyChanged(nameof(SelectedSecondNode)); } }
        public ObservableCollection<UniversityStructureNode> SelectedThirdNode { get { return _selectedThirdNode; } set { _selectedThirdNode = value; OnPropertyChanged(nameof(SelectedThirdNode)); } }
        public ObservableCollection<UniversityStructureNode> SelectedFourthNode { get { return _selectedFourthNode; } set { _selectedFourthNode = value; OnPropertyChanged(nameof(SelectedFourthNode)); } }

        private ObservableCollection<ResearchType> _selectedResearchType;
        private ObservableCollection<ScienceType> _selectedScienceType;
        private ObservableCollection<PriorityTrend> _selectedPriorityTrend;
        public ObservableCollection<ResearchType> SelectedResearchType { get { return _selectedResearchType; } set { _selectedResearchType = value; OnPropertyChanged(nameof(SelectedResearchType)); } }
        public ObservableCollection<ScienceType> SelectedScienceType { get { return _selectedScienceType; } set { _selectedScienceType = value; OnPropertyChanged(nameof(SelectedScienceType)); } }
        public ObservableCollection<PriorityTrend> SelectedPriorityTrend { get { return _selectedPriorityTrend; } set { _selectedPriorityTrend = value; OnPropertyChanged(nameof(SelectedPriorityTrend)); } }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public NewFilterWindow(DataTable grantsDataTable)
        {
            InitializeComponent();


            GrantsDataTable = grantsDataTable;

            CRUDDataBase.ConnectToDataBase();

            People = CRUDDataBase.GetPersons();
            Customers = CRUDDataBase.GetCustomers();

            FirstNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+$'"); // получение всех узлов с адресом первого уровня
            SecondNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом второго уровня
            ThirdNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом третьего уровня
            FourthNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом четвертого уровня

            ResearchTypes = CRUDDataBase.GetResearchTypes();
            ScienceTypes = CRUDDataBase.GetScienceTypes();
            PriorityTrends = CRUDDataBase.GetPriorityTrends();

            CRUDDataBase.CloseConnection();

            LeadNIOKRMultiselectComboBox.ItemsSource = new ObservableCollection<Person>(People);
            CustomerMultiselectComboBox.ItemsSource = Customers;
            ExecutorMultiselectComboBox.ItemsSource = new ObservableCollection<Person>(People);

            Okved = new ObservableCollection<OKVED>
            {
                new OKVED() {Title = "72.19"},
                new OKVED() {Title = "72.20"}
            };

            GrantPriceLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            GrantPriceRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

            FirstDepositLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            FirstDepositRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

            SecondDepositLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            SecondDepositRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

            ThirdDepositLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            ThirdDepositRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

            FourthDepositLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            FourthDepositRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

            FifthDepositLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            FifthDepositRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

            SixthDepositLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            SixthDepositRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;

            SeventhDepositLeftTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            SeventhDepositRightTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;


            SelectedOkved = new ObservableCollection<OKVED>();
            SelectedLeadNIOKR = new ObservableCollection<Person>();
            SelectedCustomer = new ObservableCollection<Customer>();
            SelectedExecutor = new ObservableCollection<Person>();
            SelectedFirstNode = new ObservableCollection<UniversityStructureNode>();
            SelectedSecondNode = new ObservableCollection<UniversityStructureNode>();
            SelectedThirdNode = new ObservableCollection<UniversityStructureNode>();
            SelectedFourthNode = new ObservableCollection<UniversityStructureNode>();
            SelectedResearchType = new ObservableCollection<ResearchType>();
            SelectedScienceType = new ObservableCollection<ScienceType>();
            SelectedPriorityTrend = new ObservableCollection<PriorityTrend>();


            DataContext = this;
        }

        private void SearchExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (SearchExpander.IsExpanded)
            {
                Height = EXTENDED_SEARCH_HEIGHT;
                Width = EXTENDED_SEARCH_WIDTH;
                SimpleSearchLabel.IsEnabled = false;
                SimpleSearchGrid.IsEnabled = false;
            }
            else
            {
                Height = SIMPLE_SEARCH_HEIGHT;
                Width = SIMPLE_SEARCH_WIDTH;
                SimpleSearchLabel.IsEnabled = true;
                SimpleSearchGrid.IsEnabled = true;
            }

            //Центрирование окна
            WindowWidth = Width;
            WindowHeight = Height;
            Left = (ScreenWidth / 2) - (WindowWidth / 2);
            Top = (ScreenHeight / 2) - (WindowHeight / 2);
        }

        private void NIR_Checked(object sender, RoutedEventArgs e)
        {
            UslugaCheckBox.IsChecked = false;
        }

        private void UslugaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NIRCheckBox.IsChecked = false;
        }

        private void NOCYesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NOCNoCheckBox.IsChecked = false;
        }

        private void NOCNoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NOCYesCheckBox.IsChecked = false;
        }

        private void DropSearchButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<TextBox> textBoxes = Utilities.FindVisualChildren<TextBox>(this);
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Text = "";
            }

            IEnumerable<ComboBox> comboBoxes = Utilities.FindVisualChildren<ComboBox>(this);
            foreach (ComboBox comboBox in comboBoxes)
            {
                comboBox.SelectedIndex = -1;
            }

            IEnumerable<DatePicker> datePickers = Utilities.FindVisualChildren<DatePicker>(this);
            foreach (DatePicker datePicker in datePickers)
            {
                datePicker.SelectedDate = null;
            }
            SelectedOkved = new ObservableCollection<OKVED>();
            SelectedLeadNIOKR = new ObservableCollection<Person>();
            SelectedCustomer = new ObservableCollection<Customer>();
            SelectedExecutor = new ObservableCollection<Person>();
            SelectedFirstNode = new ObservableCollection<UniversityStructureNode>();
            SelectedSecondNode = new ObservableCollection<UniversityStructureNode>();
            SelectedThirdNode = new ObservableCollection<UniversityStructureNode>();
            SelectedFourthNode = new ObservableCollection<UniversityStructureNode>();
            SelectedResearchType = new ObservableCollection<ResearchType>();
            SelectedScienceType = new ObservableCollection<ScienceType>();
            SelectedPriorityTrend = new ObservableCollection<PriorityTrend>();

        }
        /// <summary>
        /// Простой поиск по тексту таблицы без обращения к БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimpleSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuarry = SimpleSearchTextBox.Text;
            searchQuarry = searchQuarry.Trim();
            if (searchQuarry == "")
            {
                GrantsDataTable.DefaultView.RowFilter = null;
                return;
            }

            // составляется условие поиска и происходит фильтрация
            DataColumnCollection columns = GrantsDataTable.Columns;

            string filter = string.Empty;
            for (int i = 0; i < columns.Count; i++)
            {
                filter += "[" + columns[i].ColumnName + "] LIKE \'%" + searchQuarry + "%\'";
                if (i != columns.Count - 1)
                {
                    filter += " OR ";
                }
            }
            GrantsDataTable.DefaultView.RowFilter = filter;
        }

        private void ComplexSearchButton_Click(object sender, RoutedEventArgs e)
        {
            GrantsFilters.ResetFilters();
            IEnumerable<TextBox> textBoxes = Utilities.FindVisualChildren<TextBox>(this);
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Text = textBox.Text.Trim();
            }

            if (SelectedOkved.Count > 0)
            {
                GrantsFilters.OKVED = SelectedOkved;
            }

            if (GRNTITextBox.Text != "")
            {
                GrantsFilters.GRNTI = GRNTITextBox.Text;
            }

            if (grantNumberTextBox.Text != "")
            {
                GrantsFilters.grantNumber = grantNumberTextBox.Text;
            }

            if (SelectedLeadNIOKR.Count > 0)
            {
                GrantsFilters.LeadNIOKR = SelectedLeadNIOKR;
            }

            if (NIOKRTextBox.Text != "")
            {
                GrantsFilters.NameNIOKR = NIOKRTextBox.Text;
            }

            if ((bool)NIRCheckBox.IsChecked)
            {
                GrantsFilters.NIR = "НИР";
            }
            else
            {
                if ((bool)UslugaCheckBox.IsChecked)
                    GrantsFilters.NIR = "Услуга";
            }

            if ((bool)NOCYesCheckBox.IsChecked)
            {
                GrantsFilters.NOC = true;
            }
            else
            {
                if ((bool)NOCNoCheckBox.IsChecked)
                    GrantsFilters.NOC = false;
            }

            if (startDateDatePicker.SelectedDate != null)
            {
                GrantsFilters.StartDate = startDateDatePicker.SelectedDate;
            }

            if (endDateDatePicker.SelectedDate != null)
            {
                GrantsFilters.EndDate = endDateDatePicker.SelectedDate;
            }

            if (SelectedCustomer.Count > 0)
            {
                GrantsFilters.Customer = SelectedCustomer;
            }

            if (SelectedExecutor.Count > 0)
            {
                GrantsFilters.Executor = SelectedExecutor;
            }

            if ((bool)NoNDSCheckBox.IsChecked)
            {
                GrantsFilters.IsNoNDS = true;
            }

            FilterRange filterRange = new FilterRange();

            if (GrantPriceLeftComboBox.SelectedIndex != -1 && GrantPriceLeftComboBox.SelectedIndex != 0)
            {
                if (GrantPriceLeftTextBox.Text != "")
                {
                    filterRange.LeftValue = GrantPriceLeftTextBox.Text;
                    switch (GrantPriceLeftComboBox.SelectedIndex)
                    {
                        case 1:
                            filterRange.LeftSign = FilterRange.Signs.More;
                            break;
                        case 2:
                            filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                            break;
                    }
                }
            }

            if (GrantPriceRightComboBox.SelectedIndex != -1 && GrantPriceRightComboBox.SelectedIndex != 0)
            {
                if (GrantPriceRightTextBox.Text != "")
                {
                    filterRange.RightValue = GrantPriceRightTextBox.Text;
                    switch (GrantPriceRightComboBox.SelectedIndex)
                    {
                        case 1:
                            filterRange.RightSign = FilterRange.Signs.Less;
                            break;
                        case 2:
                            filterRange.RightSign = FilterRange.Signs.LessEqual;
                            break;
                    }

                }
            }
            if (filterRange.isActive())
            {
                GrantsFilters.Price = filterRange;
            }

            //Иностранные средства
            {
                filterRange = new FilterRange();
                if (FirstDepositLeftComboBox.SelectedIndex != -1 && FirstDepositLeftComboBox.SelectedIndex != 0)
                {
                    if (FirstDepositLeftTextBox.Text != "")
                    {
                        filterRange.LeftValue = FirstDepositLeftTextBox.Text;
                        switch (FirstDepositLeftComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (FirstDepositRightComboBox.SelectedIndex != -1 && FirstDepositRightComboBox.SelectedIndex != 0)
                {
                    if (FirstDepositRightTextBox.Text != "")
                    {
                        filterRange.RightValue = FirstDepositRightTextBox.Text;
                        switch (FirstDepositRightComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (FirstDepositLeftDateComboBox.SelectedIndex != -1 && FirstDepositLeftDateComboBox.SelectedIndex != 0)
                {
                    if (FirstDepositLeftDatePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = FirstDepositLeftDatePicker.SelectedDate;
                        switch (FirstDepositLeftDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftDateSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftDateSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (FirstDepositRightDateComboBox.SelectedIndex != -1 && FirstDepositRightDateComboBox.SelectedIndex != 0)
                {
                    if (FirstDepositRightDatePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = FirstDepositRightDatePicker.SelectedDate;
                        switch (FirstDepositRightDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightDateSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightDateSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (filterRange.isActive())
                {
                    GrantsFilters.FirstDepositor = filterRange;
                }
            }
            //Собственные средства
            {
                filterRange = new FilterRange();
                if (SecondDepositLeftComboBox.SelectedIndex != -1 && SecondDepositLeftComboBox.SelectedIndex != 0)
                {
                    if (SecondDepositLeftTextBox.Text != "")
                    {
                        filterRange.LeftValue = SecondDepositLeftTextBox.Text;
                        switch (SecondDepositLeftComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (SecondDepositRightComboBox.SelectedIndex != -1 && SecondDepositRightComboBox.SelectedIndex != 0)
                {
                    if (SecondDepositRightTextBox.Text != "")
                    {
                        filterRange.RightValue = SecondDepositRightTextBox.Text;
                        switch (SecondDepositRightComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (SecondDepositLeftDateComboBox.SelectedIndex != -1 && SecondDepositLeftDateComboBox.SelectedIndex != 0)
                {
                    if (SecondDepositLeftDatePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = SecondDepositLeftDatePicker.SelectedDate;
                        switch (SecondDepositLeftDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftDateSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftDateSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (SecondDepositRightDateComboBox.SelectedIndex != -1 && SecondDepositRightDateComboBox.SelectedIndex != 0)
                {
                    if (SecondDepositRightDatePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = SecondDepositRightDatePicker.SelectedDate;
                        switch (SecondDepositRightDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightDateSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightDateSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (filterRange.isActive())
                {
                    GrantsFilters.SecondDepositor = filterRange;
                }
            }
            //Средства бюджета субъекта Федерации
            {
                filterRange = new FilterRange();
                if (ThirdDepositLeftComboBox.SelectedIndex != -1 && ThirdDepositLeftComboBox.SelectedIndex != 0)
                {
                    if (ThirdDepositLeftTextBox.Text != "")
                    {
                        filterRange.LeftValue = ThirdDepositLeftTextBox.Text;
                        switch (ThirdDepositLeftComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (ThirdDepositRightComboBox.SelectedIndex != -1 && ThirdDepositRightComboBox.SelectedIndex != 0)
                {
                    if (ThirdDepositRightTextBox.Text != "")
                    {
                        filterRange.RightValue = ThirdDepositRightTextBox.Text;
                        switch (ThirdDepositRightComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (ThirdDepositLeftDateComboBox.SelectedIndex != -1 && ThirdDepositLeftDateComboBox.SelectedIndex != 0)
                {
                    if (ThirdDepositLeftDatePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = ThirdDepositLeftDatePicker.SelectedDate;
                        switch (ThirdDepositLeftDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftDateSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftDateSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (ThirdDepositRightDateComboBox.SelectedIndex != -1 && ThirdDepositRightDateComboBox.SelectedIndex != 0)
                {
                    if (ThirdDepositRightDatePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = ThirdDepositRightDatePicker.SelectedDate;
                        switch (ThirdDepositRightDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightDateSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightDateSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (filterRange.isActive())
                {
                    GrantsFilters.ThirdDepositor = filterRange;
                }
            }
            //Средства Российских фондов поддержки науки
            {
                filterRange = new FilterRange();
                if (FourthDepositLeftComboBox.SelectedIndex != -1 && FourthDepositLeftComboBox.SelectedIndex != 0)
                {
                    if (FourthDepositLeftTextBox.Text != "")
                    {
                        filterRange.LeftValue = FourthDepositLeftTextBox.Text;
                        switch (FourthDepositLeftComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (FourthDepositRightComboBox.SelectedIndex != -1 && FourthDepositRightComboBox.SelectedIndex != 0)
                {
                    if (FourthDepositRightTextBox.Text != "")
                    {
                        filterRange.RightValue = FourthDepositRightTextBox.Text;
                        switch (FourthDepositRightComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (FourthDepositLeftDateComboBox.SelectedIndex != -1 && FourthDepositLeftDateComboBox.SelectedIndex != 0)
                {
                    if (FourthDepositLeftDatePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = FourthDepositLeftDatePicker.SelectedDate;
                        switch (FourthDepositLeftDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftDateSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftDateSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (FourthDepositRightDateComboBox.SelectedIndex != -1 && FourthDepositRightDateComboBox.SelectedIndex != 0)
                {
                    if (FourthDepositRightDatePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = FourthDepositRightDatePicker.SelectedDate;
                        switch (FourthDepositRightDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightDateSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightDateSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (filterRange.isActive())
                {
                    GrantsFilters.FourthDepositor = filterRange;
                }
            }
            //Средства хозяйствующих субъектов
            {
                filterRange = new FilterRange();
                if (FifthDepositLeftComboBox.SelectedIndex != -1 && FifthDepositLeftComboBox.SelectedIndex != 0)
                {
                    if (FifthDepositLeftTextBox.Text != "")
                    {
                        filterRange.LeftValue = FifthDepositLeftTextBox.Text;
                        switch (FifthDepositLeftComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (FifthDepositRightComboBox.SelectedIndex != -1 && FifthDepositRightComboBox.SelectedIndex != 0)
                {
                    if (FifthDepositRightTextBox.Text != "")
                    {
                        filterRange.RightValue = FifthDepositRightTextBox.Text;
                        switch (FifthDepositRightComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (FifthDepositLeftDateComboBox.SelectedIndex != -1 && FifthDepositLeftDateComboBox.SelectedIndex != 0)
                {
                    if (FifthDepositLeftDatePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = FifthDepositLeftDatePicker.SelectedDate;
                        switch (FifthDepositLeftDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftDateSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftDateSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (FifthDepositRightDateComboBox.SelectedIndex != -1 && FifthDepositRightDateComboBox.SelectedIndex != 0)
                {
                    if (FifthDepositRightDatePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = FifthDepositRightDatePicker.SelectedDate;
                        switch (FifthDepositRightDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightDateSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightDateSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (filterRange.isActive())
                {
                    GrantsFilters.FifthDepositor = filterRange;
                }
            }
            //Физ. лица
            {
                filterRange = new FilterRange();
                if (SixthDepositLeftComboBox.SelectedIndex != -1 && SixthDepositLeftComboBox.SelectedIndex != 0)
                {
                    if (SixthDepositLeftTextBox.Text != "")
                    {
                        filterRange.LeftValue = SixthDepositLeftTextBox.Text;
                        switch (SixthDepositLeftComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (SixthDepositRightComboBox.SelectedIndex != -1 && SixthDepositRightComboBox.SelectedIndex != 0)
                {
                    if (SixthDepositRightTextBox.Text != "")
                    {
                        filterRange.RightValue = SixthDepositRightTextBox.Text;
                        switch (SixthDepositRightComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (SixthDepositLeftDateComboBox.SelectedIndex != -1 && SixthDepositLeftDateComboBox.SelectedIndex != 0)
                {
                    if (SixthDepositLeftDatePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = SixthDepositLeftDatePicker.SelectedDate;
                        switch (SixthDepositLeftDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftDateSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftDateSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (SixthDepositRightDateComboBox.SelectedIndex != -1 && SixthDepositRightDateComboBox.SelectedIndex != 0)
                {
                    if (SixthDepositRightDatePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = SixthDepositRightDatePicker.SelectedDate;
                        switch (SixthDepositRightDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightDateSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightDateSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (filterRange.isActive())
                {
                    GrantsFilters.SixthDepositor = filterRange;
                }
            }
            //ФЦП мин обра или иные источники госзаказа(бюджет)
            {
                filterRange = new FilterRange();
                if (SeventhDepositLeftComboBox.SelectedIndex != -1 && SeventhDepositLeftComboBox.SelectedIndex != 0)
                {
                    if (SeventhDepositLeftTextBox.Text != "")
                    {
                        filterRange.LeftValue = SeventhDepositLeftTextBox.Text;
                        switch (SeventhDepositLeftComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (SeventhDepositRightComboBox.SelectedIndex != -1 && SeventhDepositRightComboBox.SelectedIndex != 0)
                {
                    if (SeventhDepositRightTextBox.Text != "")
                    {
                        filterRange.RightValue = SeventhDepositRightTextBox.Text;
                        switch (SeventhDepositRightComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (SeventhDepositLeftDateComboBox.SelectedIndex != -1 && SeventhDepositLeftDateComboBox.SelectedIndex != 0)
                {
                    if (SeventhDepositLeftDatePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = SeventhDepositLeftDatePicker.SelectedDate;
                        switch (SeventhDepositLeftDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.LeftDateSign = FilterRange.Signs.More;
                                break;
                            case 2:
                                filterRange.LeftDateSign = FilterRange.Signs.MoreEqual;
                                break;
                        }
                    }
                }
                if (SeventhDepositRightDateComboBox.SelectedIndex != -1 && SeventhDepositRightDateComboBox.SelectedIndex != 0)
                {
                    if (SeventhDepositRightDatePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = SeventhDepositRightDatePicker.SelectedDate;
                        switch (SeventhDepositRightDateComboBox.SelectedIndex)
                        {
                            case 1:
                                filterRange.RightDateSign = FilterRange.Signs.Less;
                                break;
                            case 2:
                                filterRange.RightDateSign = FilterRange.Signs.LessEqual;
                                break;
                        }
                    }
                }
                if (filterRange.isActive())
                {
                    GrantsFilters.SeventhDepositor = filterRange;
                }
            }


            if (SelectedSecondNode.Count > 0)
            {
                GrantsFilters.FirstNode = SelectedFirstNode;
            }

            if (SelectedSecondNode.Count > 0)
            {
                GrantsFilters.SecondNode = SelectedSecondNode;
            }

            if(SelectedThirdNode.Count > 0)
            {
                GrantsFilters.ThirdNode = SelectedThirdNode;
            }
            
            if(SelectedFourthNode.Count > 0)
            {
                GrantsFilters.FourthNode = SelectedFourthNode;
            }

            if (SelectedResearchType.Count > 0)
            {
                GrantsFilters.ResearchType = SelectedResearchType;
            }

            if (SelectedScienceType.Count > 0)
            {
                GrantsFilters.ScienceType = SelectedScienceType;
            }

            if (SelectedPriorityTrend.Count > 0)
            {
                GrantsFilters.PriorityTrend = SelectedPriorityTrend;
            }

            GrantsFilters.GetFirstDepositorsQuarry();
        }
    }
}
