﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ResearchProgram.Forms.HelpWindows
{
    /// <summary>
    /// Логика взаимодействия для EditElementWindow.xaml
    /// </summary>
    public partial class EditElementWindow : Window, INotifyPropertyChanged
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

        private string _salary;
        public string Salary
        {
            get => _salary;
            set
            {
                _salary = value;
                OnPropertyChanged(nameof(Salary));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public EditElementWindow(string oldNameOfElement, string typeWindow, string salary = null)
        {
            InitializeComponent();

            NewNameOfElement = oldNameOfElement;

            TypeWindow = typeWindow;

            Salary = salary;

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
