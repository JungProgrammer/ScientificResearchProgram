using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using ResearchProgram.Classes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

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

        public List<Job> jobsList { get; set; }

        private bool _isEditPerson = false;
        private int _editedPersonId;
        private string _personFIO;

        public List<WorkCategories> WorkCategoriesList { get; set; }
        public List<WorkDegree> WorkDegreesList { get; set; }
        public List<WorkRank> WorkRanksList { get; set; }

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

        public createPersonWindow(DataTable personsDataTable, Person personToEdit = null)
        {
            InitializeComponent();

            peopleDataTable = personsDataTable;

            CRUDDataBase.ConnectToDataBase();
            jobsList = CRUDDataBase.GetJobs();
            WorkCategoriesList = CRUDDataBase.GetWorkCategories();
            WorkDegreesList = CRUDDataBase.GetWorkDegrees();
            WorkRanksList = CRUDDataBase.GetWorkRanks();

            CRUDDataBase.CloseConnection();

            if (personToEdit != null)
            {
                DeletePersonButton.Visibility = Visibility.Visible;
                _isEditPerson = true;
                _editedPersonId = personToEdit.Id;
                _personFIO = personToEdit.FIO;
                Title = "Изменение информации о человеке";
                createPersonButton.Content = "Сохранить";
                FIOTextBox.Text = personToEdit.FIO;

                for (int i = 0; i < WorkDegreesList.Count; i++)
                {
                    if (personToEdit.Degree.Id == WorkDegreesList[i].Id)
                        degreeComboBox.SelectedIndex = i;
                }

                for (int i = 0; i < WorkRanksList.Count; i++)
                {
                    if (personToEdit.Rank.Id == WorkRanksList[i].Id)
                        rankComboBox.SelectedIndex = i;
                }

                BirthDateDatePicker.SelectedDate = personToEdit.BitrhDate;
                if (personToEdit.Sex)
                    sexMan.IsChecked = true;
                else
                    sexWoman.IsChecked = true;

                foreach (PersonWorkPalce workPlace in personToEdit.workPlaces)
                {
                    workPlaceListView.Items.Add(workPlace.getPersonGrid());
                    workPlace.isMainWorkPlace.Checked += IsMainWorkPlace_Checked;
                }
            }

            DataContext = this;
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

            // Булевская переменная, которая отвечает за правильное создание договора. Если все необходимые данные были внесены, то договор создается
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
                foreach (Grid grid in workPlaceListView.Items.OfType<Grid>())
                {
                    // AAAAAAAAAAAAAAAAAAAA TODO
                    ComboBox workPlaceComboBox = (ComboBox)grid.Children[1];
                    ComboBox workCategoryComboBox = (ComboBox)grid.Children[3];
                    Grid jobGrid = (Grid)grid.Children[4];
                    ListView jobListView = (ListView)jobGrid.Children[3];


                    PersonWorkPalce workPlace = new PersonWorkPalce();
                    if (workPlaceComboBox.SelectedItem != null)
                    {
                        //workPlace.placeOfWork = (WorkPlace)workPlaceComboBox.SelectedItem;
                        //if (workCategoryComboBox.SelectedItem != null)
                        //{
                        //    workPlace.workCategory = (WorkCategories)workCategoryComboBox.SelectedItem;
                        //}
                        //else
                        //{
                        //    workPlace.workCategory = new WorkCategories();
                        //}

                        workPlace.jobList = new List<Job>();
                        foreach (Grid grid1 in jobListView.Items.OfType<Grid>())
                        {
                            ComboBox jobComboBox = (ComboBox)grid1.Children[0];
                            TextBox salaryRateTextBox = (TextBox)grid1.Children[2];
                            if (jobComboBox.SelectedItem != null)
                            {
                                Job job = (Job)jobComboBox.SelectedItem;
                                if (salaryRateTextBox.Text != "")
                                {
                                    job.SalaryRate = Convert.ToInt32(salaryRateTextBox.Text);
                                }
                                else
                                {
                                    job.SalaryRate = 0;
                                }
                                Console.WriteLine(job.Id);
                                Console.WriteLine(job.Salary);
                                Console.WriteLine(job.SalaryRate);

                                workPlace.jobList.Add(job);
                            }
                        }
                    }
                    newPerson.workPlaces.Add(workPlace);
                }
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
                    CRUDDataBase.InsertNewPersonToDB(newPerson);
                    //((MainWindow)Owner).LoadPersonsList();
                    MessageBox.Show("Информация о человеке успешно внесена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                ((MainWindow)Owner).PersonsUpdateButton_Click(sender, e);
                Close();


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
            PersonWorkPalce personWorkPalce = new PersonWorkPalce();

            workPlaceListView.Items.Add(personWorkPalce.getPersonGrid());
            personWorkPalce.isMainWorkPlace.Checked += IsMainWorkPlace_Checked;

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

    }
}
