using ResearchProgram.Classes;
using System;
using System.Collections.Generic;

namespace ResearchProgram
{
    public class Person : IContainer
    {

        public Person()
        {
            Id = 0;
            FIO = "";
            BitrhDate = null;
            Sex = true;
            Degree = new WorkDegree();
            Rank = new WorkRank();
            workPlaces = new List<PersonWorkPlace>();
        }

        public int Id { get; set; }
        public string FIO { get; set; }
        public DateTime? BitrhDate { get; set; }
        public bool Sex { get; set; }
        public WorkDegree Degree { get; set; }
        public WorkRank Rank { get; set; }
        public List<PersonWorkPlace> workPlaces { get; set; }


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

        public PersonWorkPlace GetActiveWorkPlace()
        {
            if (workPlaces.Count == 0)
                return new PersonWorkPlace();

            foreach(PersonWorkPlace personWorkPlace in workPlaces)
            {
                if (personWorkPlace.IsMainWorkPlace)
                {
                    return personWorkPlace;
                }
            }
            return workPlaces[0];
        }
    }
}
