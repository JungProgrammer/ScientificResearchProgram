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

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для GrantAggregationWindow.xaml
    /// </summary>
    public partial class GrantAggregationWindow : Window
    {
        public GrantAggregationWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void StartAggregatopnButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
