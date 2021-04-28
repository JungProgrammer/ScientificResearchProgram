using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        //ObservableCollection<Person> persons = CrGetPersons();

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People { get { return _people; } set { _people = value; OnPropertyChanged(nameof(People)); } }

        private ObservableCollection<Person> _customers;
        public ObservableCollection<Person> Customers { get { return _customers; } set { _customers = value; OnPropertyChanged(nameof(Customers)); } }

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


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public NewFilterWindow()
        {
            InitializeComponent();

            CRUDDataBase.ConnectToDataBase();

            People = CRUDDataBase.GetPersons();
            Console.WriteLine(People.Count);

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



            DataContext = this;
        }

        private void SearchExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(SearchExpander.IsExpanded);
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
    }
}
