using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Unit: IContainer
    {
        // Id подразделения
        public int Id { get; set; }

        // Название подразделения
        public string Title { get; set; }

        // Список кафедр
        public ObservableCollection<Kafedra> Kafedras { get; set; }

        // Список лабораторий, которые принадлежат напрямую к факультету
        public ObservableCollection<Laboratory> Laboratories { get; set; }

        public Unit()
        {
            //Title = "Не указан";
            Title = "";
            Kafedras = new ObservableCollection<Kafedra>();
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
