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
using DotNetKit.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using THE.Controls;

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
        public List<string> selectedItems { get; set; }
        public List<string> selectedValues { get; set; }

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
            ComboBox comboBoxSender = (ComboBox)sender;

            

            var itemSource = comboBoxSender.ItemsSource;
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(itemSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(comboBoxSender.Text.ToLower())) return true;
                else
                {
                    if ((((string)o).ToLower()).Contains(comboBoxSender.Text.ToLower())) return true;
                    else return false;
                }
            });
            itemsViewOriginal.Refresh();

            

            //ICollectionView filteredView = new CollectionViewSource { Source = scienceTypeList }.View;

            //filteredView.Filter = ((o) =>
            //{
            //    if (String.IsNullOrEmpty(comboBoxSender.Text.ToLower())) return true;
            //    else
            //    {
            //        if ((((string)o).ToLower()).Contains(comboBoxSender.Text.ToLower())) return true;
            //        else return false;
            //    }
            //});
            ////filteredView.Refresh();


            //comboBoxSender.ItemsSource = filteredView;
        }


        /// <summary>
        /// Добавление строки в средства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void depositsAddButton_Click_1(object sender, RoutedEventArgs e)
        {

            StackPanel horizontalStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            ComboBox depositorComboBox = new ComboBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                ItemsSource = depositsList,
                MinWidth = 240
                
            };

            TextBox sumTextBox = new TextBox()
            {
                Margin = new Thickness(5, 0, 5, 10),
                MinWidth = 90
            };

            horizontalStackPanel.Children.Add(depositorComboBox);
            horizontalStackPanel.Children.Add(sumTextBox);


            depositsVerticalListView.Items.Add(horizontalStackPanel);
        }

        /// <summary>
        /// Метод удаления строки из средств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void depositsDeleteButton_Click(object sender, RoutedEventArgs e)
        {

            int countSelectedElement = depositsVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    depositsVerticalListView.Items.Remove(depositsVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }

        private void executorAddButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBox executorComboBox = new ComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = personsList,
                IsTextSearchEnabled = false,
                IsEditable = true,
                StaysOpenOnEdit = true,
                MinWidth = 300
            };
            executorComboBox.KeyUp += Cmb_KeyUp;

            executorsVerticalListView.Items.Add(executorComboBox);
        }

        private void executorDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = executorsVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    executorsVerticalListView.Items.Remove(executorsVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }

        private void executorOnContractAddButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBox executorOnContractComboBox = new ComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = personsList,
                IsTextSearchEnabled = false,
                IsEditable = true,
                StaysOpenOnEdit = true,
                MinWidth = 300
            };
            executorOnContractComboBox.KeyUp += Cmb_KeyUp;

            executorOnContractVerticalListView.Items.Add(executorOnContractComboBox);
        }

        private void executorOnContractDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = executorOnContractVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    executorOnContractVerticalListView.Items.Remove(executorOnContractVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }

        private void scienceTypeAddButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteComboBox scienceTypeComboBox = new AutoCompleteComboBox()
            {
                Margin = new Thickness(5, 0, 5, 0),
                ItemsSource = new List<string>(scienceTypeList),
                MinWidth = 300
            };


            scienceTypeVerticalListView.Items.Add(scienceTypeComboBox);
        }

        private void scienceTypeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int countSelectedElement = scienceTypeVerticalListView.SelectedItems.Count;
            if (countSelectedElement > 0)
            {
                for (int i = 0; i < countSelectedElement; i++)
                {
                    scienceTypeVerticalListView.Items.Remove(scienceTypeVerticalListView.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выделите нужный для удаления элемент");
            }
        }
    }
}
