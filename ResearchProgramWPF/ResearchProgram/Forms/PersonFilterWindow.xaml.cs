using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
    /// Логика взаимодействия для PersonFilterWindow.xaml
    /// </summary>
    public partial class PersonFilterWindow : Window, INotifyPropertyChanged
    {
        private const int SIMPLE_SEARCH_HEIGHT = 150;
        private const int EXTENDED_SEARCH_HEIGHT = 700;
        private const int SIMPLE_SEARCH_WIDTH = 600;
        private const int EXTENDED_SEARCH_WIDTH = 1100;

        private readonly double ScreenWidth = SystemParameters.PrimaryScreenWidth;
        private readonly double ScreenHeight = SystemParameters.PrimaryScreenHeight;
        private double WindowWidth;
        private double WindowHeight;

        private ObservableCollection<UniversityStructureNode> _firstNodeList;
        private ObservableCollection<UniversityStructureNode> _secondNodeList;
        private ObservableCollection<UniversityStructureNode> _thirdNodeList;
        private ObservableCollection<UniversityStructureNode> _fourthNodeList;
        public ObservableCollection<UniversityStructureNode> FirstNode { get { return _firstNodeList; } set { _firstNodeList = value; OnPropertyChanged(nameof(FirstNode)); } }
        public ObservableCollection<UniversityStructureNode> SecondNode { get { return _secondNodeList; } set { _secondNodeList = value; OnPropertyChanged(nameof(SecondNode)); } }
        public ObservableCollection<UniversityStructureNode> ThirdNode { get { return _thirdNodeList; } set { _thirdNodeList = value; OnPropertyChanged(nameof(ThirdNode)); } }
        public ObservableCollection<UniversityStructureNode> FourthNode { get { return _fourthNodeList; } set { _fourthNodeList = value; OnPropertyChanged(nameof(FourthNode)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        // Таблица, которая отвечает за гранты
        DataTable PersonsDataTable { get; set; }

        private ObservableCollection<UniversityStructureNode> _selectedFirstNode;
        private ObservableCollection<UniversityStructureNode> _selectedSecondNode;
        private ObservableCollection<UniversityStructureNode> _selectedThirdNode;
        private ObservableCollection<UniversityStructureNode> _selectedFourthNode;
        public ObservableCollection<UniversityStructureNode> SelectedFirstNode { get { return _selectedFirstNode; } set { _selectedFirstNode = value; OnPropertyChanged(nameof(SelectedFirstNode)); } }
        public ObservableCollection<UniversityStructureNode> SelectedSecondNode { get { return _selectedSecondNode; } set { _selectedSecondNode = value; OnPropertyChanged(nameof(SelectedSecondNode)); } }
        public ObservableCollection<UniversityStructureNode> SelectedThirdNode { get { return _selectedThirdNode; } set { _selectedThirdNode = value; OnPropertyChanged(nameof(SelectedThirdNode)); } }
        public ObservableCollection<UniversityStructureNode> SelectedFourthNode { get { return _selectedFourthNode; } set { _selectedFourthNode = value; OnPropertyChanged(nameof(SelectedFourthNode)); } }

        private ObservableCollection<WorkDegree> _degree;
        public ObservableCollection<WorkDegree> Degree { get { return _degree; } set { _degree = value; OnPropertyChanged(nameof(Degree)); } }

        private ObservableCollection<WorkDegree> _selectedDegree;
        public ObservableCollection<WorkDegree> SelectedDegree { get { return _selectedDegree; } set { _selectedDegree = value; OnPropertyChanged(nameof(SelectedDegree)); } }

        private ObservableCollection<WorkRank> _rank;
        public ObservableCollection<WorkRank> Rank { get { return _rank; } set { _rank = value; OnPropertyChanged(nameof(Rank)); } }

        private ObservableCollection<WorkRank> _selectedRank;
        public ObservableCollection<WorkRank> SelectedRank { get { return _selectedRank; } set { _selectedRank = value; OnPropertyChanged(nameof(SelectedRank)); } }

        private ObservableCollection<WorkCategories> _category;
        public ObservableCollection<WorkCategories> Category { get { return _category; } set { _category = value; OnPropertyChanged(nameof(Category)); } }

        private ObservableCollection<WorkCategories> _selectedCategory;
        public ObservableCollection<WorkCategories> SelectedCategory { get { return _selectedCategory; } set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); } }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public PersonFilterWindow(DataTable personsDataTable)
        {
            InitializeComponent();


            PersonsDataTable = personsDataTable;

            CRUDDataBase.ConnectToDataBase();

            FirstNode = CRUDDataBase.GetStructureNodes("'^[0-9]+$'"); // получение всех узлов с адресом первого уровня
            SecondNode = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом второго уровня
            ThirdNode = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом третьего уровня
            FourthNode = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом четвертого уровня
            Degree = new ObservableCollection<WorkDegree>(CRUDDataBase.GetWorkDegrees());
            Rank = new ObservableCollection<WorkRank>(CRUDDataBase.GetWorkRanks());
            Category = new ObservableCollection<WorkCategories>(CRUDDataBase.GetWorkCategories());

            CRUDDataBase.CloseConnection();

            List<FrameworkElement> frameworkElements = new List<FrameworkElement>();
            Utilities.GetLogicalElements(this, frameworkElements, "LeftTextBox");
            Utilities.GetLogicalElements(this, frameworkElements, "RightTextBox");
            foreach (FrameworkElement frameworkElement in frameworkElements)
            {
                frameworkElement.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
            }


            SelectedFirstNode = new ObservableCollection<UniversityStructureNode>();
            SelectedSecondNode = new ObservableCollection<UniversityStructureNode>();
            SelectedThirdNode = new ObservableCollection<UniversityStructureNode>();
            SelectedFourthNode = new ObservableCollection<UniversityStructureNode>();
            SelectedCategory = new ObservableCollection<WorkCategories>();
            SelectedDegree = new ObservableCollection<WorkDegree>();
            SelectedRank = new ObservableCollection<WorkRank>();

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

            SelectedFirstNode = new ObservableCollection<UniversityStructureNode>();
            SelectedSecondNode = new ObservableCollection<UniversityStructureNode>();
            SelectedThirdNode = new ObservableCollection<UniversityStructureNode>();
            SelectedFourthNode = new ObservableCollection<UniversityStructureNode>();
            SelectedCategory = new ObservableCollection<WorkCategories>();
            SelectedDegree = new ObservableCollection<WorkDegree>();
            SelectedRank = new ObservableCollection<WorkRank>();

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
                PersonsDataTable.DefaultView.RowFilter = null;
                return;
            }

            // составляется условие поиска и происходит фильтрация
            DataColumnCollection columns = PersonsDataTable.Columns;

            string filter = string.Empty;
            for (int i = 0; i < columns.Count; i++)
            {
                filter += "[" + columns[i].ColumnName + "] LIKE \'%" + searchQuarry + "%\'";
                if (i != columns.Count - 1)
                {
                    filter += " OR ";
                }
            }
            PersonsDataTable.DefaultView.RowFilter = filter;
        }

        private void ComplexSearchButton_Click(object sender, RoutedEventArgs e)
        {
            PersonsFilters.ResetFilters();
            PersonsDataTable.DefaultView.RowFilter = null;

            IEnumerable<TextBox> textBoxes = Utilities.FindVisualChildren<TextBox>(this);
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Text = textBox.Text.Trim();
            }

            if (FIOtextBox.Text != "")
            {
                PersonsFilters.FIO = FIOtextBox.Text;
            }

            if ((bool)MaleCheckBox.IsChecked)
            {
                PersonsFilters.Sex = true;
            }

            if ((bool)FemaleCheckBox.IsChecked)
            {
                PersonsFilters.Sex = false;
            }

            if (SelectedDegree.Count > 0)
            {
                PersonsFilters.workDegrees = SelectedDegree;
            }

            if (SelectedRank.Count > 0)
            {
                PersonsFilters.workRanks = SelectedRank;
            }

            if (SelectedCategory.Count > 0)
            {
                PersonsFilters.WorkCategories = SelectedCategory;
            }

            if ((bool)MainWorkPlaceCheckBox.IsChecked)
            {
                PersonsFilters.IsMainWorkPlace = true;
            }

            if (SelectedFirstNode.Count > 0)
            {
                PersonsFilters.FirstNode = SelectedFirstNode;
            }

            if (SelectedSecondNode.Count > 0)
            {
                PersonsFilters.SecondNode = SelectedSecondNode;
            }

            if (SelectedThirdNode.Count > 0)
            {
                PersonsFilters.ThirdNode = SelectedThirdNode;
            }

            if (SelectedFourthNode.Count > 0)
            {
                PersonsFilters.FourthNode = SelectedFourthNode;
            }

            FilterRange filterRange = new FilterRange();
            if (AgeLeftComboBox.SelectedIndex != -1 && AgeLeftComboBox.SelectedIndex != 0)
            {
                if (AgeLeftTextBox.Text != "")
                {
                    filterRange.LeftValue = AgeLeftTextBox.Text;
                    switch (AgeLeftComboBox.SelectedIndex)
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

            if (AgeRightComboBox.SelectedIndex != -1 && AgeRightComboBox.SelectedIndex != 0)
            {
                if (AgeRightTextBox.Text != "")
                {
                    filterRange.RightValue = AgeRightTextBox.Text;
                    switch (AgeRightComboBox.SelectedIndex)
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
                PersonsFilters.Age = filterRange;
            }

            filterRange = new FilterRange();
            if (BirthDateLeftDateComboBox.SelectedIndex != -1 && BirthDateLeftDateComboBox.SelectedIndex != 0)
            {
                if (BirthDateLeftDatePicker.SelectedDate != null)
                {
                    filterRange.LeftDate = BirthDateLeftDatePicker.SelectedDate;
                    switch (BirthDateLeftDateComboBox.SelectedIndex)
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

            if (BirthDateRightDateComboBox.SelectedIndex != -1 && BirthDateRightDateComboBox.SelectedIndex != 0)
            {
                if (BirthDateRightComboBox.SelectedDate != null)
                {
                    filterRange.RightDate = BirthDateRightComboBox.SelectedDate;
                    switch (BirthDateRightDateComboBox.SelectedIndex)
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
                PersonsFilters.BirthDate = filterRange;
            }



            Console.WriteLine(PersonsFilters.IsActive());
            //CRUDDataBase.ConnectToDataBase();
            //CRUDDataBase.LoadPersonsTable(PersonsDataTable);
            //CRUDDataBase.CloseConnection();
        }

        private void MaleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FemaleCheckBox.IsChecked = false;
        }

        private void FemaleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MaleCheckBox.IsChecked = false;
        }
    }
}
