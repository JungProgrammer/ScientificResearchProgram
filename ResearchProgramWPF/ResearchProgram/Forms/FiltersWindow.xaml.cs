using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ResearchProgram
{

    /// <summary>
    /// Логика взаимодействия для FiltersWindow.xaml
    /// </summary>
    public partial class FiltersWindow : Window, INotifyPropertyChanged
    {
        // Ответает за возможность закрытия окна
        public bool WindowCanToBeClose = false;


        // Таблица, которая отвечает за гранты
        DataTable GrantsDataTable { get; set; }


        private ObservableCollection<GrantHeader> _grantHeaders;
        public ObservableCollection<GrantHeader> GrantHeaders
        {
            get => _grantHeaders;
            set
            {
                _grantHeaders = value;
                OnPropertyChanged(nameof(GrantHeaders));
            }
        }


        // Коллекция для создания фильтров
        public ObservableCollection<GrantHeader> GrantItemsForControlPanel { get; set; }

        private GrantHeader _selectedGrantHeader;
        public GrantHeader SelectedGrantHeader
        {
            get => _selectedGrantHeader;
            set
            {
                _selectedGrantHeader = value;
                OnPropertyChanged(nameof(SelectedGrantHeader));
            }
        }


        private GrantHeader _selectedGrantHeaderOnTabControl;
        public GrantHeader SelectedGrantHeaderOnTabControl
        {
            get { return _selectedGrantHeaderOnTabControl; }
            set
            {
                _selectedGrantHeaderOnTabControl = value;
                OnPropertyChanged(nameof(SelectedGrantHeaderOnTabControl));
            }
        }


        public FiltersWindow(DataTable grantsDataTable)
        {
            InitializeComponent();

            // Присваиваем ссылку на GrantsDataTable из main
            GrantsDataTable = grantsDataTable;

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
            CRUDDataBase.ConnectToDataBase();
            GrantHeaders = CRUDDataBase.GetGrantsHeadersForFilters();
            CRUDDataBase.CloseConnection();
            SelectedGrantHeader = GrantHeaders[0];
        }


        /// <summary>
        /// Добавление нового фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>    
        private void addFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGrantHeader != null)
            {
                GrantItemsForControlPanel.Add(_selectedGrantHeader);
                filtersTabControl.SelectedItem = _selectedGrantHeader;

                removeFromHeadersFilters(SelectedGrantHeader);

                if (GrantHeaders.Count > 0)
                {
                    SelectedGrantHeader = GrantHeaders[0];
                }
            }
            else
            {
                MessageBox.Show("Выберите фильтр", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    Data = curGrantHeader.ChooseDataFromCombobox != null ? curGrantHeader.ChooseDataFromCombobox.GetTitle() : ""
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
            // Если на странице Data
            else if (curGrantHeader.Is_date_needed)
            {
                curGrantHeader.FilterElementsData.Add(new FilterElement()
                {
                    Data = curGrantHeader.ChooseDateFromDatePicker.ToString("dd.MM.yyyy")
                });
            }
            // Если на странице сумма
            else if (curGrantHeader.Is_comparison_needed)
            {
                curGrantHeader.FilterElementsData.Add(new FilterElement()
                {
                    Data = curGrantHeader.ChooseAllSum,
                    Sign = curGrantHeader.ChooseComparisonSign
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
            SelectedGrantHeaderOnTabControl.FilterElementsData.Remove(SelectedGrantHeaderOnTabControl.SelectedFilter);
        }

        /// <summary>
        /// Применить поставленные фильтры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            // Для начала применить новые фильтры
            UpdateGrantsFilters();

            CRUDDataBase.ConnectToDataBase();
            // Загрузить снова таблицу
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnection();
        }

        /// <summary>
        /// Обновляет статический класс фильтров
        /// </summary>
        private void UpdateGrantsFilters()
        {
            // Сбрасываем фильтры
            GrantsFilters.ResetFilters();

            // Поиск фильтров
            GrantsFilters.grantNumber = FindGrantHeader(GrantItemsForControlPanel, "grantNumber") != null ? FindGrantHeader(GrantItemsForControlPanel, "grantNumber").FilterElementsData : null;
            GrantsFilters.OKVED = FindGrantHeader(GrantItemsForControlPanel, "OKVED") != null ? FindGrantHeader(GrantItemsForControlPanel, "OKVED").FilterElementsData : null;
            GrantsFilters.NameNIOKR = FindGrantHeader(GrantItemsForControlPanel, "NIOKR") != null ? FindGrantHeader(GrantItemsForControlPanel, "NIOKR").FilterElementsData : null;
            GrantsFilters.Customer = FindGrantHeader(GrantItemsForControlPanel, "customer") != null ? FindGrantHeader(GrantItemsForControlPanel, "customer").FilterElementsData : null;
            GrantsFilters.StartDate = FindGrantHeader(GrantItemsForControlPanel, "startDate") != null ? FindGrantHeader(GrantItemsForControlPanel, "startDate").FilterElementsData : null;
            GrantsFilters.EndDate = FindGrantHeader(GrantItemsForControlPanel, "endDate") != null ? FindGrantHeader(GrantItemsForControlPanel, "endDate").FilterElementsData : null;
            GrantsFilters.Price = FindGrantHeader(GrantItemsForControlPanel, "price") != null ? FindGrantHeader(GrantItemsForControlPanel, "price").FilterElementsData : null;
            GrantsFilters.Depositor = FindGrantHeader(GrantItemsForControlPanel, "deposits") != null ? FindGrantHeader(GrantItemsForControlPanel, "deposits").FilterElementsData : null;
            GrantsFilters.LeadNIOKR = FindGrantHeader(GrantItemsForControlPanel, "leadNIOKR") != null ? FindGrantHeader(GrantItemsForControlPanel, "leadNIOKR").FilterElementsData : null;
            GrantsFilters.Executor = FindGrantHeader(GrantItemsForControlPanel, "executors") != null ? FindGrantHeader(GrantItemsForControlPanel, "executors").FilterElementsData : null;
            GrantsFilters.Kafedra = FindGrantHeader(GrantItemsForControlPanel, "kafedra") != null ? FindGrantHeader(GrantItemsForControlPanel, "kafedra").FilterElementsData : null;
            GrantsFilters.Unit = FindGrantHeader(GrantItemsForControlPanel, "unit") != null ? FindGrantHeader(GrantItemsForControlPanel, "unit").FilterElementsData : null;
            GrantsFilters.Laboratory = FindGrantHeader(GrantItemsForControlPanel, "laboratory") != null ? FindGrantHeader(GrantItemsForControlPanel, "laboratory").FilterElementsData : null;
            GrantsFilters.Institution = FindGrantHeader(GrantItemsForControlPanel, "institution") != null ? FindGrantHeader(GrantItemsForControlPanel, "institution").FilterElementsData : null;
            GrantsFilters.GRNTI = FindGrantHeader(GrantItemsForControlPanel, "GRNTI") != null ? FindGrantHeader(GrantItemsForControlPanel, "GRNTI").FilterElementsData : null;
            GrantsFilters.ResearchTypes = FindGrantHeader(GrantItemsForControlPanel, "researchTypes") != null ? FindGrantHeader(GrantItemsForControlPanel, "researchTypes").FilterElementsData : null;
            GrantsFilters.PriorityTrands = FindGrantHeader(GrantItemsForControlPanel, "priorityTrends") != null ? FindGrantHeader(GrantItemsForControlPanel, "priorityTrends").FilterElementsData : null;
            GrantsFilters.ScienceTypes = FindGrantHeader(GrantItemsForControlPanel, "ScienceTypeItem") != null ? FindGrantHeader(GrantItemsForControlPanel, "ScienceTypeItem").FilterElementsData : null;
            GrantsFilters.NIR = FindGrantHeader(GrantItemsForControlPanel, "NIRItem") != null ? FindGrantHeader(GrantItemsForControlPanel, "NIRItem").FilterElementsData : null;
            GrantsFilters.NOC = FindGrantHeader(GrantItemsForControlPanel, "NOCItem") != null ? FindGrantHeader(GrantItemsForControlPanel, "NOCItem").FilterElementsData : null;
        }

        /// <summary>
        /// Выполняет поиск айтема по найзванию фильтра на английском
        /// </summary>
        /// <param name="grantHeaders"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private GrantHeader FindGrantHeader(ObservableCollection<GrantHeader> grantHeaders, string elementName)
        {
            foreach (GrantHeader grantHeader in grantHeaders)
            {
                if (grantHeader.nameForElement == elementName)
                {
                    return grantHeader;
                }
            }

            return null;
        }

        /// <summary>
        /// Событие, которое происходит, когда нажимается кнопка сброса фильтров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetAllFilters(object sender, RoutedEventArgs e)
        {
            // Удаление всех видов фильтров, которые остались невыбранные в combobox
            GrantHeaders.Clear();
            // Восстановить все возможные фильтры в combobox
            SetGrantHeaders();
            // Удаление всех фильтров, которые уже пользователь добавил
            GrantItemsForControlPanel.Clear();
            // Сброс текущих фильтров у текущего класса
            GrantsFilters.ResetFilters();

            // Загрузить снова таблицу
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.LoadGrantsTable(GrantsDataTable);
            CRUDDataBase.CloseConnection();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!WindowCanToBeClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
