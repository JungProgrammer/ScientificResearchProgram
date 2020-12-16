using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResearchProgram
{
    public class UniversityStructureWindowCommands
    {
        static UniversityStructureWindowCommands()
        {
            AddNode = new RoutedCommand("AddNode", typeof(UniversityStructureWindow));
        }

        public static RoutedCommand AddNode { get; set; }


    }
}
