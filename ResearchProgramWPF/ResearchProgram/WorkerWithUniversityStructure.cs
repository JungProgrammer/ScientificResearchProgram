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
    }
}
