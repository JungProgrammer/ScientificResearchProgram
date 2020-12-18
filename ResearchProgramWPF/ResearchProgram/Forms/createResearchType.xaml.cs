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
    /// Логика взаимодействия для createResearchType.xaml
    /// </summary>
    public partial class createResearchType : Window
    {
        public createResearchType()
        {
            InitializeComponent();
        }
        private void addResearchTypeButton_Click(object sender, RoutedEventArgs e)
        {
            ResearchType researchType = new ResearchType();

            if (researchTypeTextBox.Text != null)
            {
                researchType.Title = researchTypeTextBox.Text;
                // Покдлючение к бд
                CRUDDataBase.ConnectToDataBase();
                // Внесение нового человека в бд
                CRUDDataBase.InsertNewResearchTypeToDB(researchType);
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
