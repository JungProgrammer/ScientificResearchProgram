using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class UniversityStructureWindow : Window
    {
        public UniversityStructureWindow()
        {
            InitializeComponent();


            DataContext = new UniversityStructureViewModel();
        }

        public static void ShowAddTreeNodeWindow(string showedNameStructure, ref string inputNameStructure)
        {
            UniversityStructureCommandWindows.AddTreeNodeWindow addTreeNodeWindow  = new UniversityStructureCommandWindows.AddTreeNodeWindow(showedNameStructure);
            addTreeNodeWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            if (addTreeNodeWindow.ShowDialog() == true)
            {
                inputNameStructure = addTreeNodeWindow.StructureTitle;
                MessageBox.Show("Вершина успешно добавлена");
            }
            else
            {
                inputNameStructure = string.Empty;
                MessageBox.Show("Все изменения в этом окне будут сброшены");
            }
        }

        public static void ShowAlertAboutUnselectedTreeNode()
        {
            MessageBox.Show("Необходимо выделить вершину");
        }

        public static void ShowAlert(string message)
        {
            MessageBox.Show(message);
        }
    }
}
