using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResearchProgram.Classes
{
    public class UniversityStructureNode : INotifyPropertyChanged
    {
        private int id;
        private string address;
        private string title;
        private bool isMainWorkPlace;
        private string shortTitle;
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
            set
            {
                isMainWorkPlace = value;
                OnPropertyChanged("IsMainWorkPlace");
            }
        }

        public string ShortTitle
        {
            get { return shortTitle; }
            set
            {
                shortTitle = value;
                OnPropertyChanged("ShortTitle");
            }
        }

        public UniversityStructureNode()
        {
            IsMainWorkPlace = false;
            ShortTitle = "";
        }

        public override string ToString()
        {
            if (ShortTitle != "" && ShortTitle != null)
            {
                return ShortTitle;
            }
            else
            {
                return Title;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
