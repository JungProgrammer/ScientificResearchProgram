using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Kafedra: IContainer
    {
        // Id кафедры
        public int Id { get; set; }

        // Название кафедры
        public string Title { get; set; }

        // Список лабораторий на этой кафедре
        public ObservableCollection<Laboratory> Laboratories { get; set; }

        public Kafedra()
        {
            //Title = "Не указан";
            Title = "";
            Laboratories = new ObservableCollection<Laboratory>();
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
