using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResearchProgram
{
    public class GrantHeader : INotifyPropertyChanged
    {
        private string _nameOnRussia;
        public string nameOnRussia
        {
            get => _nameOnRussia;
            set
            {
                _nameOnRussia = value;
                OnPropertyChanged(nameof(nameOnRussia));
            }
        }
        public string nameForElement { get; set; }

        // Нужен вывод комбобокса
        public bool Is_combobox_needed { get; set; }
        // Данные для ItemSource этого комбобокса
        public List<IContainer> DataToComboBox { get; set; }
        // Выбранное значение для этого комбобокса
        public IContainer ChooseDataFromCombobox { get; set; }


        // Нужен вывод текстбокса
        public bool Is_textbox_needed { get; set; }
        // Текст для текстбоксового поля
        public string ChooseDataFromTextBox { get; set; }


        // Нужен вывод сравнимого выражения
        public bool Is_comparison_needed { get; set; }
        // Выбранный знак сравнения
        public string ChooseComparisonSign { get; set; }
        // Выбранная общая сумма
        public string ChooseAllSum { get; set; }


        // Нужен вывод датапикера
        public bool Is_date_needed { get; set; }
        // Выбранная дата
        public DateTime ChooseDateFromDatePicker { get; set; }


        //public ObservableCollection<FilterElement> FilterElementsData { get; set; }

        //private FilterElement _selectedFilter;
        //public FilterElement SelectedFilter
        //{
        //    get { return _selectedFilter; }
        //    set
        //    {
        //        _selectedFilter = value;
        //        OnPropertyChanged(nameof(SelectedFilter));
        //    }
        //}


        public GrantHeader()
        {
            nameOnRussia = "";
            nameForElement = "";
            //FilterElementsData = new ObservableCollection<FilterElement>();
            DataToComboBox = new List<IContainer>();
            ChooseDateFromDatePicker = DateTime.Now;
        }


        public override string ToString()
        {
            return nameOnRussia;
        }

        // Реализация интерфейса
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}