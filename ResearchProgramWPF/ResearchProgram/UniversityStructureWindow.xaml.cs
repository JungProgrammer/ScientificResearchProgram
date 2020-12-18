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
using ResearchProgram.UniversityStructureCommandWindows;

namespace ResearchProgram
{
    public partial class UniversityStructureWindow : Window
    {
        public UniversityStructureWindow()
        {
            InitializeComponent();


            DataContext = new UniversityStructureViewModel();
        }

        /// <summary>
        /// Открытие окна создания новой ноды
        /// </summary>
        /// <param name="showedNameStructure"></param>
        /// <param name="inputNameStructure"></param>
        public static void ShowAddTreeNodeWindow(string showedNameStructure, ref string inputNameStructure)
        {
            AddTreeNodeWindow addTreeNodeWindow  = new AddTreeNodeWindow(showedNameStructure);
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

        /// <summary>
        /// Открытие окна переименования вершины
        /// </summary>
        /// <param name="newInputName"></param>
        public static void ShowRenameWindow(ref string newInputName)
        {
            RenameNodeWindow renameNodeWindow = new RenameNodeWindow();
            renameNodeWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (renameNodeWindow.ShowDialog() == true)
            {
                newInputName = renameNodeWindow.StructureTitle;
                MessageBox.Show("Вершина успешно изменена");
            }
            else
            {
                newInputName = string.Empty;
                MessageBox.Show("Все изменения в этом окне будут сброшены");
            }
        }

        internal static void ShowWarning(string message, ref bool result)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(message, "Предупреждение", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                result = true;
            }
            else
            {
                result = false;
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
