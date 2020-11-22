using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResearchProgram
{

   public class GrantHeader
    {
        public string nameOnRussia;
        public string nameForElement;


        public GrantHeader()
        {
            nameOnRussia = "";
            nameForElement = "";
        }

        public override string ToString()
        {
            return nameOnRussia;
        }
    }

    /// <summary>
    /// Логика взаимодействия для FiltersWindow.xaml
    /// </summary>
    public partial class FiltersWindow : Window, INotifyPropertyChanged
    {

        private ObservableCollection<GrantHeader> _listOfGrantHeaders { get; set; }
        public ObservableCollection<GrantHeader> GrantHeaders
        {
            get => _listOfGrantHeaders;
            set
            {
                _listOfGrantHeaders = value;
                OnPropertyChanged(nameof(GrantHeaders));
            }
        }


        private GrantHeader _selectedGrantHeader;
        public GrantHeader SelectedGrantHeader { 
            get => _selectedGrantHeader; 
            set
            {
                _selectedGrantHeader = value;
                OnPropertyChanged(nameof(SelectedGrantHeader));
            }
        }


        public FiltersWindow()
        {
            InitializeComponent();

            SetGrantHeaders();

            DataContext = this;
        }

        /// <summary>
        /// Получение всех заголовков
        /// </summary>
        private void SetGrantHeaders()
        {
            CRUDDataBase.ConnectByDataBase();
            _listOfGrantHeaders = CRUDDataBase.GetGrantsHeadersForFilters();
            CRUDDataBase.CloseConnect();
            
            // Удаляем первый элемент, потому что у него изначально есть TabItem
            _listOfGrantHeaders.Remove(_listOfGrantHeaders[0]);
        }

        /// <summary>
        /// Применить поставленные фильтры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyFiltersButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Добавление нового фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>    
        private void addFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedGrantHeader != null)
            {
                filtersTabConrol.SelectedItem = filtersTabConrol.Items.OfType<TabItem>().SingleOrDefault(n => n.Name == _selectedGrantHeader.nameForElement);
                ((TabItem)(filtersTabConrol.SelectedItem)).Visibility = Visibility.Visible;

                removeFromHeadersFilters(_selectedGrantHeader);

                if(GrantHeaders.Count > 0)
                {
                    SelectedGrantHeader = GrantHeaders[0];
                }
            }
            else
            {
                MessageBox.Show("Выберите фильтр");
            }

        }

        /// <summary>
        /// Удаляет фильтр их списка фильтров
        /// </summary>
        private void removeFromHeadersFilters(GrantHeader _selectedGrantHeader)
        {
            GrantHeaders.Remove(_selectedGrantHeader);
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void scienceTypeAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void scienceTypeDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void executorOnContractDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void executorOnContractAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void priorityTrendDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void priorityTrendAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void executorAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void executorDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void depositsAddButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void depositsDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void okvedAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void okvedDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void grantNumberAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void grantNumberDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NIOKRAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NIOKRDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void customerAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void customerDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startDateAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startDateDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void endDateAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void endDateDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void priceAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void priceDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void leadNIOKRAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void leadNIOKRDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void executorsAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void executorsDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void kafedraAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void kafedraDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void unitAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void unitDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void institutionAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void institutionDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GRNTIAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GRNTIDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void priorityTrendsAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void priorityTrendsDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExecutorsContractItemAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExecutorsContractItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ScienceTypeItemAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ScienceTypeItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NOCItemAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NOCItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void researchTypesAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void researchTypesDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
