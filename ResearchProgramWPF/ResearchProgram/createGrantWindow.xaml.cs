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
        public List<string> NIOKRList { get; set; }
        public List<string> personsList { get; set; }
        public List<string> depositsList { get; set; }
        public List<string> scienceTypeList { get; set; }
        public List<string> kafedrasList { get; set; }
        public List<string> unitsList { get; set; }
        public List<string> instituionsList { get; set; }
        public List<string> researchTypesList { get; set; }

        public createGrantWindow()
        {
            InitializeComponent();
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

            DataContext = this;
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



            // if datasource is a DataView, then apply RowFilter as below and replace above logic with below one
            /* 
             DataView view = (DataView) Cmb.ItemsSource; 
             view.RowFilter = ("Name like '*" + Cmb.Text + "*'"); 
            */
        }
    }
}
