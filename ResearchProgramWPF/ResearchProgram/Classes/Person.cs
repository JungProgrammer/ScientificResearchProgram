using ResearchProgram.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Person : IContainer
    {
        public struct WorkPlace
        {
            public PlaceOfWork placeOfWork;
            public WorkCategories workCategory;
            public List<Job> jobList;
            public int Id;
        }
        public Person()
        {
            Id = 0;
            FIO = "";
            BitrhDate = DateTime.MinValue;
            Sex = true;
            Degree = new WorkDegree();
            Rank = new WorkRank();
            workPlaces = new List<WorkPlace>();
        }

        public int Id { get; set; }
        public string FIO { get; set; }
        public DateTime BitrhDate { get; set; }
        public bool Sex { get; set; }
        public WorkDegree Degree { get; set; }
        public WorkRank Rank { get; set; }
        public List<WorkPlace> workPlaces { get; set; }


        public string GetTitle()
        {
            return FIO;
        }

        public override string ToString()
        {
            return FIO;
        }

        /// <summary>
        /// Возвращает имя в формате Фамилия И.О.
        /// </summary>
        /// <returns></returns>
        public string shortName()
        {
            string[] FIODivided = FIO.Split(' ');
            string shortName;

            // Если у нас корректно заполнено ФИО
            if (FIODivided.Length == 3)
            {
                string lastName = FIODivided[0];
                string firstName = FIODivided[1];
                string patronymic = FIODivided[2];

                shortName = lastName + " " + firstName[0] + ". " + patronymic[0] + ".";
            }
            else
            {
                shortName = FIO;
            }
            
            return shortName;
        }
    }
}
