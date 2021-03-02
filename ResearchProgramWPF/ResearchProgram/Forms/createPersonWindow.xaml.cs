using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using ResearchProgram.Classes;
using System.ComponentModel;
 
namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createPersonWindow.xaml
    /// </summary>
    public partial class createPersonWindow : Window
    {
        // DataTable для таблицы людей на главной форме
        private DataTable peopleDataTable;

        public string sexChecked;

        public List<Job> jobsList { get; set; }

        private bool _isEditPerson = false;
        private int _editedPersonId;
        private string _personFIO;

        public List<PlaceOfWork> PlaceOfWorkList { get; set; }
        public List<WorkCategories> WorkCategoriesList { get; set; }
        public List<WorkDegree> WorkDegreesList { get; set; }
        public List<WorkRank> WorkRanksList { get; set; }


        /// <summary>
        /// Выбранный пол человека
        /// </summary>
        private bool _sexSelected = true;

        public createPersonWindow(DataTable personsDataTable, Person personToEdit = null)
        {
            InitializeComponent();

            peopleDataTable = personsDataTable;

            CRUDDataBase.ConnectToDataBase();
            jobsList = CRUDDataBase.GetJobs();
            PlaceOfWorkList = CRUDDataBase.GetPlacesOfWorks();
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

                for(int i = 0;i < WorkDegreesList.Count; i++)
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

                foreach(Person.WorkPlace workPlace in personToEdit.workPlaces)
                {
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

                    Label workPlaceLabel = new Label
                    {
                        Content = "Место работы"
                    };
                    grid.Children.Add(workPlaceLabel);
                    Grid.SetRow(workPlaceLabel, 0);
                    Grid.SetColumn(workPlaceLabel, 0);

                    ComboBox workPlaceComboBox = new ComboBox
                    {
                        Margin = new Thickness(5, 0, 5, 0),
                        Padding = new Thickness(5, 5, 5, 5),
                        Height = 25,
                        ItemsSource = PlaceOfWorkList
                    };
                    grid.Children.Add(workPlaceComboBox);
                    Grid.SetRow(workPlaceComboBox, 1);
                    Grid.SetColumn(workPlaceComboBox, 0);
                    for(int i =0;i< PlaceOfWorkList.Count; i++)
                    {
                        if (workPlace.placeOfWork.Id == PlaceOfWorkList[i].Id)
                            workPlaceComboBox.SelectedIndex = i;
                    }

                    Label CategoryLabel = new Label
                    {
                        Content = "Категория"
                    };
                    grid.Children.Add(CategoryLabel);
                    Grid.SetRow(CategoryLabel, 2);
                    Grid.SetColumn(CategoryLabel, 0);

                    ComboBox categoryComboBox = new ComboBox
                    {
                        Margin = new Thickness(5, 0, 5, 0),
                        Padding = new Thickness(5, 5, 5, 5),
                        Height = 25,
                        ItemsSource = WorkCategoriesList,
                        SelectedItem = workPlace.workCategory
                    };
                    grid.Children.Add(categoryComboBox);
                    Grid.SetRow(categoryComboBox, 3);
                    Grid.SetColumn(categoryComboBox, 0);
                    for (int i = 0; i < WorkCategoriesList.Count; i++)
                    {
                        if (workPlace.workCategory.Id == WorkCategoriesList[i].Id)
                            categoryComboBox.SelectedIndex = i;
                    }

                    Grid jobGrid = new Grid();
                    jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                    jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
                    jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                    grid.Children.Add(jobGrid);
                    Grid.SetRow(jobGrid, 0);
                    Grid.SetColumn(jobGrid, 1);
                    Grid.SetColumnSpan(jobGrid, 2);
                    Grid.SetRowSpan(jobGrid, 4);

                    Label jobLabel = new Label
                    {
                        Content = "Должность"
                    };
                    jobGrid.Children.Add(jobLabel);
                    Label rateLabel = new Label
                    {
                        Content = "Ставка"
                    };
                    jobGrid.Children.Add(rateLabel);

                    Grid.SetRow(rateLabel, 0);
                    Grid.SetColumn(rateLabel, 1);
                    Label salaryLabel = new Label
                    {
                        Content = "Оклад"
                    };
                    jobGrid.Children.Add(salaryLabel);
                    Grid.SetRow(salaryLabel, 0);
                    Grid.SetColumn(salaryLabel, 2);

                    ListView jobListView = new ListView();
                    jobGrid.Children.Add(jobListView);
                    Grid.SetRow(jobListView, 1);
                    Grid.SetColumn(jobListView, 0);
                    Grid.SetColumnSpan(jobListView, 3);


                    Button addJobButton = new Button
                    {
                        Content = "Добавить"

                    };
                    addJobButton.Click += addJobButton_click;
                    jobGrid.Children.Add(addJobButton);
                    Grid.SetRow(addJobButton, 2);
                    Grid.SetColumn(addJobButton, 0);

                    Button deleteJobButton = new Button
                    {
                        Content = "Удалить"
                    };
                    deleteJobButton.Click += deleteJobButton_click;
                    jobGrid.Children.Add(deleteJobButton);
                    Grid.SetRow(deleteJobButton, 2);
                    Grid.SetColumn(deleteJobButton, 1);


                    foreach(Job job in workPlace.jobList)
                    {
                        Grid grid2 = new Grid();
                        grid2.ColumnDefinitions.Add(new ColumnDefinition());
                        grid2.ColumnDefinitions.Add(new ColumnDefinition());
                        grid2.ColumnDefinitions.Add(new ColumnDefinition());

                        TextBox salaryTextBox = new TextBox
                        {
                            IsEnabled = false,
                            Width = 80,
                            Margin = new Thickness(10, 3, 5, 3),
                            Text = job.Salary.ToString()
                        };

                        ComboBox jobComboBox = new ComboBox
                        {
                            ItemsSource = jobsList,
                            Margin = new Thickness(0, 5, 0, 0),
                            Height = 25,
                            Width = 100,

                        };
                        jobComboBox.SelectionChanged += jobComboBoxSelectionChanged_event;
                        grid2.Children.Add(jobComboBox);
                        Grid.SetColumn(jobComboBox, 0);
                        for (int i = 0; i < jobsList.Count; i++)
                        {
                            if (job.Id == jobsList[i].Id)
                                jobComboBox.SelectedIndex = i;
                        }

                        grid2.Children.Add(salaryTextBox);
                        Grid.SetColumn(salaryTextBox, 1);

                        TextBox rateTextBox = new TextBox
                        {
                            Width = 80,
                            Margin = new Thickness(30, 3, 5, 3),
                            Text = job.SalaryRate.ToString()
                        };
                        rateTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
                        grid2.Children.Add(rateTextBox);
                        Grid.SetColumn(rateTextBox, 2);

                        jobListView.Items.Add(grid2);

                        void jobComboBoxSelectionChanged_event(object sender3, SelectionChangedEventArgs e3)
                        {
                            salaryTextBox.Text = ((Job)jobComboBox.SelectedItem).Salary.ToString();
                        }

                    }

                    workPlaceListView.Items.Add(grid);

                    void addJobButton_click(object sender2, RoutedEventArgs e2)
                    {
                        Grid grid2 = new Grid();
                        grid2.ColumnDefinitions.Add(new ColumnDefinition());
                        grid2.ColumnDefinitions.Add(new ColumnDefinition());
                        grid2.ColumnDefinitions.Add(new ColumnDefinition());

                        TextBox salaryTextBox = new TextBox
                        {
                            IsEnabled = false,
                            Width = 80,
                            Margin = new Thickness(10, 3, 5, 3),
                        };

                        ComboBox jobComboBox = new ComboBox
                        {
                            ItemsSource = jobsList,
                            Margin = new Thickness(0, 5, 0, 0),
                            Height = 25,
                            Width = 100,

                        };
                        jobComboBox.SelectionChanged += jobComboBoxSelectionChanged_event;
                        grid2.Children.Add(jobComboBox);
                        Grid.SetColumn(jobComboBox, 0);

                        grid2.Children.Add(salaryTextBox);
                        Grid.SetColumn(salaryTextBox, 1);

                        TextBox rateTextBox = new TextBox
                        {
                            Width = 80,
                            Margin = new Thickness(30, 3, 5, 3),
                        };
                        rateTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
                        grid2.Children.Add(rateTextBox);
                        Grid.SetColumn(rateTextBox, 2);

                        jobListView.Items.Add(grid2);

                        void jobComboBoxSelectionChanged_event(object sender3, SelectionChangedEventArgs e3)
                        {
                            salaryTextBox.Text = ((Job)jobComboBox.SelectedItem).Salary.ToString();
                        }
                    }

                    void deleteJobButton_click(object sender2, RoutedEventArgs e2)
                    {
                        int countSelectedElement = jobListView.SelectedItems.Count;
                        if (countSelectedElement > 0)
                        {
                            for (int i = 0; i < countSelectedElement; i++)
                            {
                                jobListView.Items.Remove(jobListView.SelectedItems[0]);
                            }
                        }
                    }

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
                    ComboBox workPlaceComboBox = (ComboBox)grid.Children[1];
                    ComboBox workCategoryComboBox = (ComboBox)grid.Children[3];
                    Grid jobGrid = (Grid)grid.Children[4];
                    ListView jobListView = (ListView)jobGrid.Children[3];


                    Person.WorkPlace workPlace = new Person.WorkPlace();
                    if (workPlaceComboBox.SelectedItem != null)
                    {
                        workPlace.placeOfWork = (PlaceOfWork)workPlaceComboBox.SelectedItem;
                        if (workCategoryComboBox.SelectedItem != null)
                        {
                            workPlace.workCategory = (WorkCategories)workCategoryComboBox.SelectedItem;
                        }
                        else
                        {
                            workPlace.workCategory = new WorkCategories();
                        }

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
                    Close();
                }
                else
                {
                    // Внесение нового человека в бд
                    CRUDDataBase.InsertNewPersonToDB(newPerson);

                    MessageBox.Show("Информация о человеке успешно внесена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
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
                    Close();
                    break;
            }
        }

        private void workPlaceAddButton_Click(object sender, RoutedEventArgs e)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

            Label workPlaceLabel = new Label
            {
                Content = "Место работы"
            };
            grid.Children.Add(workPlaceLabel);
            Grid.SetRow(workPlaceLabel, 0);
            Grid.SetColumn(workPlaceLabel, 0);

            ComboBox workPlaceComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Padding = new Thickness(5, 5, 5, 5),
                Height = 25,
                ItemsSource = PlaceOfWorkList,
            };
            grid.Children.Add(workPlaceComboBox);
            Grid.SetRow(workPlaceComboBox, 1);
            Grid.SetColumn(workPlaceComboBox, 0);

            Label CategoryLabel = new Label
            {
                Content = "Категория"
            };
            grid.Children.Add(CategoryLabel);
            Grid.SetRow(CategoryLabel, 2);
            Grid.SetColumn(CategoryLabel, 0);

            ComboBox categoryComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Padding = new Thickness(5, 5, 5, 5),
                Height = 25,
                ItemsSource = WorkCategoriesList,
            };
            grid.Children.Add(categoryComboBox);
            Grid.SetRow(categoryComboBox, 3);
            Grid.SetColumn(categoryComboBox, 0);

            Grid jobGrid = new Grid();
            jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
            jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
            jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
            jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
            jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
            jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
            grid.Children.Add(jobGrid);
            Grid.SetRow(jobGrid, 0);
            Grid.SetColumn(jobGrid, 1);
            Grid.SetColumnSpan(jobGrid, 2);
            Grid.SetRowSpan(jobGrid, 4);

            Label jobLabel = new Label
            {
                Content = "Должность"
            };
            jobGrid.Children.Add(jobLabel);
            Label rateLabel = new Label
            {
                Content = "Ставка"
            };
            jobGrid.Children.Add(rateLabel);

            Grid.SetRow(rateLabel, 0);
            Grid.SetColumn(rateLabel, 1);
            Label salaryLabel = new Label
            {
                Content = "Оклад"
            };
            jobGrid.Children.Add(salaryLabel);
            Grid.SetRow(salaryLabel, 0);
            Grid.SetColumn(salaryLabel, 2);

            ListView jobListView = new ListView();
            jobGrid.Children.Add(jobListView);
            Grid.SetRow(jobListView, 1);
            Grid.SetColumn(jobListView, 0);
            Grid.SetColumnSpan(jobListView, 3);


            Button addJobButton = new Button
            {
                Content = "Добавить"

            };
            addJobButton.Click += addJobButton_click;
            jobGrid.Children.Add(addJobButton);
            Grid.SetRow(addJobButton, 2);
            Grid.SetColumn(addJobButton, 0);

            Button deleteJobButton = new Button
            {
                Content = "Удалить"
            };
            deleteJobButton.Click += deleteJobButton_click;
            jobGrid.Children.Add(deleteJobButton);
            Grid.SetRow(deleteJobButton, 2);
            Grid.SetColumn(deleteJobButton, 1);


            workPlaceListView.Items.Add(grid);

            void addJobButton_click(object sender2, RoutedEventArgs e2)
            {
                Grid grid2 = new Grid();
                grid2.ColumnDefinitions.Add(new ColumnDefinition());
                grid2.ColumnDefinitions.Add(new ColumnDefinition());
                grid2.ColumnDefinitions.Add(new ColumnDefinition());

                TextBox salaryTextBox = new TextBox
                {
                    IsEnabled = false,
                    Width = 80,
                    Margin = new Thickness(10, 3, 5, 3),
                };

                ComboBox jobComboBox = new ComboBox
                {
                    ItemsSource = jobsList,
                    Margin = new Thickness(0, 5, 0, 0),
                    Height = 25,
                    Width = 100,
                    
                };
                jobComboBox.SelectionChanged += jobComboBoxSelectionChanged_event;
                grid2.Children.Add(jobComboBox);
                Grid.SetColumn(jobComboBox, 0);

                grid2.Children.Add(salaryTextBox);
                Grid.SetColumn(salaryTextBox, 1);

                TextBox rateTextBox = new TextBox
                {
                    Width = 80,
                    Margin = new Thickness(30, 3, 5, 3),
                };
                rateTextBox.PreviewTextInput += Utilities.TextBoxNumbersPreviewInput;
                grid2.Children.Add(rateTextBox);
                Grid.SetColumn(rateTextBox, 2);

                jobListView.Items.Add(grid2);

                void jobComboBoxSelectionChanged_event(object sender3, SelectionChangedEventArgs e3)
                {
                    salaryTextBox.Text = ((Job)jobComboBox.SelectedItem).Salary.ToString();
                }
            }

            void deleteJobButton_click(object sender2, RoutedEventArgs e2)
            {
                int countSelectedElement = jobListView.SelectedItems.Count;
                if (countSelectedElement > 0)
                {
                    for (int i = 0; i < countSelectedElement; i++)
                    {
                        jobListView.Items.Remove(jobListView.SelectedItems[0]);
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
