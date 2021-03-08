using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ResearchProgram.Classes;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для AddUniversityStructureNode.xaml
    /// </summary>
    public partial class AddUniversityStructureNode : Window
    {
        Classes.TreeNode _treeNode;
        Classes.TreeNode _parentNode;
        bool _isEdit = false;

        public AddUniversityStructureNode(Classes.TreeNode treeNodeToEdit = null, Classes.TreeNode ParentNode = null)
        {
            InitializeComponent();

            _treeNode = treeNodeToEdit;
            _parentNode = ParentNode;
            if (treeNodeToEdit != null)
            {
                NewNodeTextBox.Text = _treeNode.Title;
                NewNodeShortNameTextBox.Text = _treeNode.ShortTitle;
                Title = "Редактирование структурной единицы";
                DeleteNodeButton.Visibility = Visibility.Visible;
                _isEdit = true;
                AddNodeButton.Content = "Сохранить";
            }

        }

        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            NewNodeTextBox.Text = NewNodeTextBox.Text.Trim(' ');
            string NodeTitle = NewNodeTextBox.Text;
            string NodeShortTitle = NewNodeShortNameTextBox.Text;
            if (!_isEdit)
            {
                if (NewNodeTextBox.Text != "")
                {
                    CRUDDataBase.AddNewUniversityStructureNode(_parentNode, NodeTitle, NodeShortTitle);
                    System.Windows.MessageBox.Show("Добавление успешно", "Добавление новой структурной единицы", MessageBoxButton.OK, MessageBoxImage.Information);
                    ((UniversityStructureWindow)Owner).ReloadTree();
                    Close();
                }
                else
                {
                    System.Windows.MessageBox.Show("Необходимо ввести название структурной единицы", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                CRUDDataBase.EditUniversityStructureNode(_treeNode, NodeTitle, NodeShortTitle);
                System.Windows.MessageBox.Show("Информация сохранена", "Изменение структурной единицы", MessageBoxButton.OK, MessageBoxImage.Information);
                ((UniversityStructureWindow)Owner).ReloadTree();
                Close();
            }
        }

        private void DeleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult sure = System.Windows.MessageBox.Show("Удалить структурную единицу с названием " + _treeNode.Title + ", а также всех его потомков?", "Удаление структурной единицы", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            switch (sure)
            {
                case MessageBoxResult.Yes:
                    CRUDDataBase.DeleteUniversityStructureNode(_treeNode);
                    System.Windows.MessageBox.Show("Удаление успешно", "Удаление структурной единицы", MessageBoxButton.OK, MessageBoxImage.Information);
                    ((UniversityStructureWindow)Owner).ReloadTree();
                    Close();
                    break;
            }
        }
    }
}
