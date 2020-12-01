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


    public class GrantHeader : INotifyPropertyChanged
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
        public bool Is_combobox_needed { get; set; }
        // Данные для ItemSource этого комбобокса
        public List<IContainer> DataToComboBox { get; set; }
        // Выбранное значение для этого комбобокса
        public IContainer ChooseDataFromCombobox { get; set; }

        // Нужен вывод текстбокса
        public bool Is_textbox_needed { get; set; }
        // Текст для текстбоксового поля
        public string ChooseDataFromTextBox { get; set; }

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
            DataToComboBox = new List<IContainer>();
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
            GrantHeader curGrantHeader = ((GrantHeader)filtersTabControl.SelectedItem);

            // Если на странице ComboBox
            if (curGrantHeader.Is_combobox_needed)
            {
                curGrantHeader.FilterElementsData.Add(new FilterElement()
                {
                    Data = curGrantHeader.ChooseDataFromCombobox.GetTitle()
                });
            }
            // Если на странице textbox
            else if (curGrantHeader.Is_textbox_needed)
            {
                curGrantHeader.FilterElementsData.Add(new FilterElement()
                {
                    Data = curGrantHeader.ChooseDataFromTextBox
                });
            }
        }

        /// <summary>
        /// Удаление выбранного элемента из фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFilterParameter(object sender, RoutedEventArgs e)
        {

        }

    }
}
