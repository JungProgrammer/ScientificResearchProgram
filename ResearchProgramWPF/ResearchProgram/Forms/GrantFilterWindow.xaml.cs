﻿using ResearchProgram.Classes;
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
using Xceed.Wpf.AvalonDock.Layout;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для NewFilterWindow.xaml
    /// </summary>
    public partial class GrantFilterWindow : Window, INotifyPropertyChanged
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
        public GrantFilterWindow(DataTable grantsDataTable)
        {
            InitializeComponent();


            GrantsDataTable = grantsDataTable;

            FirstNodeList = StaticData.GetUniversityStructureNodeByRegex("^[0-9]+$"); // получение всех узлов с адресом первого уровня
            SecondNodeList = StaticData.GetUniversityStructureNodeByRegex("^[0-9]+\\.[0-9]+$"); // получение всех узлов с адресом второго уровня
            ThirdNodeList = StaticData.GetUniversityStructureNodeByRegex("^[0-9]+\\.[0-9]+\\.[0-9]+$"); // получение всех узлов с адресом третьего уровня
            FourthNodeList = StaticData.GetUniversityStructureNodeByRegex("^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$"); // получение всех узлов с адресом четвертого уровня

            People = new ObservableCollection<Person>(StaticData.GetAllPersons());
            Customers = new ObservableCollection<Customer>(StaticData.GetAllCustomers());
            ResearchTypes = new ObservableCollection<ResearchType>(StaticData.GetAllResearchTypes());
            ScienceTypes = new ObservableCollection<ScienceType>(StaticData.GetAllScienceTypes());
            PriorityTrends = new ObservableCollection<PriorityTrend>(StaticData.GetAllPriorityTrends());

            LeadNIOKRMultiselectComboBox.ItemsSource = new ObservableCollection<Person>(People);
            CustomerMultiselectComboBox.ItemsSource = Customers;
            ExecutorMultiselectComboBox.ItemsSource = new ObservableCollection<Person>(People);

            Okved = new ObservableCollection<OKVED>
            {
                new OKVED() {Title = "72.19"},
                new OKVED() {Title = "72.20"}
            };

            List<FrameworkElement> frameworkElements = new List<FrameworkElement>();
            Utilities.GetLogicalElements(this, frameworkElements, "LeftTextBox");
            Utilities.GetLogicalElements(this, frameworkElements, "RightTextBox");
            foreach (FrameworkElement frameworkElement in frameworkElements)
            {
                frameworkElement.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
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

            IEnumerable<CheckBox> checkBoxes = Utilities.FindVisualChildren<CheckBox>(this);
            foreach (CheckBox checkBox in checkBoxes)
            {
                checkBox.IsChecked = false;
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
        private void SimpleSearchButton_Click(object sender, RoutedEventArgs e)
        {
            GrantsFilters.ResetFilters();
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);

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
            GrantsFilters.grantFilter = filter;
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

            //Средства
            List<FrameworkElement> logicalElements = new List<FrameworkElement>();
            Utilities.GetLogicalElements(this, logicalElements, "DepositorDockPanel");

            int dockPanelCount = 0;
            foreach (FrameworkElement element in logicalElements)
            {
                ComboBox comboBox;
                TextBox textBox;
                DatePicker datePicker;
                filterRange = new FilterRange();

                List<FrameworkElement> tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "LeftTextBox");
                textBox = (TextBox)tempLogicalElemenets[0];

                tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "LeftComboBox");
                comboBox = (ComboBox)tempLogicalElemenets[0];
                if (comboBox.SelectedIndex != -1 && comboBox.SelectedIndex != 0)
                {
                    if (textBox.Text != "")
                    {
                        filterRange.LeftValue = textBox.Text;
                        switch (comboBox.SelectedIndex)
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

                tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "RightTextBox");
                textBox = (TextBox)tempLogicalElemenets[0];

                tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "RightComboBox");
                comboBox = (ComboBox)tempLogicalElemenets[0];
                if (comboBox.SelectedIndex != -1 && comboBox.SelectedIndex != 0)
                {
                    if (textBox.Text != "")
                    {
                        filterRange.RightValue = textBox.Text;
                        switch (comboBox.SelectedIndex)
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

                tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "LeftDatePicker");
                datePicker = (DatePicker)tempLogicalElemenets[0];

                tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "LeftDateComboBox");
                comboBox = (ComboBox)tempLogicalElemenets[0];
                if (comboBox.SelectedIndex != -1 && comboBox.SelectedIndex != 0)
                {
                    if (datePicker.SelectedDate != null)
                    {
                        filterRange.LeftDate = datePicker.SelectedDate;
                        switch (comboBox.SelectedIndex)
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

                tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "RightDatePicker");
                datePicker = (DatePicker)tempLogicalElemenets[0];

                tempLogicalElemenets = new List<FrameworkElement>();
                Utilities.GetLogicalElements(element, tempLogicalElemenets, "RightDateComboBox");
                comboBox = (ComboBox)tempLogicalElemenets[0];

                if (comboBox.SelectedIndex != -1 && comboBox.SelectedIndex != 0)
                {
                    if (datePicker.SelectedDate != null)
                    {
                        filterRange.RightDate = datePicker.SelectedDate;
                        switch (comboBox.SelectedIndex)
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
                    if (GrantsFilters.Depositors == null)
                    {
                        GrantsFilters.Depositors = new ObservableCollection<FilterRange>();
                        for (int i = 0; i < StaticData.depositsVerbose.Count; i++)
                        {
                            GrantsFilters.Depositors.Add(new FilterRange());
                        }
                    }
                    GrantsFilters.Depositors[dockPanelCount] = filterRange;
                }
                dockPanelCount++;
            }

            if (SelectedFirstNode.Count > 0)
            {
                GrantsFilters.FirstNode = SelectedFirstNode;
            }

            if (SelectedSecondNode.Count > 0)
            {
                GrantsFilters.SecondNode = SelectedSecondNode;
            }

            if (SelectedThirdNode.Count > 0)
            {
                GrantsFilters.ThirdNode = SelectedThirdNode;
            }

            if (SelectedFourthNode.Count > 0)
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
            GrantsDataTable.DefaultView.RowFilter = null;

            Console.WriteLine(GrantsFilters.IsActive());
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
        }
    }
}
