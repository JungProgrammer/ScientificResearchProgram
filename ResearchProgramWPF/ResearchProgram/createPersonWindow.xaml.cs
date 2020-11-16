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
using DotNetKit.Windows.Controls;


namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createPersonWindow.xaml
    /// </summary>
    public partial class createPersonWindow : Window
    {
        public string sexChecked;

        public List<Job> jobsList { get; set; }



        public createPersonWindow(DataTable personsDataTable)
        {
            InitializeComponent();

            CRUDDataBase.ConnectByDataBase();
            jobsList = CRUDDataBase.GetJobs();
            CRUDDataBase.CloseConnect();

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
            StackPanel horizontalStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            AutoCompleteComboBox jobComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<Job>(jobsList),
                MinWidth = 150
            };

            TextBox salaryTextBox = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                MinWidth = 120,
                IsEnabled = false
            };

            TextBox salaryRateTextBox = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                MinWidth = 75
            };

            horizontalStackPanel.Children.Add(jobComboBox);
            horizontalStackPanel.Children.Add(salaryTextBox);
            horizontalStackPanel.Children.Add(salaryRateTextBox);


            jobsVerticalListView.Items.Add(horizontalStackPanel);
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
