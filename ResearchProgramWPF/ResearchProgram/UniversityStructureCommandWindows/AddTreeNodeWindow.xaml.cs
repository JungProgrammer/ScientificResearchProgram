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
    /// Логика взаимодействия для AddTreeNodeWindow.xaml
    /// </summary>
    public partial class AddTreeNodeWindow : Window, INotifyPropertyChanged
    {
        public string NameStructure { get; set; }

        public AddTreeNodeWindow(string showedNameStructure)
        {
            InitializeComponent();

            NameStructure = showedNameStructure;

            DataContext = this;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
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
