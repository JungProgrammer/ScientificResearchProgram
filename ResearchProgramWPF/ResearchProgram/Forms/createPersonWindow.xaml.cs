using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createPersonWindow.xaml
    /// </summary>
    public partial class createPersonWindow : Window, INotifyPropertyChanged
    {
        // DataTable для таблицы людей на главной форме
        private DataTable peopleDataTable;

        public string sexChecked;

        //public List<Job> JobsList { get; set; }
        //public List<WorkCategories> WorkCategoriesList { get; set; }

        private List<WorkDegree> _workDegreesList;
        public List<WorkDegree> WorkDegreesList
        {
            get
            {
                return _workDegreesList;
            }
            set
            {
                _workDegreesList = value;
                NotifyPropertyChanged(nameof(WorkDegreesList));
            }
        }

        private WorkDegree _workDegreeSelectedItem;
        public WorkDegree WorkDegreeSelectedItem
        {
            get
            {
                return _workDegreeSelectedItem;
            }
            set
            {
                if(value != null) _workDegreeSelectedItem = value;
                NotifyPropertyChanged(nameof(WorkDegreeSelectedItem));
            }
        }


        private List<WorkRank> _workRanksList;
        public List<WorkRank> WorkRanksList
        {
            get
            {
                return _workRanksList;
            }
            set
            {
                _workRanksList = value;
                NotifyPropertyChanged(nameof(WorkRanksList));
            }
        }

        private WorkRank _workRankSelectedItem;
        public WorkRank WorkRankSelectedItem
        {
            get
            {
                return _workRankSelectedItem;
            }
            set
            {
                if(value != null) _workRankSelectedItem = value;
                NotifyPropertyChanged(nameof(WorkRankSelectedItem));
            }
        }


        private bool _isEditPerson = false;
        private int _editedPersonId;
        private string _personFIO;

        /// <summary>
        /// Выбранный пол человека
        /// </summary>
        private bool _sexSelected = true;

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private Person _personToEdit;

        public createPersonWindow(DataTable personsDataTable, Person personToEdit = null)
        {
            InitializeComponent();


            FormsManager.CreatePersonWindow = this;


            peopleDataTable = personsDataTable;

            _personToEdit = personToEdit;

            LoadDataAsync();


            DataContext = this;
        }


        public async void UpdateDataAsync()
        {
            await Task.Run(() => UpdateData());
        }


        private void UpdateData()
        {
            CRUDDataBase.ConnectToDataBase();
            Dispatcher.Invoke(() => WorkDegreesList = CRUDDataBase.GetWorkDegrees());
            Dispatcher.Invoke(() => WorkRanksList = CRUDDataBase.GetWorkRanks());
            CRUDDataBase.CloseConnection();

            List<PersonWorkPlace> copiedPersonWorkPlaces = null;
            Dispatcher.Invoke(() => copiedPersonWorkPlaces = PersonWorkPlace.ConvertListViewToWorkPlaceList(workPlaceListView));


            int itemsCount = workPlaceListView.Items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                Dispatcher.Invoke(() => workPlaceListView.Items.RemoveAt(0));
            }


            foreach (PersonWorkPlace personWorkPlace in copiedPersonWorkPlaces)
            {
                Dispatcher.Invoke(() => workPlaceListView.Items.Add(personWorkPlace.getPersonGrid()));
                Dispatcher.Invoke(() => personWorkPlace.isMainWorkPlace.Checked += IsMainWorkPlace_Checked);
            }


            for (int i = 0; i < WorkDegreesList.Count; i++)
            {
                if (_workDegreeSelectedItem != null)
                {
                    if (_workDegreeSelectedItem.Id == WorkDegreesList[i].Id)
                        Dispatcher.Invoke(() => WorkDegreeSelectedItem = WorkDegreesList[i]);
                }
            }

            for (int i = 0; i < WorkRanksList.Count; i++)
            {
                if (_workRankSelectedItem != null)
                {
                    if (_workRankSelectedItem.Id == WorkRanksList[i].Id)
                        Dispatcher.Invoke(() => WorkRankSelectedItem = WorkRanksList[i]);
                }
            }
        }

        
        /// <summary>
        /// Изначальная загрузка данных в форму
        /// </summary>
        public async void LoadDataAsync()
        {
            await Task.Run(() => LoadData());
        }


        /// <summary>
        /// Загрузка данных в списки, на которые ссылаются комбобоксы
        /// </summary>
        private void LoadData()
        {
            CRUDDataBase.ConnectToDataBase();
            //JobsList = CRUDDataBase.GetJobs();
            //WorkCategoriesList = CRUDDataBase.GetWorkCategories();
            Dispatcher.Invoke(() => WorkDegreesList = CRUDDataBase.GetWorkDegrees());
            Dispatcher.Invoke(() => WorkRanksList = CRUDDataBase.GetWorkRanks());
            CRUDDataBase.CloseConnection();


            if (_personToEdit != null)
            {
                Dispatcher.Invoke(() => DeletePersonButton.Visibility = Visibility.Visible);

                _isEditPerson = true;
                _editedPersonId = _personToEdit.Id;
                _personFIO = _personToEdit.FIO;
                Dispatcher.Invoke(() => Title = "Изменение информации о человеке");

                Dispatcher.Invoke(() => createPersonButton.Content = "Сохранить");
                Dispatcher.Invoke(() => FIOTextBox.Text = _personToEdit.FIO);

                for (int i = 0; i < WorkDegreesList.Count; i++)
                {
                    if (_personToEdit.Degree.Id == WorkDegreesList[i].Id)
                        Dispatcher.Invoke(() => WorkDegreeSelectedItem = WorkDegreesList[i]);
                }

                for (int i = 0; i < WorkRanksList.Count; i++)
                {
                    if (_personToEdit.Rank.Id == WorkRanksList[i].Id)
                        Dispatcher.Invoke(() => WorkRankSelectedItem = WorkRanksList[i]);
                }

                Dispatcher.Invoke(() => BirthDateDatePicker.SelectedDate = _personToEdit.BitrhDate);
                if (_personToEdit.Sex)
                    Dispatcher.Invoke(() => sexMan.IsChecked = true);
                else
                    Dispatcher.Invoke(() => sexWoman.IsChecked = true);

                foreach (PersonWorkPlace workPlace in _personToEdit.workPlaces)
                {
                    Dispatcher.Invoke(() => workPlaceListView.Items.Add(workPlace.getPersonGrid()));
                    Dispatcher.Invoke(() => workPlace.isMainWorkPlace.Checked += IsMainWorkPlace_Checked);
                }
            }
        }


        /// <summary>
        /// Запоминание выбора пола человека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (((RadioButton)sender).Content.ToString() == "Мужчина")
            {
                _sexSelected = true;
            }
            else
            {
                _sexSelected = false;
            }
        }


        /// <summary>
        /// Добавление человека в бд и строки в таблицу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createPersonButtonClick(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            
            bool isAllOkey = true;
            string incorrectDataString = "";

            if (FIOTextBox.Text != "")
            {
                newPerson.FIO = FIOTextBox.Text;
            }
            else
            {
                isAllOkey = false;
                incorrectDataString += "Необходимо ввести ФИО\n";
            }

            if (BirthDateDatePicker.SelectedDate != null)
            {
                newPerson.BitrhDate = (DateTime)BirthDateDatePicker.SelectedDate;
            }

            newPerson.Sex = _sexSelected;

            if (degreeComboBox.SelectedItem != null)
            {
                newPerson.Degree = (WorkDegree)degreeComboBox.SelectedItem;
            }

            if (rankComboBox.SelectedItem != null)
            {
                newPerson.Rank = (WorkRank)rankComboBox.SelectedItem;
            }

            if (workPlaceListView.Items != null)
            {
                newPerson.workPlaces = PersonWorkPlace.ConvertListViewToWorkPlaceList(workPlaceListView);
            }
            if (isAllOkey)
            {
                CRUDDataBase.ConnectToDataBase();

                if (_isEditPerson)
                {
                    newPerson.Id = _editedPersonId;
                    CRUDDataBase.UpdateFIO(newPerson);
                    CRUDDataBase.UpdateBirthDate(newPerson);
                    CRUDDataBase.UpdateSex(newPerson);
                    CRUDDataBase.UpdatePlaceOfWork(newPerson);
                    CRUDDataBase.UpdateDegree(newPerson);
                    CRUDDataBase.UpdateRank(newPerson);
                    MessageBox.Show("Информация о человеке успешно изменена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Внесение нового человека в бд
                    if (CRUDDataBase.IsPersonAlreadyExists(newPerson))
                    {
                        MessageBoxResult sure = MessageBox.Show("Человек с такими именем и датой рождения уже существует.\nВсё равно добавить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        switch (sure)
                        {
                            case MessageBoxResult.Yes:
                                newPerson = CRUDDataBase.InsertNewPersonToDB(newPerson);
                                MessageBox.Show("Информация о человеке успешно внесена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                                ((MainWindow)Owner).PersonsUpdateButton_Click(sender, e);
                                Close();
                                break;
                            case MessageBoxResult.No:
                                break;
                        }
                    }
                    else
                    {
                        newPerson = CRUDDataBase.InsertNewPersonToDB(newPerson);
                        MessageBox.Show("Информация о человеке успешно внесена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        ((MainWindow)Owner).PersonsUpdateButton_Click(sender, e);
                        Close();
                    }
                }

                CRUDDataBase.CloseConnection();
            }
            else
            {
                MessageBox.Show(incorrectDataString, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeletePersonButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult sure = MessageBox.Show("Удалить человека с именем " + _personFIO + "?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            switch (sure)
            {
                case MessageBoxResult.Yes:
                    CRUDDataBase.DeletePerson(_editedPersonId);
                    MessageBox.Show("Удаление успешно", "Удаление человека", MessageBoxButton.OK, MessageBoxImage.Information);
                    ((MainWindow)Owner).PersonsUpdateButton_Click(sender, e);
                    Close();
                    break;
            }
        }

        private void workPlaceAddButton_Click(object sender, RoutedEventArgs e)
        {
            PersonWorkPlace personWorkPlace = new PersonWorkPlace();

            workPlaceListView.Items.Add(personWorkPlace.getPersonGrid());
            personWorkPlace.isMainWorkPlace.Checked += IsMainWorkPlace_Checked;

        }




        private void IsMainWorkPlace_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox CheckedCheckBox = (CheckBox)sender;
            if (workPlaceListView.Items != null)
            {
                foreach (Grid grid in workPlaceListView.Items.OfType<Grid>())
                {
                    CheckBox checkBox = (CheckBox)grid.Children[2];

                    if (checkBox != CheckedCheckBox)
                    {
                        checkBox.IsChecked = false;
                    }
                }
            }
        }

        private void workPlaceDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = workPlaceListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    workPlaceListView.Items.Remove(workPlaceListView.SelectedItems[0]);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            FormsManager.CreatePersonWindow = null;
        }
    }
}
