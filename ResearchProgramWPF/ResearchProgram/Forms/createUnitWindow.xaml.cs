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
    /// Логика взаимодействия для createUnitWindow.xaml
    /// </summary>
    public partial class createUnitWindow : Window
    {
        public createUnitWindow()
        {
            InitializeComponent();
        }
        private void addUnitButton_Click(object sender, RoutedEventArgs e)
        {
            Unit unit = new Unit();

            if (unitTextBox.Text != null)
            {
                unit.Title = unitTextBox.Text;
                // Покдлючение к бд
                CRUDDataBase.ConnectToDataBase();
                // Внесение нового человека в бд
                CRUDDataBase.InsertNewUnitToDB(unit);
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
