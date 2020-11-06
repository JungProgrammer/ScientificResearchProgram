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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResearchProgram
{
    public class Grant
    {
        // Id
        public int Id { get; set; }
        // ОКВЕД
        public string OKVED { get; set; }
        // Наименование НИОКР
        public string NameNIOKR { get; set; }
        // Заказчик
        public string Customer { get; set; }
        // Дата начала договора
        public DateTime StartDate { get; set; }
        // Дата окончания договора
        public DateTime EndDate { get; set; }
        // Цена договора
        public float Price { get; set; }
        // Средства
        public string Depositor { get; set; }
        // руководитель
        public string LeadNIOKR { get; set; }
        // Исполнитель
        public string Executor { get; set; }
        // Кафедра
        public string Kafedra { get; set; }
        // Подразделение
        public string Unit { get; set; }
        // ГРНТИ
        public string GRNTI { get; set; }
        // Тип исследования
        public string ResearchType { get; set; }
        // Приоритетные направления
        public string PriorityTrand { get; set; }
        // Исполнители по договору
        public string ExecutorContract { get; set; }
        // Тип науки
        public string ScienceType { get; set; }
        // НИР или услуга
        public string NIR { get; set; }
        // НОЦ
        public string NOC { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();


            DataGrid dg = new DataGrid();
            dg.Name = "testName1";

            // Создание столбцов
            DataGridTextColumn textcol = new DataGridTextColumn();
            Binding b = new Binding("Lol kek cheburek");
            textcol.Binding = b;
            textcol.Header = "textcol1";
            dg.Columns.Add(textcol);
            tabItem1.Content = dg;

        }


    }
}
