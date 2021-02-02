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
        public class DepositorSum
        {
            public string Sum { get; set; }
        }

        public class RecieptDate
        {
            public string Date { get; set; }
        }

        public ObservableCollection<DepositorSum> depositorSums;
        public ObservableCollection<DepositorSum> depositorSumsNoNDS;

        public ObservableCollection<RecieptDate> recieptDates;


        public FullGrantInfo(Grant grant)
        {
            InitializeComponent();

            SetStartViewModelSettings(grant);

            CopyDepositors(grant);
            CopyRecieptDates(grant);

            DataContext = grant;

            SetDataContexts();
        }

        private void SetStartViewModelSettings(Grant grant)
        {
            grant.Depositor.Insert(0, new Depositor() { Title = "Источники:" });
        }

        private void CopyRecieptDates(Grant grant)
        {
            recieptDates = new ObservableCollection<RecieptDate>();

            recieptDates.Add(new RecieptDate() { Date = "Даты поступления:" });

            foreach(string recieptDate in grant.ReceiptDate)
            {
                recieptDates.Add(new RecieptDate() { Date = recieptDate });
            }
        }

        private void CopyDepositors(Grant grant)
        {
            depositorSums = new ObservableCollection<DepositorSum>();
            depositorSumsNoNDS = new ObservableCollection<DepositorSum>();

            depositorSums.Add(new DepositorSum() { Sum = "Сумма:" });
            depositorSumsNoNDS.Add(new DepositorSum() { Sum = "Сумма:" });

            foreach (float depositorSum in grant.DepositorSum)
            {
                depositorSums.Add(new DepositorSum() { Sum = depositorSum.ToString() });
            }

            foreach (float depositorSumNoNDS in grant.DepositorSumNoNDS)
            {
                depositorSumsNoNDS.Add(new DepositorSum() { Sum = depositorSumNoNDS.ToString() });
            }
        }

        private void SetDataContexts()
        {
            // Установка привзяки для сумм
            if (Settings.Default.NDSKey)
            {
                DepositorsSumsControl.ItemsSource = depositorSums;
            }
            else
            {
                DepositorsSumsControl.ItemsSource = depositorSumsNoNDS;
            }

            // Установка привязки для дат
            RecieptDatesControl.ItemsSource = recieptDates;
        }

    }
}
