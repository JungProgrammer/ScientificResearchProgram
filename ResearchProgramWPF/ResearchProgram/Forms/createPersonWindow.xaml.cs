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
using System.Data;
using DotNetKit.Windows.Controls;
using ResearchProgram.Classes;

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
                DeletePersonButton.Visibility = System.Windows.Visibility.Visible;
                _isEditPerson = true;
                _editedPersonId = personToEdit.Id;
                _personFIO = personToEdit.FIO;
                Title = "Редактирование человека";
                createPersonButton.Content = "Редактировать человека";
                FIOTextBox.Text = personToEdit.FIO;
                //placeOfWorkTextBox.Text = personToEdit.PlaceOfWork;
                //degreeTextBox.Text = personToEdit.Degree;
                //categoryTextBox.Text = personToEdit.Category;
                //rankTextBox.Text = personToEdit.Rank;
                BirthDateDatePicker.SelectedDate = personToEdit.BitrhDate;
                if (personToEdit.Sex)
                    sexMan.IsChecked = true;
                else
                    sexWoman.IsChecked = true;
                for(int i = 0; i< personToEdit.Jobs.Count; i++)
                {
                    StackPanel horizontalStackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                    };


                    AutoCompleteComboBox jobComboBox = new AutoCompleteComboBox()
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        ItemsSource = new List<Job>(jobsList),
                        Width = 130,
                    };
                    for (int j = 0; j < jobsList.Count; j++)
                        if (personToEdit.Jobs[i].Title == jobsList[j].Title)
                            jobComboBox.SelectedIndex = j;

                    TextBlock salaryTextBlock = new TextBlock()
                    {
                        Margin = new Thickness(30, 0, 5, 0),
                        Width = 80,
                        IsEnabled = false,
                        Text = personToEdit.Jobs[i].Salary.ToString()
                    };
                    salaryTextBlock.SetBinding(TextBlock.TextProperty, new Binding() { Source = jobComboBox, Path = new PropertyPath(ComboBox.SelectedValueProperty), TargetNullValue = "", Converter = new JobConverter() });


                    TextBox salaryRateTextBox = new TextBox()
                    {
                        Margin = new Thickness(5, 0, 5, 0),
                        Width = 75,
                        Text = personToEdit.Jobs[i].SalaryRate.ToString()
                    };
                    salaryRateTextBox.PreviewTextInput += TextBoxNumbersPreviewInput;

                    horizontalStackPanel.Children.Add(jobComboBox);
                    horizontalStackPanel.Children.Add(salaryRateTextBox);
                    horizontalStackPanel.Children.Add(salaryTextBlock);
                    //jobsVerticalListView.Items.Add(horizontalStackPanel);
                }
            }

            DataContext = this;
        }

        //Функция для ввода в текст бокс только чисел с одним разделителем
        private void TextBoxNumbersPreviewInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (DS_Count(((TextBox)sender).Text) < 1))));
        }

        // функция подсчета разделителя
        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
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
        /// Добавление StackPanel с элементами для работы. Добавление в jobsVerticalListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jobsAddButton_Click_1(object sender, RoutedEventArgs e)
        {
            StackPanel horizontalStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };


            AutoCompleteComboBox jobComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(0, 0, 0, 0),
                ItemsSource = new List<Job>(jobsList),
                Width = 130
            };

            TextBlock salaryTextBlock = new TextBlock()
            {
                Margin = new Thickness(30, 0, 5, 0),
                Width = 80,
                IsEnabled = false
            };
            salaryTextBlock.SetBinding(TextBlock.TextProperty, new Binding() { Source = jobComboBox, Path = new PropertyPath(ComboBox.SelectedValueProperty), TargetNullValue = "", Converter = new JobConverter() });


            TextBox salaryRateTextBox = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                Width = 75
            };
            salaryRateTextBox.PreviewTextInput += TextBoxNumbersPreviewInput;

            horizontalStackPanel.Children.Add(jobComboBox);
            horizontalStackPanel.Children.Add(salaryRateTextBox);
            horizontalStackPanel.Children.Add(salaryTextBlock);

            //jobsVerticalListView.Items.Add(horizontalStackPanel);
        }

        /// <summary>
        /// Удаление StackPanel из jobsVerticalListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jobsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //    int countSelectedElement = jobsVerticalListView.SelectedItems.Count;
            //    if (countSelectedElement > 0)
            //    {
            //        for (int i = 0; i < countSelectedElement; i++)
            //        {
            //            jobsVerticalListView.Items.Remove(jobsVerticalListView.SelectedItems[0]);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Выделите нужный для удаления элемент");
            //    }
        }
        private void ChangeJobComboBox(object sender, RoutedEventArgs e)
        {

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

            newPerson.Id = _editedPersonId;
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

            //if (placeOfWorkTextBox.Text != null)
            //{
            //    newPerson.PlaceOfWork = placeOfWorkTextBox.Text;
            //}

            //if (categoryTextBox.Text != null)
            //{
            //    newPerson.Category = categoryTextBox.Text;
            //}

            //if (degreeTextBox.Text != null)
            //{
            //    newPerson.Degree = degreeTextBox.Text;
            //}

            //if (rankTextBox.Text != null)
            //{
            //    newPerson.Rank = rankTextBox.Text;
            //}

            //if (jobsVerticalListView.Items != null)
            //{
            //    AutoCompleteComboBox jobCmb;
            //    TextBox salaryRateTextBox;

            //    foreach (StackPanel jobStackPanel in jobsVerticalListView.Items.OfType<StackPanel>())
            //    {
            //        jobCmb = (AutoCompleteComboBox)jobStackPanel.Children[0];
            //        salaryRateTextBox = (TextBox)jobStackPanel.Children[1];

            //        if (jobCmb.SelectedItem != null && salaryRateTextBox.Text.ToString() != "")
            //        {
            //            newPerson.Jobs.Add(new Job()
            //            {
            //                Id = ((Job)jobCmb.SelectedItem).Id,
            //                Title = ((Job)jobCmb.SelectedItem).Title,
            //                Salary = ((Job)jobCmb.SelectedItem).Salary,
            //                SalaryRate = Parser.ConvertToRightFloat(salaryRateTextBox.Text.ToString())
            //            });
            //        }
            //    }
            //}

            if (isAllOkey)
            {
                CRUDDataBase.ConnectToDataBase();

                if (_isEditPerson)
                {
                    CRUDDataBase.UpdateFIO(newPerson);
                    CRUDDataBase.UpdateBirthDate(newPerson);
                    CRUDDataBase.UpdateSex(newPerson);
                    CRUDDataBase.UpdatePlaceOfWork(newPerson);
                    CRUDDataBase.UpdateCategory(newPerson);
                    CRUDDataBase.UpdateDegree(newPerson);
                    CRUDDataBase.UpdateRank(newPerson);
                    CRUDDataBase.UpdateSalary(newPerson);
                    MessageBox.Show("Человек успешно изменен", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    // Внесение нового человека в бд
                    CRUDDataBase.InsertNewPersonToDB(newPerson);

                    WorkerWithTablesOnMainForm.AddRowToPersonsTable(peopleDataTable, newPerson);

                    MessageBox.Show("Человек успешно внесен", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
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
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) }) ;
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });

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
            Grid.SetRow(CategoryLabel, 0);
            Grid.SetColumn(CategoryLabel, 1);

            ComboBox categoryComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Padding = new Thickness(5, 5, 5, 5),
                ItemsSource = WorkCategoriesList,
            };
            grid.Children.Add(categoryComboBox);
            Grid.SetRow(categoryComboBox, 1);
            Grid.SetColumn(categoryComboBox, 1);

            Label DegreeLabel = new Label
            {
                Content = "Степень"
            };
            grid.Children.Add(DegreeLabel);
            Grid.SetRow(DegreeLabel, 2);
            Grid.SetColumn(DegreeLabel, 0);

            ComboBox degreeComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Padding = new Thickness(5, 5, 5, 5),
                ItemsSource = WorkDegreesList,
            };
            grid.Children.Add(degreeComboBox);
            Grid.SetRow(degreeComboBox, 3);
            Grid.SetColumn(degreeComboBox, 0);

            Label rankLabel = new Label
            {
                Content = "Звание"
            };
            grid.Children.Add(rankLabel);
            Grid.SetRow(rankLabel, 2);
            Grid.SetColumn(rankLabel, 1);

            ComboBox rankComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Padding = new Thickness(5, 5, 5, 5),
                ItemsSource = WorkRanksList,
            };
            grid.Children.Add(rankComboBox);
            Grid.SetRow(rankComboBox, 3);
            Grid.SetColumn(rankComboBox, 1);



            workPlaceListView.Items.Add(grid);

        }

        private void workPlaceDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
