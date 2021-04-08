using ResearchProgram.Classes;
using ResearchProgram.Forms.HelpWindows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для DegreesWindow.xaml
    /// </summary>
    public partial class DegreesWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<WorkDegree> DegreesNames { get; set; }

        private WorkDegree _selectedDegreeName;
        public WorkDegree SelectedDegreeName
        {
            get => _selectedDegreeName;
            set
            {
                _selectedDegreeName = value;
                OnPropertyChanged(nameof(SelectedDegreeName));
            }
        }

        private WorkDegree newDegreeElement;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public DegreesWindow()
        {
            InitializeComponent();

            LoadDegrees();

            DataContext = this;
        }

        private void LoadDegrees()
        {
            DegreesNames = new ObservableCollection<WorkDegree>();

            CRUDDataBase.ConnectToDataBase();
            DegreesNames = CRUDDataBase.LoadDegrees();
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
                AddNewDegree(newInputName);
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
                EditDegree(newInputName);

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
            if (SelectedDegreeName != null)
            {
                ShowEditElementWindow(SelectedDegreeName.Title);
            }
            else
            {
                MessageBox.Show("Для редактирования необходимо выбрать нужный элемент");
            }
        }

        private void AddNewDegree(string newDegreeName)
        {
            if (newDegreeName != null)
            {
                CRUDDataBase.ConnectToDataBase();
                if (CRUDDataBase.IsDegreeAlreadyExists(newDegreeName))
                {
                    MessageBoxResult sure = MessageBox.Show("Степень с такими названием уже существует.\nВсё равно добавить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    switch (sure)
                    {
                        case MessageBoxResult.Yes:
                            // Добавление в бд
                            newDegreeElement = CRUDDataBase.AddDegree(newDegreeName);
                            // Добавление в список
                            DegreesNames.Add(newDegreeElement);
                            MessageBox.Show("Степень добавлена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                else
                {
                    // Добавление в бд
                    newDegreeElement = CRUDDataBase.AddDegree(newDegreeName);
                    // Добавление в список
                    DegreesNames.Add(newDegreeElement);
                    MessageBox.Show("Степень добавлена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                CRUDDataBase.CloseConnection();
            }
        }

        private void EditDegree(string newNamePlaceOfWork)
        {
            // Изменение в списке
            SelectedDegreeName.Title = newNamePlaceOfWork;

            // Изменение в бд
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.EditDegree(SelectedDegreeName);
            CRUDDataBase.CloseConnection();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDegreeName != null)
            {
                // Удаление из бд
                CRUDDataBase.ConnectToDataBase();
                CRUDDataBase.DeleteDegree(SelectedDegreeName);
                CRUDDataBase.CloseConnection();

                // Удаление из списка
                DegreesNames.Remove(SelectedDegreeName);
            }
            else
            {
                MessageBox.Show("Для удаления необходимо выбрать нужный элемент");
            }
        }
    }
}
