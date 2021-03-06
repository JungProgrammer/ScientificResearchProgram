using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram.Classes
{
    public class UniversityStructureNode : INotifyPropertyChanged
    {
        private int id;
        private string address;
        private string title;
        private bool isMainWorkPlace;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool IsMainWorkPlace
        {
            get { return isMainWorkPlace; }
            set { isMainWorkPlace = value;
                OnPropertyChanged("IsMainWorkPlace");
            }
        }

        public UniversityStructureNode()
        {
            IsMainWorkPlace = false;
        }

        public override string ToString()
        {
            return Title;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
