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

            public ObservableCollection<UniversityStructureTreeNode> Children { get; set; }


            public UniversityStructureTreeNode(object NodeElement)
            {
                // Определение типа, присваивание соответсвующего свойства
                if (NodeElement is Institution)
                {
                    InstitutionNode = (Institution)NodeElement;
                    TitleNode = $"{InstitutionNode.Title}";
                }
                else if (NodeElement is Unit)
                {
                    UnitNode = (Unit)NodeElement;
                    TitleNode = UnitNode.Title;
                }
                else if (NodeElement is Kafedra)
                {
                    KafedraNode = (Kafedra)NodeElement;
                    TitleNode = $"Кафедра \"{KafedraNode.Title}\"";
                }
                else if (NodeElement is Laboratory)
                {
                    LaboratoryNode = (Laboratory)NodeElement;
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

                foreach(Institution institution in universityStructure.Institutions)
                {
                    UniversityStructureTreeNode institutionNode = new UniversityStructureTreeNode(institution);
                    rootNode.Children.Add(institutionNode);

                    foreach(Unit unit in institution.Units)
                    {
                        UniversityStructureTreeNode unitNode = new UniversityStructureTreeNode(unit);
                        institutionNode.Children.Add(unitNode);

                        foreach(Kafedra kafedra in unit.Kafedras)
                        {
                            UniversityStructureTreeNode kafedraNode = new UniversityStructureTreeNode(kafedra);
                            unitNode.Children.Add(kafedraNode);

                            foreach(Laboratory laboratory in kafedra.Laboratories)
                            {
                                UniversityStructureTreeNode laboratoryNode = new UniversityStructureTreeNode(laboratory);
                                kafedraNode.Children.Add(laboratoryNode);
                            }
                        }
                        foreach(Laboratory laboratory in unit.Laboratories)
                        {
                            UniversityStructureTreeNode laboratoryNode = new UniversityStructureTreeNode(laboratory);
                            unitNode.Children.Add(laboratoryNode);
                        }
                    }
                }

                return rootNode;
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


        public UniversityStructureViewModel()
        {
            // Получение структуры бд
            CRUDDataBase.ConnectByDataBase();
            UniversityStructure = CRUDDataBase.GetUniversityStructure();
            CRUDDataBase.CloseConnect();

            RootNode = new ObservableCollection<UniversityStructureTreeNode>() { UniversityStructureTreeNode.ConvertUsualStructureToTreeElement(UniversityStructure) };
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
