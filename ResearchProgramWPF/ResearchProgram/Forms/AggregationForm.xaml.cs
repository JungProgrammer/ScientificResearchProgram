using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using static ResearchProgram.Utilities;

namespace ResearchProgram.Forms
{
    public class DataGridData
    {
        private string _title;
        private int _count;
        private string _percent;
        public string Title { get { if (_title == "" || _title == null) return "Значения нет"; else return _title; } set { _title = value; ; } }

        public int Count { get => _count; set { _count = value; ; } }

        public string Percent { get => _percent; set { _percent = value; ; } }
    }

    public partial class AggregationForm : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private string _aggregationType;
        public string AggregationType { get { return _aggregationType; } set { _aggregationType = "Аггрегация по " + value; OnPropertyChanged(nameof(AggregationType)); } }

        private DataTable _aggregationDataTable;
        public DataTable AggregationDataTable { get => _aggregationDataTable; set { _aggregationDataTable = value; OnPropertyChanged(nameof(AggregationDataTable)); } }

        private ObservableCollection<DataGridData> _dataGridData;
        public ObservableCollection<DataGridData> DataGridData { get { return _dataGridData; } set { _dataGridData = value; OnPropertyChanged(nameof(DataGridData)); } }

        public AggregationForm(string aggType, Dictionary<bool?, int> boolDataDict, Dictionary<int, MappedValue> mappedDataDict, Dictionary<string, int> stringDataDict)
        {
            InitializeComponent();

            AggregationType = aggType;
            DataGridData = new ObservableCollection<DataGridData>();

            var ds = new DataSet("Aggregation");
            AggregationDataTable = ds.Tables.Add("AggregationTable");
            int count = 0;

            if (boolDataDict.Count > 0)
            { 
                count = boolDataDict.Values.Sum((x => x));
                foreach (bool key in boolDataDict.Keys)
                {
                    addRow(key ? "Да" : "Нет", boolDataDict[key], boolDataDict[key] * 1.0 / count * 100);
                }
            }
            else if (mappedDataDict.Count > 0)
            {
                count = mappedDataDict.Values.Sum((x => x.Count));
                foreach (int key in mappedDataDict.Keys)
                {
                    addRow(mappedDataDict[key].Title, mappedDataDict[key].Count, mappedDataDict[key].Count * 1.0 / count * 100);
                }
            }
            else
            {
                count = stringDataDict.Values.Sum((x => x));
                foreach (string key in stringDataDict.Keys)
                {
                    addRow(key, stringDataDict[key], stringDataDict[key] * 1.0 / count * 100);
                }
            }

            DataContext = this;
        }

        public void addRow(string title, int count, double percent)
        {
            DataGridData dataGridData = new DataGridData()
            {
                Title = title,
                Count = count,
                Percent = string.Format("{0:0.##}", percent)
            };
            DataGridData.Add(dataGridData);
        }
    }
}
