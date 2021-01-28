﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Person: IContainer
    {
        public Person()
        {
            Id = 0;
            FIO = "";
            BitrhDate = DateTime.MinValue;
            Sex = true;
            PlaceOfWork = "";
            Category = "";
            Degree = "";
            Rank = "";
            Jobs = new List<Job>();
        }

        public int Id { get; set; }

        public string FIO { get; set; }
        public DateTime BitrhDate { get; set; }
        public bool Sex { get; set; }
        public string PlaceOfWork { get; set; }
        public string Category { get; set; }
        public string Degree { get; set; }
        public string Rank { get; set; }
        public List<Job> Jobs { get; set; }

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