using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ResearchProgram.Classes
{
    public class PersonWorkPlace : INotifyPropertyChanged
    {
        // кароче этот класс совмещает в себе всю инфу о месте работы человека и дает возможность добавлять всю эту инфу на форму
        public int Id { get; set; }

        public bool IsMainWorkPlace { get; set; }

        public List<Job> jobList { get; set; }

        public UniversityStructureNode firstNode { get; set; }
        public UniversityStructureNode secondNode { get; set; }
        public UniversityStructureNode thirdNode { get; set; }
        public UniversityStructureNode fourthNode { get; set; }

        public WorkCategories workCategory { get; set; }


        public List<WorkCategories> WorkCategoriesList { get; set; }
        public List<WorkDegree> WorkDegreesList { get; set; }
        public List<WorkRank> WorkRanksList { get; set; }
        public List<Job> jobsList { get; set; }

        private ObservableCollection<UniversityStructureNode> _firstNodeList;
        private ObservableCollection<UniversityStructureNode> _secondNodeList;
        private ObservableCollection<UniversityStructureNode> _thirdNodeList;
        private ObservableCollection<UniversityStructureNode> _fourthNodeList;
        public ObservableCollection<UniversityStructureNode> FirstNodeList { get { return _firstNodeList; } set { _firstNodeList = value; OnPropertyChanged("FirstNodeList"); } }
        public ObservableCollection<UniversityStructureNode> SecondNodeList { get { return _secondNodeList; } set { _secondNodeList = value; OnPropertyChanged("SecondNodeList"); } }
        public ObservableCollection<UniversityStructureNode> ThirdNodeList { get { return _thirdNodeList; } set { _thirdNodeList = value; OnPropertyChanged("ThirdNodeList"); } }
        public ObservableCollection<UniversityStructureNode> FourthNodeList { get { return _fourthNodeList; } set { _fourthNodeList = value; OnPropertyChanged("FourthNodeList"); } }

        private bool isUserInteraction = false;
        public CheckBox isMainWorkPlace;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public Grid getPersonGrid()
        {
            CRUDDataBase.ConnectToDataBase();
            jobsList = CRUDDataBase.GetJobs();
            WorkCategoriesList = CRUDDataBase.GetWorkCategories();
            WorkDegreesList = CRUDDataBase.GetWorkDegrees();
            WorkRanksList = CRUDDataBase.GetWorkRanks();
            CRUDDataBase.CloseConnection();

            CRUDDataBase.ConnectToDataBase();
            FirstNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+$'"); // получение всех узлов с адресом первого уровня
            SecondNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом второго уровня
            ThirdNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом третьего уровня
            FourthNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом четвертого уровня
            CRUDDataBase.CloseConnection();

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(180) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(170) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

            ComboBox workPlaceComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Height = 25,
                ItemsSource = FirstNodeList,
                //IsEditable = true,
            };

            ComboBox UnitComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Height = 25,
                ItemsSource = SecondNodeList,
                //IsEditable = true
            };

            ComboBox DepartmentComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Height = 25,
                ItemsSource = ThirdNodeList,
                //IsEditable = true
            };

            ComboBox StructNodeComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Height = 25,
                ItemsSource = FourthNodeList,
                //IsEditable = true
            };

            Label CategoryLabel = new Label
            {
                Content = "Категория"
            };
            grid.Children.Add(CategoryLabel);
            Grid.SetRow(CategoryLabel, 0);
            Grid.SetColumn(CategoryLabel, 0);

            ComboBox categoryComboBox = new ComboBox
            {
                Margin = new Thickness(5, 0, 5, 0),
                Height = 25,
                ItemsSource = WorkCategoriesList,
            };
            if (workCategory != null)
                for (int i = 0; i < WorkCategoriesList.Count; i++)
                {
                    if (workCategory.Id == WorkCategoriesList[i].Id)
                        categoryComboBox.SelectedIndex = i;
                }
            grid.Children.Add(categoryComboBox);
            Grid.SetRow(categoryComboBox, 1);
            Grid.SetColumn(categoryComboBox, 0);

            isMainWorkPlace = new CheckBox
            {
                Content = "Основное место работы",
                IsChecked = false,
                Margin = new Thickness(5)
            };
            grid.Children.Add(isMainWorkPlace);
            Grid.SetRow(isMainWorkPlace, 1);
            Grid.SetColumn(isMainWorkPlace, 1);
            if (IsMainWorkPlace != null)
            {
                isMainWorkPlace.IsChecked = IsMainWorkPlace;
            }


            Label workPlaceLabel = new Label
            {
                Content = "Учреждение"
            };
            grid.Children.Add(workPlaceLabel);
            Grid.SetRow(workPlaceLabel, 2);
            Grid.SetColumn(workPlaceLabel, 0);


            grid.Children.Add(workPlaceComboBox);
            Grid.SetRow(workPlaceComboBox, 3);
            Grid.SetColumn(workPlaceComboBox, 0);
            workPlaceComboBox.SelectionChanged += WorkPlaceComboBox_SelectionChanged;
            workPlaceComboBox.PreviewMouseDown += ComboBoxPreviewMouseDown;
            if (firstNode != null)
            {
                isUserInteraction = true;
                for (int i = 0; i < FirstNodeList.Count; i++)
                {
                    if (firstNode.Id == FirstNodeList[i].Id)
                    {
                        workPlaceComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }


            Label UnitLabel = new Label
            {
                Content = "Подразделение"
            };
            grid.Children.Add(UnitLabel);
            Grid.SetRow(UnitLabel, 4);
            Grid.SetColumn(UnitLabel, 0);

            grid.Children.Add(UnitComboBox);
            Grid.SetRow(UnitComboBox, 5);
            Grid.SetColumn(UnitComboBox, 0);
            UnitComboBox.SelectionChanged += UnitComboBox_SelectionChanged;
            UnitComboBox.PreviewMouseDown += ComboBoxPreviewMouseDown;
            if (secondNode != null)
            {
                isUserInteraction = true;
                for (int i = 0; i < SecondNodeList.Count; i++)
                {
                    if (secondNode.Id == SecondNodeList[i].Id)
                    {
                        UnitComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            Label DepartmentLabel = new Label
            {
                Content = "Отдел"
            };
            grid.Children.Add(DepartmentLabel);
            Grid.SetRow(DepartmentLabel, 2);
            Grid.SetColumn(DepartmentLabel, 1);

            grid.Children.Add(DepartmentComboBox);
            Grid.SetRow(DepartmentComboBox, 3);
            Grid.SetColumn(DepartmentComboBox, 1);
            DepartmentComboBox.SelectionChanged += DepartmentComboBox_SelectionChanged;
            DepartmentComboBox.PreviewMouseDown += ComboBoxPreviewMouseDown;
            if (thirdNode != null)
            {
                isUserInteraction = true;
                for (int i = 0; i < ThirdNodeList.Count; i++)
                {
                    if (thirdNode.Id == ThirdNodeList[i].Id)
                    {
                        DepartmentComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            Label StructNodeLabel = new Label
            {
                Content = "Структурная единица"
            };
            grid.Children.Add(StructNodeLabel);
            Grid.SetRow(StructNodeLabel, 4);
            Grid.SetColumn(StructNodeLabel, 1);

            grid.Children.Add(StructNodeComboBox);
            Grid.SetRow(StructNodeComboBox, 5);
            Grid.SetColumn(StructNodeComboBox, 1);
            StructNodeComboBox.SelectionChanged += StructNodeComboBox_SelectionChanged;
            StructNodeComboBox.PreviewMouseDown += ComboBoxPreviewMouseDown;
            if (fourthNode != null)
            {
                isUserInteraction = true;
                for (int i = 0; i < FourthNodeList.Count; i++)
                {
                    if (fourthNode.Id == FourthNodeList[i].Id)
                    {
                        StructNodeComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            isUserInteraction = false;

            Grid jobGrid = new Grid();
            jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
            jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
            jobGrid.ColumnDefinitions.Add(new ColumnDefinition());
            jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(120) });
            jobGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.Children.Add(jobGrid);
            Grid.SetRow(jobGrid, 0);
            Grid.SetColumn(jobGrid, 2);
            Grid.SetColumnSpan(jobGrid, 2);
            Grid.SetRowSpan(jobGrid, 6);

            Label jobLabel = new Label
            {
                Content = "Должность"
            };
            jobGrid.Children.Add(jobLabel);

            Label salaryLabel = new Label
            {
                Content = "Оклад"
            };
            jobGrid.Children.Add(salaryLabel);
            Grid.SetRow(salaryLabel, 0);
            Grid.SetColumn(salaryLabel, 1);

            Label rateLabel = new Label
            {
                Content = "Ставка"
            };
            jobGrid.Children.Add(rateLabel);
            Grid.SetRow(rateLabel, 0);
            Grid.SetColumn(rateLabel, 2);

            ListView jobListView = new ListView();
            jobGrid.Children.Add(jobListView);
            Grid.SetRow(jobListView, 1);
            Grid.SetColumn(jobListView, 0);
            Grid.SetColumnSpan(jobListView, 3);


            Button addJobButton = new Button
            {
                Content = "Добавить",
                Margin = new Thickness(0, 5, 5, 2)

            };
            addJobButton.Click += addJobButton_click;
            jobGrid.Children.Add(addJobButton);
            Grid.SetRow(addJobButton, 2);
            Grid.SetColumn(addJobButton, 0);

            Button deleteJobButton = new Button
            {
                Content = "Удалить",
                Margin = new Thickness(0, 5, 5, 2)
            };
            deleteJobButton.Click += deleteJobButton_click;
            jobGrid.Children.Add(deleteJobButton);
            Grid.SetRow(deleteJobButton, 2);
            Grid.SetColumn(deleteJobButton, 1);

            if (jobList != null)
                foreach (Job job in jobList)
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
                        Margin = new Thickness(5),
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



            void addJobButton_click(object sender2, RoutedEventArgs e2)
            {

                CRUDDataBase.ConnectToDataBase();
                FirstNodeList = CRUDDataBase.GetStructureNodes("'^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$'"); // получение всех узлов с адресом первого уровня
                CRUDDataBase.CloseConnection();


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
                    Margin = new Thickness(5),
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

            void WorkPlaceComboBox_SelectionChanged(object sender2, SelectionChangedEventArgs e2)
            {
                if (isUserInteraction)
                {
                    isUserInteraction = false;
                    ComboBox comboBox = (ComboBox)sender2;
                    UniversityStructureNode universityStructureNode = (UniversityStructureNode)comboBox.SelectedItem;

                    SecondNodeList.Clear();
                    foreach (UniversityStructureNode u in CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address + "\\.[0-9]+$'"))
                    {
                        SecondNodeList.Add(u);
                    }
                    ThirdNodeList.Clear();
                    foreach (UniversityStructureNode u in CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address + "\\.[0-9]+\\.[0-9]+$'"))
                    {
                        ThirdNodeList.Add(u);
                    }

                    FourthNodeList.Clear();
                    foreach (UniversityStructureNode u in CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address + "\\.[0-9]+\\.[0-9]+\\.[0-9]+$'"))
                    {
                        FourthNodeList.Add(u);
                    }
                }

            }

            void UnitComboBox_SelectionChanged(object sender2, SelectionChangedEventArgs e2)
            {
                if (isUserInteraction)
                {
                    isUserInteraction = false;
                    ComboBox comboBox = (ComboBox)sender2;
                    UniversityStructureNode universityStructureNode = (UniversityStructureNode)comboBox.SelectedItem;
                    string[] address = universityStructureNode.Address.Split('.');
                    UniversityStructureNode parentNode = CRUDDataBase.GetStructureNodes("'^" + address[0] + "$'")[0];


                    for (int i = 0; i < workPlaceComboBox.Items.Count; i++)
                    {
                        if (parentNode.Id == ((UniversityStructureNode)workPlaceComboBox.Items[i]).Id)
                        {
                            workPlaceComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    // костыль капец нафек
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (universityStructureNode.Id == ((UniversityStructureNode)comboBox.Items[i]).Id)
                        {
                            comboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    if (universityStructureNode != null)
                    {
                        ThirdNodeList.Clear();
                        foreach (UniversityStructureNode u in CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address + "\\.[0-9]+$'"))
                        {
                            ThirdNodeList.Add(u);
                        }

                        FourthNodeList.Clear();
                        foreach (UniversityStructureNode u in CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address + "\\.[0-9]+\\.[0-9]+$'"))
                        {
                            FourthNodeList.Add(u);
                        }
                    }
                }
            }

            void DepartmentComboBox_SelectionChanged(object sender2, SelectionChangedEventArgs e2)
            {
                if (isUserInteraction)
                {
                    isUserInteraction = false;
                    ComboBox comboBox = (ComboBox)sender2;
                    UniversityStructureNode universityStructureNode = (UniversityStructureNode)comboBox.SelectedItem;

                    UniversityStructureNode parentNode = CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address.Split('.')[0] + "$'")[0];

                    for (int i = 0; i < workPlaceComboBox.Items.Count; i++)
                    {
                        if (parentNode.Id == ((UniversityStructureNode)workPlaceComboBox.Items[i]).Id)
                        {
                            workPlaceComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    // костыль капец нафек
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (universityStructureNode.Id == ((UniversityStructureNode)comboBox.Items[i]).Id)
                        {
                            comboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    parentNode = CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address.Split('.')[0] + '.' + universityStructureNode.Address.Split('.')[1] + "$'")[0];

                    for (int i = 0; i < UnitComboBox.Items.Count; i++)
                    {
                        if (parentNode.Id == ((UniversityStructureNode)UnitComboBox.Items[i]).Id)
                        {
                            UnitComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    // костыль капец нафек
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (universityStructureNode.Id == ((UniversityStructureNode)comboBox.Items[i]).Id)
                        {
                            comboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    if (universityStructureNode != null)
                    {
                        FourthNodeList.Clear();
                        foreach (UniversityStructureNode u in CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address + "\\.[0-9]+$'"))
                        {
                            FourthNodeList.Add(u);
                        }
                    }
                }
            }

            void StructNodeComboBox_SelectionChanged(object sender2, SelectionChangedEventArgs e2)
            {
                if (isUserInteraction)
                {
                    isUserInteraction = false;
                    ComboBox comboBox = (ComboBox)sender2;
                    UniversityStructureNode universityStructureNode = (UniversityStructureNode)comboBox.SelectedItem;
                    UniversityStructureNode parentNode = CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address.Split('.')[0] + "$'")[0];

                    for (int i = 0; i < workPlaceComboBox.Items.Count; i++)
                    {
                        if (parentNode.Id == ((UniversityStructureNode)workPlaceComboBox.Items[i]).Id)
                        {
                            workPlaceComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    // костыль капец нафек
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (universityStructureNode.Id == ((UniversityStructureNode)comboBox.Items[i]).Id)
                        {
                            comboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    parentNode = CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address.Split('.')[0] + '.' + universityStructureNode.Address.Split('.')[1] + "$'")[0];

                    for (int i = 0; i < UnitComboBox.Items.Count; i++)
                    {
                        if (parentNode.Id == ((UniversityStructureNode)UnitComboBox.Items[i]).Id)
                        {
                            UnitComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    // костыль капец нафек
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (universityStructureNode.Id == ((UniversityStructureNode)comboBox.Items[i]).Id)
                        {
                            comboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    parentNode = CRUDDataBase.GetStructureNodes("'^" + universityStructureNode.Address.Split('.')[0] + '.' + universityStructureNode.Address.Split('.')[1] + '.' + universityStructureNode.Address.Split('.')[2] + "$'")[0];

                    for (int i = 0; i < DepartmentComboBox.Items.Count; i++)
                    {
                        if (parentNode.Id == ((UniversityStructureNode)DepartmentComboBox.Items[i]).Id)
                        {
                            DepartmentComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    // костыль капец нафек
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (universityStructureNode.Id == ((UniversityStructureNode)comboBox.Items[i]).Id)
                        {
                            comboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    if (universityStructureNode != null)
                    {

                    }
                }
            }

            return grid;
        }

        private void ComboBoxPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isUserInteraction = true;
        }
    }
}
