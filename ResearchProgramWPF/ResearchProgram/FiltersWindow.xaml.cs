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

    public class FilterElement
    {
        public string Data { get; set; }

    }


   public class GrantHeader: INotifyPropertyChanged
   {
        private string _nameOnRussia;
        public string nameOnRussia {
            get => _nameOnRussia;
            set
            {
                _nameOnRussia = value;
                OnPropertyChanged(nameof(nameOnRussia));
            }
        }
        public string nameForElement { get; set; }

        // Нужен вывод комбобокса
        private bool _is_combobox_needed;
        public bool Is_combobox_needed {
            get => _is_combobox_needed;
            set
            {
                _is_combobox_needed = value;
                OnPropertyChanged(nameof(Is_combobox_needed));
            }
        }
        // Нужен вывод текстбокса
        public bool Is_textbox_needed { get; set; }
        // Нужен вывод сравнимого выражения
        public bool Is_comparison_needed { get; set; }
        // Нужен вывод датапикера
        public bool Is_date_needed { get; set; }


        public ObservableCollection<FilterElement> FilterElementsData { get; set; }


        public GrantHeader()
        {
            nameOnRussia = "";
            nameForElement = "";
            FilterElementsData = new ObservableCollection<FilterElement>();
        }


        public override string ToString()
        {
            return nameOnRussia;
        }

        // Реализация интерфейса
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }


    /// <summary>
    /// Логика взаимодействия для FiltersWindow.xaml
    /// </summary>
    public partial class FiltersWindow : Window, INotifyPropertyChanged
    {

        private ObservableCollection<GrantHeader> _listOfGrantHeaders { get; set; }
        public ObservableCollection<GrantHeader> GrantHeaders { get; set; }

        // Коллекция для создания фильтров
        public ObservableCollection<GrantHeader> GrantItemsForControlPanel { get; set; }

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

            InitializeData();

            SetGrantHeaders();

            this.DataContext = this;
        }

        private void InitializeData()
        {
            GrantItemsForControlPanel = new ObservableCollection<GrantHeader>();
        }

        /// <summary>
        /// Получение всех заголовков
        /// </summary>
        private void SetGrantHeaders()
        {
            CRUDDataBase.ConnectByDataBase();
            GrantHeaders = CRUDDataBase.GetGrantsHeadersForFilters();
            CRUDDataBase.CloseConnect();
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
                GrantItemsForControlPanel.Add(_selectedGrantHeader);
                filtersTabControl.SelectedItem = _selectedGrantHeader;

                removeFromHeadersFilters(SelectedGrantHeader);

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

        /// <summary>
        /// Добавление нового параметра в фильтр
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNewFilterParameter(object sender, RoutedEventArgs e)
        {
            ((GrantHeader)filtersTabControl.SelectedItem).FilterElementsData.Add(new FilterElement()
            {
                Data = "abcd"
            });
        }

        /// <summary>
        /// Удаление выбранного элемента из фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFilterParameter(object sender, RoutedEventArgs e)
        {

        }

        //private void scienceTypeAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void scienceTypeDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void executorOnContractDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void RadioButton_Checked(object sender, RoutedEventArgs e)
        //{

        //}

        //private void depositsAddButton_Click_1(object sender, RoutedEventArgs e)
        //{

        //}

        //private void depositsDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void okvedAddButton_Click(object sender, RoutedEventArgs e)
        //{
        //    TextBox textBox = new TextBox() {
        //        MinWidth = 400,
        //        MinHeight = 22,
        //        FontSize = 13,
        //        Margin = new Thickness(40, 10, 5, 10),
        //    };

        //    okvedVerticalListView.Items.Add(textBox);
        //}

        //private void okvedDeleteButton_Click(object sender, RoutedEventArgs e)
        //{
        //    int countSelectedElement = okvedVerticalListView.SelectedItems.Count;
        //    if (countSelectedElement > 0)
        //    {
        //        for (int i = 0; i < countSelectedElement; i++)
        //        {
        //            okvedVerticalListView.Items.Remove(okvedVerticalListView.SelectedItems[0]);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Выделите нужный для удаления элемент");
        //    }
        //}

        //private void grantNumberAddButton_Click(object sender, RoutedEventArgs e)
        //{
        //    TextBox textBox = new TextBox()
        //    {
        //        MinWidth = 400,
        //        MinHeight = 22,
        //        FontSize = 13,
        //        Margin = new Thickness(40, 10, 5, 10),
        //    };

        //    numberOfGrantsVerticalListView.Items.Add(textBox);
        //}

        //private void grantNumberDeleteButton_Click(object sender, RoutedEventArgs e)
        //{
        //    int countSelectedElement = numberOfGrantsVerticalListView.SelectedItems.Count;
        //    if (countSelectedElement > 0)
        //    {
        //        for (int i = 0; i < countSelectedElement; i++)
        //        {
        //            numberOfGrantsVerticalListView.Items.Remove(numberOfGrantsVerticalListView.SelectedItems[0]);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Выделите нужный для удаления элемент");
        //    }
        //}

        //private void NIOKRAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void NIOKRDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void customerAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void customerDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void startDateAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void startDateDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void endDateAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void endDateDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void priceAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void priceDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void leadNIOKRAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void leadNIOKRDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void executorsAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void executorsDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void kafedraAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void kafedraDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void unitAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void unitDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void institutionAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void institutionDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void GRNTIAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void GRNTIDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void priorityTrendsAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void priorityTrendsDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ExecutorsContractItemAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ExecutorsContractItemDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ScienceTypeItemAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ScienceTypeItemDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void NOCItemAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void NOCItemDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void researchTypesAddButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void researchTypesDeleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
