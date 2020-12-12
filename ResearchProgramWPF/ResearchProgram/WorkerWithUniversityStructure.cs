using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class WorkerWithUniversityStructure
    {

        public WorkerWithUniversityStructure()
        {
            Institutions = new ObservableCollection<Institution>();
        }

        public ObservableCollection<Institution> Institutions { get; set; }

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
