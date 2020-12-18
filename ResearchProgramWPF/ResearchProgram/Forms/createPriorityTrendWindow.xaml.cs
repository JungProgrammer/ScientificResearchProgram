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

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createPriorityTrendWindow.xaml
    /// </summary>
    public partial class createPriorityTrendWindow : Window
    {
        public createPriorityTrendWindow()
        {
            InitializeComponent();
        }
        private void addPriorityTrendButton_Click(object sender, RoutedEventArgs e)
        {
            PriorityTrend priorityTrend = new PriorityTrend();

            if (priorityTrendTextBox.Text != null)
            {
                priorityTrend.Title = priorityTrendTextBox.Text;
                // Покдлючение к бд
                CRUDDataBase.ConnectToDataBase();
                // Внесение нового человека в бд
                CRUDDataBase.InsertNewPriorityTrendsToDB(priorityTrend);
                // Закрытие соединения с бд
                CRUDDataBase.CloseConnection();

                MessageBox.Show("Добавление успешно");

                Close();
            }
            else
            {
                MessageBox.Show("Необходимо ввести источник средств");
            }
        }
    }
}
