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
    /// Логика взаимодействия для createDepositsWindow.xaml
    /// </summary>
    public partial class createDepositsWindow : Window
    {
        public createDepositsWindow()
        {
            InitializeComponent();
        }
        private void addDepositsButton_Click(object sender, RoutedEventArgs e)
        {
            Depositor depositor = new Depositor();

            if (depositsTextBox.Text != null)
            {
                depositor.Title = depositsTextBox.Text;
                // Покдлючение к бд
                CRUDDataBase.ConnectToDataBase();
                // Внесение нового человека в бд
                CRUDDataBase.InsertNewDepositToDB(depositor);
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
