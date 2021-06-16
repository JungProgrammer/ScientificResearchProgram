using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram.Classes
{
    public class GrantDepositor
    {
        public Depositor Depositor { get; set; }

        public double Sum { get; set; }

        public double SumNoNds { get; set; }

        public DateTime? RecieptDate { get; set; }

    }
}
