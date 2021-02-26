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

namespace ResearchProgram.Forms.HelpWindows
{
    /// <summary>
    /// Логика взаимодействия для EditElementWindow.xaml
    /// </summary>
    public partial class EditElementWindow : Window, INotifyPropertyChanged
    {

        private string _newNameOfElement;
        public string NewNameOfElement
        {
            get => _newNameOfElement;
            set
            {
                _newNameOfElement = value;
                OnPropertyChanged(nameof(NewNameOfElement));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public EditElementWindow(string oldNameOfElement)
        {
            InitializeComponent();

            NewNameOfElement = oldNameOfElement;

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
