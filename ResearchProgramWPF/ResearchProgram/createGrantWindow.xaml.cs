using System;
using System.Collections.Generic;
using System.Linq;
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
using Npgsql;

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для createGrantWindow.xaml
    /// </summary>
    public partial class createGrantWindow : Window
    {
        //Списки данных из БД
        public List<string> NIOKRList { get; set; }
        public List<string> personsList { get; set; }
        public List<string> depositsList { get; set; }
        public List<string> scienceTypeList { get; set; }
        public List<string> kafedrasList { get; set; }
        public List<string> unitsList { get; set; }
        public List<string> instituionsList { get; set; }
        public List<string> researchTypesList { get; set; }
        //Списки данных из формы
        public List<ComboBox> enteredExecutorsList { get; set; }
        public List<Object[]> enteredDepositsList { get; set; }
        public List<ComboBox> enteredExecutorsContractList { get; set; }
        public List<ComboBox> enteredScienceTypesList { get; set; }


        public createGrantWindow()
        {
            InitializeComponent();

            //Заполнение списков
            NIOKRList = new List<string>();
            NIOKRList.Add("19");
            NIOKRList.Add("20");

            CRUDDataBase.ConnectByDataBase();
            personsList = CRUDDataBase.GetPersons();
            depositsList = CRUDDataBase.GetDeposits();
            kafedrasList = CRUDDataBase.GetKafedras();
            unitsList = CRUDDataBase.GetUnits();
            instituionsList = CRUDDataBase.GetInstitutions();
            researchTypesList = CRUDDataBase.GetResearchTypes();
            scienceTypeList = CRUDDataBase.GetScienceTypes();

            enteredDepositsList = new List<object[]>();
            enteredExecutorsContractList = new List<ComboBox>();
            enteredScienceTypesList = new List<ComboBox>();
            enteredExecutorsList = new List<ComboBox>();

            /*Label lbl = new Label() { Content = "Средства", Margin = new Thickness(0, 5, 0, 0)};
            depositsGrid.Children.Add(lbl);
            Grid.SetRow(lbl, 0);
            Grid.SetColumn(lbl, 0);
            lbl = new Label() { Content = "Сумма", Margin = new Thickness(0, 5, 0, 0) };
            depositsGrid.Children.Add(lbl);
            Grid.SetRow(lbl, 0);
            Grid.SetColumn(lbl, 1);*/
            addExecutorDepositsOnForm();
            addExecutorOnContractOnForm();
            addExecutortOnForm();
            addScienceTypeForm();
            DataContext = this;
        }
        /// <summary>
        /// Динамическое добавление полей ввода на форму средств
        /// </summary>
        private void addExecutorOnContractOnForm()
        {
            ComboBox cmb = new ComboBox() { Margin = new Thickness(5, 0, 5, 0), ItemsSource = personsList, IsTextSearchEnabled = false, IsEditable = true, StaysOpenOnEdit = true, IsDropDownOpen = true };
            cmb.KeyUp += Cmb_KeyUp;
            executorsContractGrid.Children.Add(cmb);
            Grid.SetRow(cmb, 1);
            Grid.SetColumnSpan(cmb, 2);
            enteredExecutorsContractList.Add(cmb);
        }
        /// <summary>
        /// Динамическое добавление полей ввода на форму исполнителей
        /// </summary>
        private void addExecutortOnForm()
        {
            ComboBox cmb = new ComboBox() { Margin = new Thickness(5, 0, 5, 0), ItemsSource = personsList, IsTextSearchEnabled = false, IsEditable = true, StaysOpenOnEdit = true, IsDropDownOpen = true };
            cmb.KeyUp += Cmb_KeyUp;
            executorsGrid.Children.Add(cmb);
            Grid.SetRow(cmb, 1);
            Grid.SetColumnSpan(cmb, 2);
            enteredExecutorsList.Add(cmb);
        }
        /// <summary>
        /// Динамическое добавление полей ввода на форму исполнителей по договору
        /// </summary>
        private void addExecutorDepositsOnForm()
        {
            ComboBox cmb = new ComboBox() { Margin = new Thickness(5, 0, 5, 0), ItemsSource = depositsList };
            depositsGrid.Children.Add(cmb);
            Grid.SetRow(cmb, 1);
            Grid.SetColumn(cmb, 0);
            TextBox txt = new TextBox() { Margin = new Thickness(5, 0, 5, 0) };
            depositsGrid.Children.Add(txt);
            Grid.SetRow(txt, 1);
            Grid.SetColumn(txt, 1);
            enteredDepositsList.Add(new object[2] { cmb, txt });
        }
        /// <summary>
        /// Динамическое добавление полей ввода на форму типов исследования
        /// </summary>
        private void addScienceTypeForm()
        {
            ComboBox cmb = new ComboBox() { Margin = new Thickness(5, 0, 5, 0), ItemsSource = scienceTypeList, IsTextSearchEnabled = false, IsEditable = true, StaysOpenOnEdit = true, IsDropDownOpen = true };
            cmb.KeyUp += Cmb_KeyUp;
            scienceTypeGrid.Children.Add(cmb);
            Grid.SetRow(cmb, 1);
            Grid.SetColumnSpan(cmb, 2);
            enteredScienceTypesList.Add(cmb);
        }

        private void Cmb_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(((ComboBox)sender).ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(((ComboBox)sender).Text.ToLower())) return true;
                else
                {
                    if ((((string)o).ToLower()).Contains(((ComboBox)sender).Text.ToLower())) return true;
                    else return false;
                }
            });
            itemsViewOriginal.Refresh();
        }

        private void depositsAddButton_Click(object sender, RoutedEventArgs e)
        {

            depositsGrid.RowDefinitions.Add(new RowDefinition());

            ComboBox cmb = new ComboBox() { Margin = new Thickness(5, 0, 5, 0), ItemsSource = depositsList };
            depositsGrid.Children.Add(cmb);
            Grid.SetRow(cmb, enteredDepositsList.Count() + 1);
            Grid.SetColumn(cmb, 0);
            TextBox txt = new TextBox() { Margin = new Thickness(5, 0, 5, 0) };
            depositsGrid.Children.Add(txt);
            Grid.SetRow(txt, enteredDepositsList.Count() + 1);
            Grid.SetColumn(txt, 1);
            enteredDepositsList.Add(new object[2] { cmb, txt });
        }

        private void buttonAddExecutorOnContract_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
