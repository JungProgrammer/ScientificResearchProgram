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
    /// Логика взаимодействия для createInstitutionWindow.xaml
    /// </summary>
    public partial class createInstitutionWindow : Window
    {
        public createInstitutionWindow()
        {
            InitializeComponent();
        }
        private void addInstitutionButton_Click(object sender, RoutedEventArgs e)
        {
            Institution institution = new Institution();

            if (institutionTextBox.Text != null)
            {
                institution.Title = institutionTextBox.Text;
                // Покдлючение к бд
                CRUDDataBase.ConnectByDataBase();
                // Внесение нового человека в бд
                CRUDDataBase.InsertNewInstituitonToDB(institution);
                // Закрытие соединения с бд
                CRUDDataBase.CloseConnect();

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
