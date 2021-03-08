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

        //// команда добавления нового объекта
        //private RelayCommand addCommand;
        //public RelayCommand AddCommand
        //{
        //    get
        //    {
        //        return addCommand ??
        //          (addCommand = new RelayCommand(obj =>
        //          {
        //              TreeNode selectedNode = TreeNode.GetSelectedNode(RootNode);

        //              if (selectedNode != null)
        //              {
        //                  // Новая нода
        //                  UniversityStructureTreeNode newNode = null;

        //                  string showedNameStructure = string.Empty;
        //                  string inputNameStructure = string.Empty;

        //                  bool isAllOkey = true;

        //                  // Если нужно добавить вуз
        //                  if (selectedNode.IsRootNode)
        //                  {
        //                      showedNameStructure = "Название учреждения";
        //                      UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
        //                      if (inputNameStructure != string.Empty)
        //                      {
        //                          newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewInstitution(inputNameStructure), selectedNode);

        //                          // Создание ноды подразделений
        //                          UniversityStructureTreeNode unitsNode = new UniversityStructureTreeNode(null);
        //                          unitsNode.TitleNode = "Подразделения";
        //                          unitsNode.IsUnitsFolder = true;
        //                          newNode.Children.Add(unitsNode);
        //                          unitsNode.Parent = newNode;

        //                          // Создание ноды лабораторий
        //                          UniversityStructureTreeNode institutionLaboratoriesNode = new UniversityStructureTreeNode(null);
        //                          institutionLaboratoriesNode.TitleNode = "Лаборатории учреждения";
        //                          institutionLaboratoriesNode.IsLaboratoriesFolder = true;
        //                          newNode.Children.Add(institutionLaboratoriesNode);
        //                          institutionLaboratoriesNode.Parent = newNode;
        //                      }
        //                  }
        //                  else if (selectedNode.IsInstitutionNode)
        //                  {
        //                      UniversityStructureWindow.ShowAlert("Нужно уточнить: подразделение или лабораторию");
        //                  }
        //                  // Нужно добавить подразделение
        //                  else if (selectedNode.IsUnitsFolder)
        //                  {
        //                      showedNameStructure = "Название подразделения";
        //                      UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);

        //                      if (inputNameStructure != string.Empty)
        //                      {
        //                          newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewUnit(inputNameStructure, selectedNode.Parent.InstitutionNode), selectedNode);

        //                          // Создание ноды кафедр
        //                          UniversityStructureTreeNode kafedrasNode = new UniversityStructureTreeNode(null);
        //                          kafedrasNode.TitleNode = "Кафедры";
        //                          kafedrasNode.IsKafedrasFolder = true;
        //                          newNode.Children.Add(kafedrasNode);
        //                          kafedrasNode.Parent = newNode;

        //                          // Создание ноды лабораторий
        //                          UniversityStructureTreeNode laboratoriesNode = new UniversityStructureTreeNode(null);
        //                          laboratoriesNode.TitleNode = "Лаборатории";
        //                          laboratoriesNode.IsLaboratoriesFolder = true;
        //                          newNode.Children.Add(laboratoriesNode);
        //                          laboratoriesNode.Parent = newNode;
        //                      }
        //                  }
        //                  // Нужно добавить кафедру
        //                  else if (selectedNode.IsKafedrasFolder)
        //                  {
        //                      showedNameStructure = "Название кафедры";
        //                      UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
        //                      if (inputNameStructure != string.Empty) newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewKafedra(inputNameStructure, selectedNode.Parent.UnitNode), selectedNode);
        //                  }
        //                  // Если нужно добавить лабораторию в подразделение
        //                  else if (selectedNode.IsLaboratoriesFolder)
        //                  {
        //                      showedNameStructure = "Название лаборатории";
        //                      UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
        //                      if (inputNameStructure != string.Empty)
        //                      {
        //                          // Если добавление в список лабораторий у подразделения
        //                          if (selectedNode.Parent.UnitNode != null)
        //                          {
        //                              newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewLaboratory(inputNameStructure, selectedNode.Parent.UnitNode), selectedNode);
        //                          }
        //                          // Если добавление в список лабораторий у учреждения
        //                          else if (selectedNode.Parent.InstitutionNode != null)
        //                          {
        //                              newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewLaboratory(inputNameStructure, selectedNode.Parent.InstitutionNode), selectedNode);
        //                          }
        //                      }
        //                  }
        //                  // Если нужно добавить лабораторию в кафедру
        //                  else if (selectedNode.IsKafedraNode)
        //                  {
        //                      showedNameStructure = "Название лаборатории";
        //                      UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
        //                      if (inputNameStructure != string.Empty) newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewLaboratory(inputNameStructure, selectedNode.KafedraNode), selectedNode);
        //                  }
        //                  else if (selectedNode.IsLaboratoryNode)
        //                  {
        //                      UniversityStructureWindow.ShowAlert("Невозможно добавить в лабораторию что-либо");
        //                      isAllOkey = false;
        //                  }
        //                  else
        //                  {
        //                      UniversityStructureWindow.ShowAlert("Нужно уточнить: кафедру или лабораторию");
        //                      isAllOkey = false;
        //                  }

        //                  if (isAllOkey)
        //                  {
        //                      if (newNode != null)
        //                      {
        //                          selectedNode.Children.Add(newNode);
        //                      }
        //                  }
        //              }
        //              else
        //              {
        //                  UniversityStructureWindow.ShowAlertAboutUnselectedTreeNode();
        //              }
        //          }));
        //    }
        //}

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
