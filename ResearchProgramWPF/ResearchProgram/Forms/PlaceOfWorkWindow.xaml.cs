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
using ResearchProgram.Classes;
using ResearchProgram.Forms.HelpWindows;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для PlaceOfWork.xaml
    /// </summary>
    public partial class PlaceOfWorkWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<PlaceOfWork> PlaceWorkNames { get; set; }

        private PlaceOfWork _selectedPlaceOfWork;
        public PlaceOfWork SelectedPlaceOfWork
        {
            get => _selectedPlaceOfWork;
            set
            {
                _selectedPlaceOfWork = value;
                OnPropertyChanged(nameof(SelectedPlaceOfWork));
            }
        }

        private PlaceOfWork newPlaceWorkElement;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public PlaceOfWorkWindow()
        {
            InitializeComponent();

            LoadPlaceWorkNames();

            DataContext = this;
        }

        private void LoadPlaceWorkNames()
        {
            PlaceWorkNames = new ObservableCollection<PlaceOfWork>();

            CRUDDataBase.ConnectToDataBase();
            PlaceWorkNames = CRUDDataBase.LoadPlacesOfWorks();
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
                AddNewPlaceOfWork(newInputName);

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
            EditElementWindow editElementWindow = new EditElementWindow(selectedElement);
            editElementWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            string newInputName;

            if (editElementWindow.ShowDialog() == true)
            {
                newInputName = editElementWindow.NewNameOfElement;
                EditPlaceOfWork(newInputName);

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
            if (SelectedPlaceOfWork != null)
            {
                ShowEditElementWindow(SelectedPlaceOfWork.Title);
            }
            else
            {
                MessageBox.Show("Для редактирования необходимо выбрать нужный элемент");
            }
        }

        private void AddNewPlaceOfWork(string newNamePlaceOfWork)
        {
            if(newNamePlaceOfWork != null)
            {
                // Добавление в бд
                CRUDDataBase.ConnectToDataBase();
                newPlaceWorkElement = CRUDDataBase.AddPlaceOfWork(newNamePlaceOfWork);
                CRUDDataBase.CloseConnection();

                // Добавление в список
                PlaceWorkNames.Add(newPlaceWorkElement);
            }
        }

        private void EditPlaceOfWork(string newNamePlaceOfWork)
        {
            // Изменение в списке
            SelectedPlaceOfWork.Title = newNamePlaceOfWork;

            // Изменение в бд
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.EditPlaceOfWork(SelectedPlaceOfWork);
            CRUDDataBase.CloseConnection();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlaceOfWork != null)
            {
                // Удаление из бд
                CRUDDataBase.ConnectToDataBase();
                CRUDDataBase.DeletePlaceOfWork(SelectedPlaceOfWork);
                CRUDDataBase.CloseConnection();

                // Удаление из списка
                PlaceWorkNames.Remove(SelectedPlaceOfWork);
            }
            else
            {
                MessageBox.Show("Для удаления необходимо выбрать нужный элемент");
            }
        }
    }
}
