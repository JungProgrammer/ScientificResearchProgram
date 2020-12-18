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
    /// <summary>
    /// Предоставляет доступ ко всей структуре университета.
    /// Связывает внути себя:
    /// Учреждение -> Подразделение -> Кафедра -> Лаборатория
    /// либо Учреждение -> Подразделение -> Лаборатория
    /// </summary>
    public class WorkerWithUniversityStructure: INotifyPropertyChanged
    {

        private Institution selectedInstitution;
        public Institution SelectedInstitution
        {
            get => selectedInstitution;
            set
            {
                selectedInstitution = value;
                OnPropertyChanged(nameof(SelectedInstitution));
            }
        }

        private Unit selectedUnit;
        public Unit SelectedUnit
        {
            get => selectedUnit;
            set
            {
                selectedUnit = value;
                OnPropertyChanged(nameof(SelectedUnit));
            }
        }

        private Kafedra selectedKafedra;
        public Kafedra SelectedKafedra
        {
            get => selectedKafedra;
            set
            {
                selectedKafedra = value;
                OnPropertyChanged(nameof(SelectedKafedra));
            }
        }

        private Laboratory selectedLaboratory;
        public Laboratory SelectedLaboratory
        {
            get => selectedLaboratory;
            set
            {
                selectedLaboratory = value;
                OnPropertyChanged(nameof(SelectedLaboratory));
            }
        }

        public ObservableCollection<Institution> Institutions { get; set; }

        public WorkerWithUniversityStructure()
        {
            Institutions = new ObservableCollection<Institution>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public Institution FindInstitution(int institutionId)
        {
            Institution institutionFound = null;

            foreach(Institution institution in Institutions)
            {
                if (institution.Id == institutionId) institutionFound = institution;
            }

            return institutionFound;
        }

        public Unit FindUnit(Institution institution, int unitId)
        {
            Unit unitFound = null;

            foreach (Unit unit in institution.Units)
            {
                if (unit.Id == unitId) unitFound = unit;
            }

            return unitFound;
        }

        public Kafedra FindKafedra(Unit unitFound, int kafedraId)
        {
            Kafedra kafedraFound = null;

            foreach(Kafedra kafedra in unitFound.Kafedras)
            {
                if (kafedra.Id == kafedraId) kafedraFound = kafedra;
            }

            return kafedraFound;
        }

        public Laboratory FindLaboratoryInKafedra(Kafedra kafedraFound, int laboratoryId)
        {
            Laboratory laboratoryFound = null;

            foreach(Laboratory laboratory in kafedraFound.Laboratories)
            {
                if (laboratory.Id == laboratoryId) laboratoryFound = laboratory;
            }

            return laboratoryFound;
        }


        public Laboratory FindLaboratoryInUnit(Unit unitFound, int laboratoryId)
        {
            Laboratory laboratoryFound = null;

            foreach (Laboratory laboratory in unitFound.Laboratories)
            {
                if (laboratory.Id == laboratoryId) laboratoryFound = laboratory;
            }

            return laboratoryFound;
        }


        public static Institution CreateNewInstitution(string institutionTitle)
        {
            Institution newInstitution = null;

            CRUDDataBase.ConnectByDataBase();
            newInstitution = CRUDDataBase.AddNewInstitution(institutionTitle);
            CRUDDataBase.CloseConnect();

            return newInstitution;
        }

        public static Unit CreateNewUnit(string unitTitle, Institution parent)
        {
            Unit newUnit = null;

            CRUDDataBase.ConnectByDataBase();
            newUnit = CRUDDataBase.AddNewUnit(unitTitle, parent.Id);
            CRUDDataBase.CloseConnect();

            return newUnit;
        }

        public static Kafedra CreateNewKafedra(string kafedraTitle, Unit parent)
        {
            Kafedra newKafedra = null;

            CRUDDataBase.ConnectByDataBase();
            newKafedra = CRUDDataBase.AddNewKafedra(kafedraTitle, parent.Id);
            CRUDDataBase.CloseConnect();

            return newKafedra;
        }

        /// <summary>
        /// Создание лаборатории для подразделения
        /// </summary>
        /// <param name="laboratoryTitle"></param>
        /// <returns></returns>
        public static Laboratory CreateNewLaboratory(string laboratoryTitle, Unit parent)
        {
            Laboratory newLaboratory = null;

            CRUDDataBase.ConnectByDataBase();
            newLaboratory = CRUDDataBase.AddNewLaboratoryToUnit(laboratoryTitle, parent.Id);
            CRUDDataBase.CloseConnect();

            return newLaboratory;
        }

        /// <summary>
        /// Создание лаборатории для кафедры
        /// </summary>
        /// <param name="laboratoryTitle"></param>
        /// <returns></returns>
        public static Laboratory CreateNewLaboratory(string laboratoryTitle, Kafedra parent)
        {
            Laboratory newLaboratory = null;

            CRUDDataBase.ConnectByDataBase();
            newLaboratory = CRUDDataBase.AddNewLaboratoryToKafedra(laboratoryTitle, parent.Id);
            CRUDDataBase.CloseConnect();

            return newLaboratory;
        }

        internal static void RenameInstitution(Institution institutionNode, string newTitle)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.RenameInstitution(institutionNode, newTitle);
            CRUDDataBase.CloseConnect();
        }

        internal static void RenameUnit(Unit unitNode, string newTitle)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.RenameUnit(unitNode, newTitle);
            CRUDDataBase.CloseConnect();
        }

        internal static void RenameKafedra(Kafedra kafedraNode, string newTitle)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.RenameKafedra(kafedraNode, newTitle);
            CRUDDataBase.CloseConnect();
        }

        internal static void RenameLaboratory(Laboratory laboratoryNode, string newTitle)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.RenameLaboratory(laboratoryNode, newTitle);
            CRUDDataBase.CloseConnect();
        }

        internal static void DeleteInstitution(Institution institutionNode)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.DeleteInstitution(institutionNode.Id);
            CRUDDataBase.CloseConnect();
        }

        internal static void DeleteUnit(Unit unitNode)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.DeleteUnit(unitNode.Id);
            CRUDDataBase.CloseConnect();
        }

        internal static void DeleteKafedra(Kafedra kafedraNode)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.DeleteKafedra(kafedraNode.Id);
            CRUDDataBase.CloseConnect();
        }

        internal static void DeleteLaboratory(Laboratory laboratoryNode)
        {
            CRUDDataBase.ConnectByDataBase();
            CRUDDataBase.DeleteLaboratory(laboratoryNode.Id);
            CRUDDataBase.CloseConnect();
        }
    }
}
