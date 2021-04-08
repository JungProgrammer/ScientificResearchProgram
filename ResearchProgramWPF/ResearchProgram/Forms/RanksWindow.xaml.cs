using ResearchProgram.Classes;
using ResearchProgram.Forms.HelpWindows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для RanksWindow.xaml
    /// </summary>
    public partial class RanksWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<WorkRank> RanksNames { get; set; }

        private WorkRank _selectedRankName;
        public WorkRank SelectedRankName
        {
            get => _selectedRankName;
            set
            {
                _selectedRankName = value;
                OnPropertyChanged(nameof(SelectedRankName));
            }
        }

        private WorkRank newRankElement;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public RanksWindow()
        {
            InitializeComponent();

            LoadRanks();

            DataContext = this;
        }

        private void LoadRanks()
        {
            RanksNames = new ObservableCollection<WorkRank>();

            CRUDDataBase.ConnectToDataBase();
            RanksNames = CRUDDataBase.LoadRanks();
            CRUDDataBase.CloseConnection();
        }

        /// <summary>
        /// Открытие окна добавления элемента
        /// </summary>
        /// <param name="newInputName"></param>
        public void ShowAddElementWindow()
        {
            AddElementWindow addElementWindow = new AddElementWindow(WindowsArgumentsTranfer.IsPlaceOfWorksWindow);
            addElementWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            string newInputName;

            if (addElementWindow.ShowDialog() == true)
            {
                newInputName = addElementWindow.NewNameOfElement;
                AddNewRank(newInputName);
            }
            else
            {
                newInputName = string.Empty;
                //MessageBox.Show("Все изменения в этом окне будут сброшены");
            }
        }

        /// <summary>
        /// Открытие окна изменения элемента
        /// </summary>
        /// <param name="newInputName"></param>
        public void ShowEditElementWindow(string selectedElement)
        {
            EditElementWindow editElementWindow = new EditElementWindow(selectedElement, WindowsArgumentsTranfer.IsPlaceOfWorksWindow);
            editElementWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            string newInputName;

            if (editElementWindow.ShowDialog() == true)
            {
                newInputName = editElementWindow.NewNameOfElement;
                EditRank(newInputName);

                MessageBox.Show("Информация сохранена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                newInputName = string.Empty;
                //MessageBox.Show("Все изменения в этом окне будут сброшены");
            }
        }

        /// <summary>
        /// Нажатие на кнопку добавления нового элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowAddElementWindow();
        }

        /// <summary>
        /// Нажатие на кнопку изменения элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (SelectedRankName != null)
            {
                ShowEditElementWindow(SelectedRankName.Title);
            }
            else
            {
                MessageBox.Show("Для редактирования необходимо выбрать нужный элемент");
            }
        }

        private void AddNewRank(string newRankName)
        {
            if (newRankName != null)
            {
                CRUDDataBase.ConnectToDataBase();

                if (CRUDDataBase.IsRankAlreadyExists(newRankName))
                {
                    MessageBoxResult sure = MessageBox.Show("Звание с такими названием уже существует.\nВсё равно добавить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    switch (sure)
                    {
                        case MessageBoxResult.Yes:
                            // Добавление в бд
                            newRankElement = CRUDDataBase.AddRank(newRankName);
                            // Добавление в список
                            RanksNames.Add(newRankElement);
                            MessageBox.Show("Звание добавлено.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                else
                {
                    // Добавление в бд
                    newRankElement = CRUDDataBase.AddRank(newRankName);
                    // Добавление в список
                    RanksNames.Add(newRankElement);
                    MessageBox.Show("Звание добавлено.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CRUDDataBase.CloseConnection();
            }
        }

        private void EditRank(string newNamePlaceOfWork)
        {
            // Изменение в списке
            SelectedRankName.Title = newNamePlaceOfWork;

            // Изменение в бд
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.EditRank(SelectedRankName);
            CRUDDataBase.CloseConnection();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRankName != null)
            {
                // Удаление из бд
                CRUDDataBase.ConnectToDataBase();
                CRUDDataBase.DeleteRank(SelectedRankName);
                CRUDDataBase.CloseConnection();

                // Удаление из списка
                RanksNames.Remove(SelectedRankName);
            }
            else
            {
                MessageBox.Show("Для удаления необходимо выбрать нужный элемент");
            }
        }
    }
}
