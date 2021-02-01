using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Customer : IContainer
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Customer()
        {
            Title = "";
        }

        public string GetTitle()
        {
            return Title;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
