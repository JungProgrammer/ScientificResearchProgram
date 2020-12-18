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
    /// Логика взаимодействия для createScienceTypeWindow.xaml
    /// </summary>
    public partial class createScienceTypeWindow : Window
    {
        public createScienceTypeWindow()
        {
            InitializeComponent();
        }
        private void addScienceTypeButton_Click(object sender, RoutedEventArgs e)
        {
            ScienceType scienceType = new ScienceType();

            if (scienceTypeTextBox.Text != null)
            {
                scienceType.Title = scienceTypeTextBox.Text;
                // Покдлючение к бд
                CRUDDataBase.ConnectToDataBase();
                // Внесение нового человека в бд
                CRUDDataBase.InsertNewScienceTypeToDB(scienceType);
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
