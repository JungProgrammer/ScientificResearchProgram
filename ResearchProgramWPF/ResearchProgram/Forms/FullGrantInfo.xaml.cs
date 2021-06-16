using ResearchProgram.Classes;
using System;
using System.Collections.ObjectModel;
using System.Windows;

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

            SetDataContexts(grant);
        }

        private void SetStartViewModelSettings(Grant grant)
        {
            grant.Depositors.Insert(0, new GrantDepositor() { Depositor = new Depositor() { Title = "Источники:" }});
        }

        private void CopyRecieptDates(Grant grant)
        {
            recieptDates = new ObservableCollection<RecieptDate>();

            recieptDates.Add(new RecieptDate() { Date = "Даты поступления:" });

            foreach (GrantDepositor depositor in grant.Depositors)
            {
                recieptDates.Add(new RecieptDate() { Date = depositor.RecieptDate?.ToString("dd.MM.yyyy") });
            }
        }

        private void CopyDepositors(Grant grant)
        {
            depositorSums = new ObservableCollection<DepositorSum>();
            depositorSumsNoNDS = new ObservableCollection<DepositorSum>();

            depositorSums.Add(new DepositorSum() { Sum = "Сумма:" });
            depositorSumsNoNDS.Add(new DepositorSum() { Sum = "Сумма:" });

            foreach (GrantDepositor depositor in grant.Depositors)
            {
                depositorSums.Add(new DepositorSum() { Sum = depositor.Sum.ToString() });
                depositorSumsNoNDS.Add(new DepositorSum() { Sum = depositor.SumNoNds.ToString() });
            }
        }

        private void SetDataContexts(Grant grant)
        {
            // Установка привзяки для сумм
            if (!grant.isWIthNDS && Settings.Default.NDSKey || !Settings.Default.NDSKey)
            {
                DepositorsSumsControl.ItemsSource = depositorSumsNoNDS;
            }
            else
            {
                DepositorsSumsControl.ItemsSource = depositorSums;
            }

            // Установка привязки для дат
            RecieptDatesControl.ItemsSource = recieptDates;
        }

    }
}
