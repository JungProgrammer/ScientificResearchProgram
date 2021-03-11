using ResearchProgram.Classes;
using ResearchProgram.Forms;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ResearchProgram
{
    public partial class UniversityStructureWindow : Window
    {
        TreeNode rootNode = new TreeNode
        {
            Id = -1,
            Title = "Структура университета",
            Address = "-1",
            ShortTitle = "",
            Nodes = new ObservableCollection<TreeNode>()
        };

        TreeNode SelectedNode;
        public UniversityStructureWindow()
        {
            InitializeComponent();

            ReloadTree();
            DataContext = this;

        }

        public void ReloadTree()
        {
            rootNode = new TreeNode
            {
                Id = -1,
                Title = "Структура университета",
                Address = "-1",
                ShortTitle = "",
                Nodes = new ObservableCollection<TreeNode>()
            };
            TreeNode TreeNodes = GetHierarchicalNodes();
            treeUniversityStructure.ItemsSource = TreeNodes.Nodes;
        }

        private void GetSelectedTreeViewItemRecursive(TreeNode node)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (node.Nodes[i].IsSelected)
                    SelectedNode = node.Nodes[i];
                GetSelectedTreeViewItemRecursive(node.Nodes[i]);
            }
        }

        private TreeNode GetHierarchicalNodes()
        {

            ObservableCollection<UniversityStructureNode> universityStructureNodes = CRUDDataBase.GetStructureNodes("''");

            Regex rgx = new Regex("^[0-9]+$");
            int k = 0;
            void GetNodesRecursive(Regex r, ref TreeNode node)
            {
                for (int i = 0; i < universityStructureNodes.Count; i++)
                {
                    if (r.IsMatch(universityStructureNodes[i].Address))
                    {
                        TreeNode tr = new TreeNode
                        {
                            Id = universityStructureNodes[i].Id,
                            Title = universityStructureNodes[i].Title,
                            Address = universityStructureNodes[i].Address,
                            ShortTitle = universityStructureNodes[i].ShortTitle,
                            Nodes = new ObservableCollection<TreeNode>()
                        };
                        node.Nodes.Add(tr);
                        Regex rg = new Regex("^" + universityStructureNodes[i].Address + "\\.[0-9]+$");
                        k++;
                        GetNodesRecursive(rg, ref tr);
                    }
                }
            }

            GetNodesRecursive(rgx, ref rootNode);

            return rootNode;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            SelectedNode = null;
            GetSelectedTreeViewItemRecursive(rootNode);
            if (SelectedNode != null)
            {
                Console.WriteLine(SelectedNode);
                AddUniversityStructureNode addUniversityStructureNodeWindow = new AddUniversityStructureNode(ParentNode: SelectedNode)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Owner = this
                };
                addUniversityStructureNodeWindow.Closing += (senders, args) => { addUniversityStructureNodeWindow.Owner = null; };
                addUniversityStructureNodeWindow.Show();
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            SelectedNode = null;
            GetSelectedTreeViewItemRecursive(rootNode);
            if (SelectedNode != null)
            {
                GetSelectedTreeViewItemRecursive(rootNode);
                AddUniversityStructureNode addUniversityStructureNodeWindow = new AddUniversityStructureNode(treeNodeToEdit: SelectedNode)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Owner = this
                };
                addUniversityStructureNodeWindow.Closing += (senders, args) => { addUniversityStructureNodeWindow.Owner = null; };
                addUniversityStructureNodeWindow.Show();
            }
        }


        /// <summary>
        /// Выделение узла по правому клику, своровано отсюда https://stackoverflow.com/questions/592373/select-treeview-node-on-right-click-before-displaying-contextmenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem VisualUpwardSearch(DependencyObject source)
            {
                while (source != null && !(source is TreeViewItem))
                    source = VisualTreeHelper.GetParent(source);

                return source as TreeViewItem;
            }

            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
    }


}
