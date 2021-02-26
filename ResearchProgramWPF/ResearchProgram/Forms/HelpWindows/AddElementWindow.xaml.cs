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
    /// Логика взаимодействия для AddElementWindow.xaml
    /// </summary>
    public partial class AddElementWindow : Window, INotifyPropertyChanged
    {
        private string _typeWindow;
        public string TypeWindow
        {
            get => _typeWindow;
            set
            {
                _typeWindow = value;
                OnPropertyChanged(nameof(_typeWindow));
            }
        }

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

        private string _salaty;
        public string Salary
        {
            get => _salaty;
            set
            {
                _salaty = value;
                OnPropertyChanged(nameof(Salary));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public AddElementWindow(string typeWindow)
        {
            InitializeComponent();

            TypeWindow = typeWindow;

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
