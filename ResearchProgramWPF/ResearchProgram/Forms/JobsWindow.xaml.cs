using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using ResearchProgram.Forms.HelpWindows;
using ResearchProgram.Classes;
using System.Runtime.CompilerServices;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для JobsWindow.xaml
    /// </summary>
    public partial class JobsWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Job> JobNames { get; set; }

        private Job _selectedJob;
        public Job SelectedJob
        {
            get => _selectedJob;
            set
            {
                _selectedJob = value;
                OnPropertyChanged(nameof(SelectedJob));
            }
        }

        private Job newPlaceWorkElement;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public JobsWindow()
        {
            InitializeComponent();

            LoadJobNames();

            DataContext = this;
        }

        private void LoadJobNames()
        {
            JobNames = new ObservableCollection<Job>();

            CRUDDataBase.ConnectToDataBase();
            JobNames = CRUDDataBase.LoadJobs();
            CRUDDataBase.CloseConnection();
        }

        /// <summary>
        /// Открытие окна добавления элемента
        /// </summary>
        /// <param name="newInputName"></param>
        public void ShowAddElementWindow()
        {
            AddElementWindow addElementWindow = new AddElementWindow(WindowsArgumentsTranfer.IsJobsWindow);
            addElementWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            string newInputName;
            string newSalary;

            if (addElementWindow.ShowDialog() == true)
            {
                newInputName = addElementWindow.NewNameOfElement;
                newSalary = addElementWindow.Salary;
                AddNewJob(newInputName, newSalary);

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
        public void ShowEditElementWindow(Job job)
        {
            EditElementWindow editElementWindow = new EditElementWindow(job.Title, WindowsArgumentsTranfer.IsJobsWindow, job.Salary.ToString());
            editElementWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            string newInputName;
            string newSalary;

            if (editElementWindow.ShowDialog() == true)
            {
                newInputName = editElementWindow.NewNameOfElement;
                newSalary = editElementWindow.Salary;
                EditJob(newInputName, newSalary);

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
            if (SelectedJob != null)
            {
                ShowEditElementWindow(SelectedJob);
            }
            else
            {
                MessageBox.Show("Для редактирования необходимо выбрать нужный элемент");
            }
        }

        private void AddNewJob(string newJobName, string newSalary)
        {
            if (newJobName != null)
            {
                // Добавление в бд
                CRUDDataBase.ConnectToDataBase();
                newPlaceWorkElement = CRUDDataBase.AddJob(newJobName, newSalary);
                CRUDDataBase.CloseConnection();

                // Добавление в список
                JobNames.Add(newPlaceWorkElement);
            }
        }

        private void EditJob(string newJobName, string newSalary)
        {
            // Изменение в списке
            SelectedJob.Title = newJobName;
            SelectedJob.Salary = Parser.ConvertToRightFloat(newSalary);

            // Изменение в бд
            CRUDDataBase.ConnectToDataBase();
            CRUDDataBase.EditJob(SelectedJob);
            CRUDDataBase.CloseConnection();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob != null)
            {
                // Удаление из бд
                CRUDDataBase.ConnectToDataBase();
                CRUDDataBase.DeleteJob(SelectedJob);
                CRUDDataBase.CloseConnection();

                // Удаление из списка
                JobNames.Remove(SelectedJob);
            }
            else
            {
                MessageBox.Show("Для удаления необходимо выбрать нужный элемент");
            }
        }
    }
}
