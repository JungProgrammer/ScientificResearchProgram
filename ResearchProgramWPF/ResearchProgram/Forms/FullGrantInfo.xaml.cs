using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class FullGrantInfo : Window
    {
        public class DepositorSum: INotifyPropertyChanged
        {
            private float _sum;
            public float sum
            {
                get => _sum;
                set
                {
                    _sum = value;
                    OnPropertyChanged(nameof(sum));
                }
            }

            public string sum1 = "21312";

            public override string ToString()
            {
                return sum.ToString();
            }


            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public ObservableCollection<DepositorSum> depositorSums;
        public ObservableCollection<DepositorSum> depositorSumsNoNDS;


        public FullGrantInfo(Grant grant)
        {
            InitializeComponent();

            CopyDepositors(grant);

            //DataContext = this;
            DepositorsSumsControl.ItemsSource = depositorSums;
        }


        private void CopyDepositors(Grant grant)
        {
            depositorSums = new ObservableCollection<DepositorSum>();
            depositorSumsNoNDS = new ObservableCollection<DepositorSum>();

            foreach (float depositorSum in grant.DepositorSum)
            {
                depositorSums.Add(new DepositorSum() { sum = depositorSum });
            }

            foreach (float depositorSumNoNDS in grant.DepositorSumNoNDS)
            {
                depositorSumsNoNDS.Add(new DepositorSum() { sum = depositorSumNoNDS });
            }
        }

        private void SetDataContextDepositors()
        {
            DepositorsSumsControl.DataContext = depositorSums;
        }

    }
}
