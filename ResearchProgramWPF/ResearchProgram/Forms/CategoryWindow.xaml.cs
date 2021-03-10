using ResearchProgram.Classes;
using ResearchProgram.Forms.HelpWindows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window, INotifyPropertyChanged
    {

        public ObservableCollection<WorkCategories> CategoryNames { get; set; }

        private WorkCategories _selectedCategoryName;
        public WorkCategories SelectedCategoryName
        {
            get => _selectedCategoryName;
            set
            {
                _selectedCategoryName = value;
                OnPropertyChanged(nameof(SelectedCategoryName));
            }
        }

        private WorkCategories newCategoryElement;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public CategoryWindow()
        {
            InitializeComponent();

            LoadCategories();

            DataContext = this;
        }

        private void LoadCategories()
        {
            CategoryNames = new ObservableCollection<WorkCategories>();

            CRUDDataBase.ConnectToDataBase();
            CategoryNames = CRUDDataBase.LoadCategories();
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
                AddNewCategory(newInputName);

                MessageBox.Show("Информация сохранена");
            }
            else
            {
                newInputName = string.Empty;
                MessageBox.Show("Все изменения в этом окне будут сброшены");
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
                EditCategory(newInputName);

                MessageBox.Show("Информация сохранена");
            }
            else
            {
                newInputName = string.Empty;
                MessageBox.Show("Все изменения в этом окне будут сброшены");
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
            if (SelectedCategoryName != null)
            {
                ShowEditElementWindow(SelectedCategoryName.Title);
            }
            else
            {
                MessageBox.Show("Для редактирования необходимо выбрать нужный элемент");
            }
        }

        private void AddNewCategory(string newCategoryName)
        {
            if (newCategoryName != null)
            {
                // Добавление в бд
                CRUDDataBase.ConnectToDataBase();
                newCategoryElement = CRUDDataBase.AddCategory(newCategoryName);
                CRUDDataBase.CloseConnection();

                // Добавление в список
                CategoryNames.Add(newCategoryElement);
            }
        }

        private void EditCategory(string newNamePlaceOfWork)
        {
            // Изменение в списке
            SelectedCategoryName.Title = newNamePlaceOfWork;

            // Изменение в бд
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.EditCategory(SelectedCategoryName);
            CRUDDataBase.CloseConnection();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCategoryName != null)
            {
                // Удаление из бд
                CRUDDataBase.ConnectToDataBase();
                CRUDDataBase.DeleteCategory(SelectedCategoryName);
                CRUDDataBase.CloseConnection();

                // Удаление из списка
                CategoryNames.Remove(SelectedCategoryName);
            }
            else
            {
                MessageBox.Show("Для удаления необходимо выбрать нужный элемент");
            }

        }
    }
}
