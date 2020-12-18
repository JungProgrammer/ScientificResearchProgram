using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class UniversityStructureViewModel: INotifyPropertyChanged
    {
        public class UniversityStructureTreeNode: INotifyPropertyChanged
        {
            public bool IsRootNode { get; set; }
            public bool IsKafedrasFolder { get; set; }
            public bool IsLaboratoriesFolder { get; set; }
            public bool IsInstitutionNode { get; set; }
            public bool IsUnitNode { get; set; }
            public bool IsKafedraNode { get; set; }
            public bool IsLaboratoryNode { get; set; }

            public Institution InstitutionNode { get; set; }
            public Unit UnitNode { get; set; }
            public Kafedra KafedraNode { get; set; }
            public Laboratory LaboratoryNode { get; set; }


            private string titleNode;
            // Отображаемое имя элемента дерева
            public string TitleNode
            {
                get { return titleNode; }
                set
                {
                    titleNode = value;
                    OnPropertyChanged(nameof(TitleNode));
                }
            }

            private bool _isSelected;
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    if (value != _isSelected)
                    {
                        _isSelected = value;
                        this.OnPropertyChanged("IsSelected");
                    }
                }
            }

            private bool _isExpanded;
            public bool IsExpanded
            {
                get { return _isExpanded; }
                set
                {
                    if (value != _isExpanded)
                    {
                        _isExpanded = value;
                        this.OnPropertyChanged("IsExpanded");
                    }
                }
            }

            private UniversityStructureTreeNode _parent;
            public UniversityStructureTreeNode Parent
            {
                get { return _parent; }
                set
                {
                    _parent = value;
                    OnPropertyChanged(nameof(Parent));
                }
            }

            public ObservableCollection<UniversityStructureTreeNode> Children { get; set; }


            public UniversityStructureTreeNode(object NodeElement)
            {
                // Определение типа, присваивание соответсвующего свойства
                if (NodeElement is Institution)
                {
                    InstitutionNode = (Institution)NodeElement;
                    IsInstitutionNode = true;
                    TitleNode = $"{InstitutionNode.Title}";
                }
                else if (NodeElement is Unit)
                {
                    UnitNode = (Unit)NodeElement;
                    IsUnitNode = true;
                    TitleNode = UnitNode.Title;
                }
                else if (NodeElement is Kafedra)
                {
                    KafedraNode = (Kafedra)NodeElement;
                    IsKafedraNode = true;
                    TitleNode = $"Кафедра \"{KafedraNode.Title}\"";
                }
                else if (NodeElement is Laboratory)
                {
                    LaboratoryNode = (Laboratory)NodeElement;
                    IsLaboratoryNode = true;
                    TitleNode = LaboratoryNode.Title;
                }


                Children = new ObservableCollection<UniversityStructureTreeNode>();
            }


            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }

            internal static UniversityStructureTreeNode ConvertUsualStructureToTreeElement(WorkerWithUniversityStructure universityStructure)
            {
                UniversityStructureTreeNode rootNode = new UniversityStructureTreeNode(null);
                rootNode.TitleNode = "Учреждения";
                rootNode.IsRootNode = true;

                foreach(Institution institution in universityStructure.Institutions)
                {
                    UniversityStructureTreeNode institutionNode = new UniversityStructureTreeNode(institution);
                    rootNode.Children.Add(institutionNode);
                    institutionNode.Parent = rootNode;

                    foreach(Unit unit in institution.Units)
                    {
                        UniversityStructureTreeNode unitNode = new UniversityStructureTreeNode(unit);
                        institutionNode.Children.Add(unitNode);
                        unitNode.Parent = institutionNode;

                        // Создание ноды кафедр
                        UniversityStructureTreeNode kafedrasNode = new UniversityStructureTreeNode(null);
                        kafedrasNode.TitleNode = "Кафедры";
                        kafedrasNode.IsKafedrasFolder = true;
                        unitNode.Children.Add(kafedrasNode);
                        kafedrasNode.Parent = unitNode;

                        UniversityStructureTreeNode laboratoriesNode = new UniversityStructureTreeNode(null);
                        laboratoriesNode.TitleNode = "Лаборатории";
                        laboratoriesNode.IsLaboratoriesFolder = true;
                        unitNode.Children.Add(laboratoriesNode);
                        laboratoriesNode.Parent = unitNode;

                        foreach(Kafedra kafedra in unit.Kafedras)
                        {
                            UniversityStructureTreeNode kafedraNode = new UniversityStructureTreeNode(kafedra);
                            kafedrasNode.Children.Add(kafedraNode);
                            kafedraNode.Parent = kafedrasNode;

                            foreach(Laboratory laboratory in kafedra.Laboratories)
                            {
                                UniversityStructureTreeNode laboratoryNode = new UniversityStructureTreeNode(laboratory);
                                kafedraNode.Children.Add(laboratoryNode);
                                laboratoryNode.Parent = kafedraNode;
                            }
                        }
                        foreach(Laboratory laboratory in unit.Laboratories)
                        {
                            UniversityStructureTreeNode laboratoryNode = new UniversityStructureTreeNode(laboratory);
                            laboratoriesNode.Children.Add(laboratoryNode);
                            laboratoryNode.Parent = laboratoriesNode;
                        }
                    }
                }

                return rootNode;
            }

            /// <summary>
            /// Создание новой ноды
            /// </summary>
            /// <param name="newStructure"></param>
            /// <param name="Parent"></param>
            /// <returns></returns>
            public static UniversityStructureTreeNode CreateNewTreeNode(object newStructure, UniversityStructureTreeNode Parent)
            {
                UniversityStructureTreeNode newTreeNode = new UniversityStructureTreeNode(newStructure);
                newTreeNode.Parent = Parent;

                return newTreeNode;
            }


            /// <summary>
            /// Поиск выделенной вершины
            /// </summary>
            /// <returns></returns>
            public static UniversityStructureTreeNode GetSelectedNode(ObservableCollection<UniversityStructureTreeNode> RootNode)
            {
                UniversityStructureTreeNode selectedTreeNode = null;

                // Перебираем корень
                foreach (UniversityStructureTreeNode treeNode in RootNode)
                {
                    if (treeNode.IsSelected) selectedTreeNode = treeNode;
                    // Перебираем университеты
                    foreach (UniversityStructureTreeNode treeNode1 in treeNode.Children)
                    {
                        if (treeNode1.IsSelected) selectedTreeNode = treeNode1;
                        else
                        {
                            // Перебираем подразделения
                            foreach (UniversityStructureTreeNode treeNode2 in treeNode1.Children)
                            {
                                if (treeNode2.IsSelected) selectedTreeNode = treeNode2;
                                else
                                {
                                    // Перебираем папки кафедры и лаборатории в подразделении
                                    foreach (UniversityStructureTreeNode treeNode3 in treeNode2.Children)
                                    {
                                        if (treeNode3.IsSelected) selectedTreeNode = treeNode3;
                                        else
                                        {
                                            // Перебираем кафедры в подразделении
                                            foreach (UniversityStructureTreeNode treeNode4 in treeNode3.Children)
                                            {
                                                if (treeNode4.IsSelected) selectedTreeNode = treeNode4;
                                                else
                                                {
                                                    // Перебираем лаборатории в кафедре
                                                    foreach (UniversityStructureTreeNode treeNode5 in treeNode4.Children)
                                                    {
                                                        if (treeNode5.IsSelected) selectedTreeNode = treeNode5;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return selectedTreeNode;
            }


            public static void DeleteNode(UniversityStructureTreeNode deleteNode)
            {
                deleteNode.Parent.Children.Remove(deleteNode);
            }
        }




        public UniversityStructureViewModel()
        {
            // Получение структуры бд
            CRUDDataBase.ConnectToDataBase();
            UniversityStructure = CRUDDataBase.GetUniversityStructure();
            CRUDDataBase.CloseConnection();

            RootNode = new ObservableCollection<UniversityStructureTreeNode>() { UniversityStructureTreeNode.ConvertUsualStructureToTreeElement(UniversityStructure) };
        }


        // команда добавления нового объекта
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      UniversityStructureTreeNode selectedNode = UniversityStructureTreeNode.GetSelectedNode(RootNode);

                      if (selectedNode != null)
                      {
                          // Новая нода
                          UniversityStructureTreeNode newNode = null;

                          string showedNameStructure = string.Empty;
                          string inputNameStructure = string.Empty;

                          bool isAllOkey = true;

                          // Если нужно добавить вуз
                          if (selectedNode.IsRootNode)
                          {
                              showedNameStructure = "Название учреждения";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
                              if (inputNameStructure != string.Empty) newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewInstitution(inputNameStructure), selectedNode);
                          }
                          // Нужно добавить подразделение
                          else if (selectedNode.IsInstitutionNode)
                          {
                              showedNameStructure = "Название подразделения";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);

                              if (inputNameStructure != string.Empty) 
                              {
                                  newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewUnit(inputNameStructure, selectedNode.InstitutionNode), selectedNode);

                                  // Создание ноды кафедр
                                  UniversityStructureTreeNode kafedrasNode = new UniversityStructureTreeNode(null);
                                  kafedrasNode.TitleNode = "Кафедры";
                                  kafedrasNode.IsKafedrasFolder = true;
                                  newNode.Children.Add(kafedrasNode);
                                  kafedrasNode.Parent = newNode;

                                  // Создание ноды лабораторий
                                  UniversityStructureTreeNode laboratoriesNode = new UniversityStructureTreeNode(null);
                                  laboratoriesNode.TitleNode = "Лаборатории";
                                  laboratoriesNode.IsLaboratoriesFolder = true;
                                  newNode.Children.Add(laboratoriesNode);
                                  laboratoriesNode.Parent = newNode;
                              }
                          }
                          // Нужно добавить кафедру
                          else if (selectedNode.IsKafedrasFolder)
                          {
                              showedNameStructure = "Название кафедры";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
                              if(inputNameStructure != string.Empty) newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewKafedra(inputNameStructure, selectedNode.Parent.UnitNode), selectedNode);
                          }
                          // Если нужно добавить лабораторию в подразделение
                          else if (selectedNode.IsLaboratoriesFolder)
                          {
                              showedNameStructure = "Название лаборатории";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
                              if (inputNameStructure != string.Empty) newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewLaboratory(inputNameStructure, selectedNode.Parent.UnitNode), selectedNode);
                          }
                          // Если нужно добавить лабораторию в кафедру
                          else if (selectedNode.IsKafedraNode)
                          {
                              showedNameStructure = "Название лаборатории";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
                              if (inputNameStructure != string.Empty)  newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewLaboratory(inputNameStructure, selectedNode.KafedraNode), selectedNode);
                          }
                          else if(selectedNode.IsLaboratoryNode)
                          {
                              UniversityStructureWindow.ShowAlert("Невозможно добавить в лабораторию что-либо");
                              isAllOkey = false;
                          }
                          else
                          {
                              UniversityStructureWindow.ShowAlert("Нужно уточнить: кафедру или лабораторию");
                              isAllOkey = false;
                          }

                          if (isAllOkey)
                          {
                              if(newNode != null)
                              {
                                  selectedNode.Children.Add(newNode);
                              }
                          }
                      }
                      else
                      {
                          UniversityStructureWindow.ShowAlertAboutUnselectedTreeNode();
                      }
                  }));
            }
        }


        // Команда переименования
        private RelayCommand renameCommand;
        public RelayCommand RenameCommand
        {
            get
            {
                return renameCommand ??
                  (renameCommand = new RelayCommand(obj =>
                  {
                      UniversityStructureTreeNode selectedNode = UniversityStructureTreeNode.GetSelectedNode(RootNode);


                      string newInputName = string.Empty;

                      if (selectedNode != null)
                      {
                          // Если был переименован университет
                          if (selectedNode.IsInstitutionNode)
                          {
                              UniversityStructureWindow.ShowRenameWindow(ref newInputName);
                              WorkerWithUniversityStructure.RenameInstitution(selectedNode.InstitutionNode, newInputName);
                              selectedNode.TitleNode = newInputName;
                          }
                          // Если было переименовано подразделение
                          else if (selectedNode.IsUnitNode)
                          {
                              UniversityStructureWindow.ShowRenameWindow(ref newInputName);
                              WorkerWithUniversityStructure.RenameUnit(selectedNode.UnitNode, newInputName);
                              selectedNode.TitleNode = newInputName;
                          }
                          // Если была переименована кафедра
                          else if (selectedNode.IsKafedraNode)
                          {
                              UniversityStructureWindow.ShowRenameWindow(ref newInputName);
                              WorkerWithUniversityStructure.RenameKafedra(selectedNode.KafedraNode, newInputName);
                              selectedNode.TitleNode = $"Кафедра \"{newInputName}\"";
                          }
                          // Если была переименована лаборатория
                          else if (selectedNode.IsLaboratoryNode)
                          {
                              UniversityStructureWindow.ShowRenameWindow(ref newInputName);
                              WorkerWithUniversityStructure.RenameLaboratory(selectedNode.LaboratoryNode, newInputName);
                              selectedNode.TitleNode = newInputName;
                          }
                          // Если другая вершина, то предупреждение
                          else
                          {
                              UniversityStructureWindow.ShowAlert("Эту вершину невозможно переименовать");
                          }
                      }
                      else
                      {
                          UniversityStructureWindow.ShowAlertAboutUnselectedTreeNode();
                      }

                  }));
            }
        }


        // Команда удаления
        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      UniversityStructureTreeNode selectedNode = UniversityStructureTreeNode.GetSelectedNode(RootNode);


                      if (selectedNode != null)
                      {
                          bool result = false;

                          // Если был удален университет
                          if (selectedNode.IsInstitutionNode)
                          {
                              UniversityStructureWindow.ShowWarning("Вы действительно хотите удалить университет?", ref result);
                              if (result)
                              {
                                  WorkerWithUniversityStructure.DeleteInstitution(selectedNode.InstitutionNode);
                                  UniversityStructureTreeNode.DeleteNode(selectedNode);
                              }
                          }
                          // Если было удалено подразделение
                          else if (selectedNode.IsUnitNode)
                          {
                              UniversityStructureWindow.ShowWarning("Вы действительно хотите удалить подразделение?", ref result);
                              if (result)
                              {
                                  WorkerWithUniversityStructure.DeleteUnit(selectedNode.UnitNode);
                                  UniversityStructureTreeNode.DeleteNode(selectedNode);
                              }
                          }
                          // Если была удалена кафедра
                          else if (selectedNode.IsKafedraNode)
                          {
                              UniversityStructureWindow.ShowWarning("Вы действительно хотите удалить кафедру?", ref result);
                              if (result)
                              {
                                  WorkerWithUniversityStructure.DeleteKafedra(selectedNode.KafedraNode);
                                  UniversityStructureTreeNode.DeleteNode(selectedNode);
                              }
                          }
                          // Если была удалена лаборатория
                          else if (selectedNode.IsLaboratoryNode)
                          {
                              UniversityStructureWindow.ShowWarning("Вы действительно хотите удалить лабораторию?", ref result);
                              if (result)
                              {
                                  WorkerWithUniversityStructure.DeleteLaboratory(selectedNode.LaboratoryNode);
                                  UniversityStructureTreeNode.DeleteNode(selectedNode);
                              }
                          }
                          // Если другая вершина, то предупреждение
                          else
                          {
                              UniversityStructureWindow.ShowAlert("Эту вершину невозможно удалить");
                          }
                      }
                      else
                      {
                          UniversityStructureWindow.ShowAlertAboutUnselectedTreeNode();
                      }

                  }));
            }
        }


        public ObservableCollection<UniversityStructureTreeNode> RootNode { get; set; }


        private WorkerWithUniversityStructure universityStructure;
        public WorkerWithUniversityStructure UniversityStructure
        {
            get { return universityStructure; }
            set
            {
                universityStructure = value;
                OnPropertyChanged(nameof(UniversityStructure));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
