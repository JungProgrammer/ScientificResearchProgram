using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ThreadTest
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }


        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }


        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> _lolList { get; set; }
        public ObservableCollection<int> LolList
        {
            get => _lolList;
            set
            {
                if (value == _lolList) return;
                _lolList = value;
                OnPropertyChanged();
            }
        }


        private Thread _thread;
        private CancellationTokenSource _tokenSource;

        public MainWindowViewModel()
        {
            StartCommand = new DelegateCommand((p) =>
            {
                _tokenSource = new CancellationTokenSource();
                _thread = new Thread(Worker) { IsBackground = true };
                _thread.Start(_tokenSource.Token);
            },
                p => _thread == null);

            StopCommand = new DelegateCommand(p =>
            {
                _tokenSource.Cancel();
                _tokenSource = null;
                _thread = null;
            }, p => _thread != null);


            LolList = new ObservableCollection<int>() { 1, 2, 3, 4 };
            LolList = new ObservableCollection<int>() { 4, 10, 3 };
        }

        private void Worker(object state)
        {
            var token = (CancellationToken)state;
            while (!token.IsCancellationRequested)
            {
                Value++;
                LolList = new ObservableCollection<int>() { 5, 6, 7, 8 };
                Thread.Sleep(1000);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
