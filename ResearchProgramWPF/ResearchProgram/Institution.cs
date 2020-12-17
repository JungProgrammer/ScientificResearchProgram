using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Institution: IContainer
    {
        // Id учреждения
        public int Id { get; set; }

        // Название учреждения
        public string Title { get; set; }

        // Список подразделений
        public ObservableCollection<Unit> Units { get; set; }

        public Institution()
        {
            //Title = "Не указан";
            Title = "";
            Units = new ObservableCollection<Unit>();
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
