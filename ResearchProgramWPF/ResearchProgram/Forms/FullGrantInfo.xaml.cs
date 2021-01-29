using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для FullGrantInfo.xaml
    /// </summary>
    public partial class FullGrantInfo : Window, INotifyPropertyChanged
    {
        private string _okved;
        public string OKVED
        {
            get => _okved;
            set
            {
                _okved = value;
                OnPropertyChanged(nameof(OKVED));
            }
        }

        //private string _okved;
        //public string OKVED
        //{
        //    get => _okved;
        //    set
        //    {
        //        _okved = value;
        //        OnPropertyChanged(nameof(OKVED));
        //    }
        //}

        //private string _okved;
        //public string OKVED
        //{
        //    get => _okved;
        //    set
        //    {
        //        _okved = value;
        //        OnPropertyChanged(nameof(OKVED));
        //    }
        //}

        //private string _okved;
        //public string OKVED
        //{
        //    get => _okved;
        //    set
        //    {
        //        _okved = value;
        //        OnPropertyChanged(nameof(OKVED));
        //    }
        //}

        //private string _okved;
        //public string OKVED
        //{
        //    get => _okved;
        //    set
        //    {
        //        _okved = value;
        //        OnPropertyChanged(nameof(OKVED));
        //    }
        //}

        //private string _okved;
        //public string OKVED
        //{
        //    get => _okved;
        //    set
        //    {
        //        _okved = value;
        //        OnPropertyChanged(nameof(OKVED));
        //    }
        //}

        //private string _okved;
        //public string OKVED
        //{
        //    get => _okved;
        //    set
        //    {
        //        _okved = value;
        //        OnPropertyChanged(nameof(OKVED));
        //    }
        //}



        public FullGrantInfo(Grant grant)
        {
            InitializeComponent();

            DataContext = grant;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
