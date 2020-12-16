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
        }




        public UniversityStructureViewModel()
        {
            // Получение структуры бд
            CRUDDataBase.ConnectByDataBase();
            UniversityStructure = CRUDDataBase.GetUniversityStructure();
            CRUDDataBase.CloseConnect();

            RootNode = new ObservableCollection<UniversityStructureTreeNode>() { UniversityStructureTreeNode.ConvertUsualStructureToTreeElement(UniversityStructure) };
        }

        private UniversityStructureTreeNode GetSelectedNode()
        {
            UniversityStructureTreeNode selectedTreeNode = null;

            // Перебираем корень
            foreach(UniversityStructureTreeNode treeNode in RootNode)
            {
                if (treeNode.IsSelected) selectedTreeNode = treeNode;
                // Перебираем университеты
                foreach (UniversityStructureTreeNode treeNode1 in treeNode.Children)
                {
                    if (treeNode1.IsSelected) selectedTreeNode = treeNode1;
                    else
                    {
                        // Перебираем подразделения
                        foreach(UniversityStructureTreeNode treeNode2 in treeNode1.Children)
                        {
                            if (treeNode2.IsSelected) selectedTreeNode = treeNode2;
                            else
                            {
                                // Перебираем папки кафедры и лаборатории в подразделении
                                foreach(UniversityStructureTreeNode treeNode3 in treeNode2.Children)
                                {
                                    if (treeNode3.IsSelected) selectedTreeNode = treeNode3;
                                    else
                                    {
                                        // Перебираем кафедры в подразделении
                                        foreach(UniversityStructureTreeNode treeNode4 in treeNode3.Children)
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

        // команда добавления нового объекта
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      UniversityStructureTreeNode selectedNode = GetSelectedNode();
                      if(selectedNode != null)
                      {
                          // Новая нода
                          UniversityStructureTreeNode newNode = null;

                          string showedNameStructure = string.Empty;
                          string inputNameStructure = string.Empty;

                          // Если нужно добавить вуз
                          if (selectedNode.IsRootNode)
                          {
                              showedNameStructure = "Название учреждения";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
                              newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewInstitution(inputNameStructure), selectedNode);
                          }
                          // Нужно добавить подразделение
                          else if (selectedNode.IsInstitutionNode)
                          {
                              showedNameStructure = "Название подразделения";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
                              newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewUnit(inputNameStructure), selectedNode);
                          }
                          // Нужно добавить кафедру
                          else if (selectedNode.IsKafedrasFolder)
                          {
                              showedNameStructure = "Название кафедры";
                              UniversityStructureWindow.ShowAddTreeNodeWindow(showedNameStructure, ref inputNameStructure);
                              newNode = UniversityStructureTreeNode.CreateNewTreeNode(WorkerWithUniversityStructure.CreateNewU(inputNameStructure), selectedNode);
                          }


                          selectedNode.Children.Add(newNode);
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
