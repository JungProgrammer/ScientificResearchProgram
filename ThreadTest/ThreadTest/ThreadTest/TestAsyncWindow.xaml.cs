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

namespace ThreadTest
{
    /// <summary>
    /// Логика взаимодействия для TestAsyncWindow.xaml
    /// </summary>
    public partial class TestAsyncWindow : Window, INotifyPropertyChanged
    {
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }


        public TestAsyncWindow()
        {
            InitializeComponent();


            ChangeTextAsync();


            DataContext = this;
        }


        private async void ChangeTextAsync()
        {
            await Task.Run(() => ChangeText());
        }

        private async void ChangeText()
        {
            for(int i = 0; i < 1000000; i++)
            {
                await Task.Delay(1);

                Dispatcher.Invoke(() => Value = i);

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (long j = 0; j < 1000000; j++)
            {
                textBlock2.Text = j.ToString();

            }
        }





        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
