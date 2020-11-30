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
    /// Логика взаимодействия для createKafedraWindow.xaml
    /// </summary>
    public partial class createKafedraWindow : Window
    {
        public createKafedraWindow()
        {
            InitializeComponent();
        }

        private void addKafedraButton_Click(object sender, RoutedEventArgs e)
        {
            Kafedra kafedra = new Kafedra();

            if (kafedraComboBox.Text != null)
            {
                kafedra.Title = kafedraComboBox.Text;
                // Покдлючение к бд
                CRUDDataBase.ConnectByDataBase();
                // Внесение нового человека в бд
                CRUDDataBase.InsertNewKafedraToDB(kafedra);
                // Закрытие соединения с бд
                CRUDDataBase.CloseConnect();

                MessageBox.Show("Добавление успешно");

                Close();
            }
            else
            {
                MessageBox.Show("Необходимо ввести название кафедры");
            }
        }
    }
}
