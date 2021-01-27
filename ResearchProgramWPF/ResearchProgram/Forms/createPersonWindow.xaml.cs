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
            CRUDDataBase.CloseConnection();

            if (personToEdit != null)
            {
                FIOTextBox.Text = personToEdit.FIO;
                placeOfWorkTextBox.Text = personToEdit.PlaceOfWork;
                degreeTextBox.Text = personToEdit.Degree;
                categoryTextBox.Text = personToEdit.Category;
                rankTextBox.Text = personToEdit.Rank;
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
                    jobsVerticalListView.Items.Add(horizontalStackPanel);
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

            jobsVerticalListView.Items.Add(horizontalStackPanel);
        }

        /// <summary>
        /// Удаление StackPanel из jobsVerticalListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jobsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = jobsVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    jobsVerticalListView.Items.Remove(jobsVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
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


            if (FIOTextBox.Text != null)
            {
                newPerson.FIO = FIOTextBox.Text;
            }

            if (BirthDateDatePicker.SelectedDate != null)
            {
                newPerson.BitrhDate = (DateTime)BirthDateDatePicker.SelectedDate;
            }

            newPerson.Sex = _sexSelected;

            if (placeOfWorkTextBox.Text != null)
            {
                newPerson.PlaceOfWork = placeOfWorkTextBox.Text;
            }

            if (categoryTextBox.Text != null)
            {
                newPerson.Category = categoryTextBox.Text;
            }

            if (degreeTextBox.Text != null)
            {
                newPerson.Degree = degreeTextBox.Text;
            }

            if (rankTextBox.Text != null)
            {
                newPerson.Rank = rankTextBox.Text;
            }

            if (jobsVerticalListView.Items != null)
            {
                AutoCompleteComboBox jobCmb;
                TextBox salaryRateTextBox;

                foreach (StackPanel jobStackPanel in jobsVerticalListView.Items.OfType<StackPanel>())
                {
                    jobCmb = (AutoCompleteComboBox)jobStackPanel.Children[0];
                    salaryRateTextBox = (TextBox)jobStackPanel.Children[2];

                    if (jobCmb.SelectedItem != null && salaryRateTextBox.Text.ToString() != "")
                    {
                        newPerson.Jobs.Add(new Job()
                        {
                            Id = ((Job)jobCmb.SelectedItem).Id,
                            Title = ((Job)jobCmb.SelectedItem).Title,
                            Salary = ((Job)jobCmb.SelectedItem).Salary,
                            SalaryRate = Parser.ConvertToRightFloat(salaryRateTextBox.Text.ToString())
                        });
                    }
                }
            }


            // Покдлючение к бд
            CRUDDataBase.ConnectToDataBase();
            // Внесение нового человека в бд
            CRUDDataBase.InsertNewPersonToDB(newPerson);
            // Закрытие соединения с бд
            CRUDDataBase.CloseConnection();

            WorkerWithTablesOnMainForm.AddRowToPersonsTable(peopleDataTable, newPerson);

            MessageBox.Show("Человек успешно внесен");
        }

    }
}
