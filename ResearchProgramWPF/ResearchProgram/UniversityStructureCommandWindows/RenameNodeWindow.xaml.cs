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

namespace ResearchProgram.UniversityStructureCommandWindows
{
    /// <summary>
    /// Логика взаимодействия для RenameNodeWindow.xaml
    /// </summary>
    public partial class RenameNodeWindow : Window, INotifyPropertyChanged
    {
        public RenameNodeWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private string _title;
        public string StructureTitle
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(StructureTitle));
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
