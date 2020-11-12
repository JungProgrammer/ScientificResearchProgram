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

        public override string ToString()
        {
            return FIO;
        }
    }
}
