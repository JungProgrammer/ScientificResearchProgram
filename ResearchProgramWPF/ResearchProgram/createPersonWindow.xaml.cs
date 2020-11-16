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


namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createPersonWindow.xaml
    /// </summary>
    public partial class createPersonWindow : Window
    {
        public string sexChecked;

        public createPersonWindow(DataTable personsDataTable)
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            sexChecked = pressed.Content.ToString();
        }

        private void createPersonButtonClick(object sender, RoutedEventArgs e)
        {

        }
        private void jobsAddButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void jobsDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void personParametersButtonClick(object sender, RoutedEventArgs e)
        {
            createPersonTabControl.SelectedItem = createPersonTabControl.Items.OfType<TabItem>().SingleOrDefault(n => n.Name == ((Button)sender).Tag.ToString());
            foreach (Button button in personParametersButtonStackPanel.Children.OfType<Button>())
            {
                button.Background = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
            }
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 189, 189, 189));
        }
    }
}
