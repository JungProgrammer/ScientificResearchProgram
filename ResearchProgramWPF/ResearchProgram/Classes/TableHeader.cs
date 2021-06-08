using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram.Classes
{
    public class TableHeader
    {
        public string Title { get; set; }
        public bool IsMultiple { get; set; }
        public bool IsService { get; set; }

        public bool IsCountable { get; set; }

        public TableHeader()
        {
            IsCountable = false;
            IsMultiple = false;
            IsService = false;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
