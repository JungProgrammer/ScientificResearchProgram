using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Person
    {
        public int Id { get; set; }

        public string FIO { get; set; }
        public string BitrhDate { get; set; }
        public bool Sex { get; set; }
        public string PlaceOfWork { get; set; }
        public string Category { get; set; }
        public string Degree { get; set; }
        public string Rank { get; set; }
        public List<Job> Jobs { get; set; }


        public Person()
        {
            Jobs = new List<Job>();
        }

        public override string ToString()
        {
            return FIO;
        }
    }
}
