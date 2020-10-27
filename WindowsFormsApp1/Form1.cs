using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();

            InputTestData();

            SettingContextMenuForTable();
        }

        /// <summary>
        /// Скрытие выделенных элементов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideSelectedColomns(object sender, EventArgs e)
        {
            foreach(DataGridViewCell cell in mainDataGrid.SelectedCells)
            {
                mainDataGrid.Columns[cell.ColumnIndex].Visible = false;
            }
        }

        /// <summary>
        /// Показ всех скрытых столбцов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAllColumns(object sender, EventArgs e)
        {
            for (int i = 0; i < mainDataGrid.Columns.Count; i++)
            {
                mainDataGrid.Columns[i].Visible = true;
            }
        }

        /// <summary>
        /// Инициализация и настройка контекстного меню для dataGridView
        /// </summary>
        private void SettingContextMenuForTable()
        {
            ToolStripMenuItem hideSelectedColomnsMenuItem = new ToolStripMenuItem("Скрыть выделенные столбцы");
            ToolStripMenuItem showColomnsMenuItem = new ToolStripMenuItem("Показать скрытые столбцы");
            contextMenuStripForMainDataGrid.Items.AddRange(new[] { showColomnsMenuItem, hideSelectedColomnsMenuItem });
            mainDataGrid.ContextMenuStrip = contextMenuStripForMainDataGrid;
            peopleDataGridView.ContextMenuStrip = contextMenuStripForMainDataGrid;
            hideSelectedColomnsMenuItem.Click += HideSelectedColomns;
            showColomnsMenuItem.Click += ShowAllColumns;
        }

        /// <summary>
        /// Создание окна с фильтрами
        /// </summary>
        private void ShowFiltersForm()
        {
            FiltersForm filtersForm = new FiltersForm();
            filtersForm.Show();
        }

        /// <summary>
        /// Ввод тестовых данных (для разработки)
        /// </summary>
        private void InputTestData()
        {
            mainDataGrid.Rows[0].Cells[0].Value = "72.19";
            mainDataGrid.Rows[0].Cells[1].Value = "Наименование";
            mainDataGrid.Rows[0].Cells[2].Value = "Иванов Иван Иванович";
            mainDataGrid.Rows[0].Cells[3].Value = "10.10.1010";
            mainDataGrid.Rows[0].Cells[4].Value = "20.20.2020";
            mainDataGrid.Rows[0].Cells[5].Value = "373";
            mainDataGrid.Rows[0].Cells[6].Value = "средства Российских фондов поддержки науки" + Environment.NewLine + "Физ. лица";

            mainDataGrid.Rows[0].Cells[7].Value = "123" + Environment.NewLine + "250";

            mainDataGrid.Rows[0].Cells[8].Value = "Петров Пётр Петрович";
            string names = "Сидоров Сидор Сидорович" + Environment.NewLine + "Михайлов Михаил Михайлович";
            mainDataGrid.Rows[0].Cells[9].Value = names;
            mainDataGrid.Rows[0].Cells[10].Value = "ИБиСС";
            mainDataGrid.Rows[0].Cells[11].Value = "Механико-математический факультет";
            mainDataGrid.Rows[0].Cells[12].Value = "ПГНИУ";
            mainDataGrid.Rows[0].Cells[13].Value = "25.231.2";
            mainDataGrid.Rows[0].Cells[14].Value = "Прикладные исследования";
            mainDataGrid.Rows[0].Cells[15].Value = "Индустрия наносистем\nНауки о жизни";
            mainDataGrid.Rows[0].Cells[16].Value = "Дмитриев Дмитрий Дмитриевич";
            mainDataGrid.Rows[0].Cells[17].Value = "Науки об обществе";
            mainDataGrid.Rows[0].Cells[18].Value = "Услуга";
            mainDataGrid.Rows[0].Cells[19].Value = "НОЦ";



            int rowId = mainDataGrid.Rows.Add();
            DataGridViewRow row = mainDataGrid.Rows[rowId];
            row.Cells[0].Value = "55.19";
            row.Cells[1].Value = "Что-то умное";
            row.Cells[2].Value = "Кац Артем Адамович";
            row.Cells[3].Value = "15.1.1234";
            row.Cells[4].Value = "1.2.4567";
            row.Cells[5].Value = "7894";
            row.Cells[6].Value = "физ лица";
            row.Cells[7].Value = "7894";
            row.Cells[8].Value = "Логутенко Герасим Ефремович";
            row.Cells[9].Value = "Кошельков Эрнст Эмилевич";
            row.Cells[10].Value = "ЛАПГ";
            row.Cells[11].Value = "Химический факультет";
            row.Cells[12].Value = "ПГНИУ";
            row.Cells[13].Value = "111.2.2";
            row.Cells[14].Value = "Прикладные исследования";
            row.Cells[15].Value = "Рациональное природопользование";
            row.Cells[16].Value = "Серебряков Онисим Викентиевич";
            row.Cells[17].Value = "Гуманитарные науки";
            row.Cells[18].Value = "Услуга";
            row.Cells[19].Value = "НОЦ";

            rowId = mainDataGrid.Rows.Add();
            row = mainDataGrid.Rows[rowId];
            row.Cells[0].Value = "76.54";
            row.Cells[1].Value = "Птица";
            row.Cells[2].Value = "Лесков Еремей Ипатович";
            row.Cells[3].Value = "31.12.5678";
            row.Cells[4].Value = "14.9.2020";
            row.Cells[5].Value = "7564394.12";
            row.Cells[6].Value = "средства Российских фондов поддержки науки";
            row.Cells[7].Value = "7564394.12";
            row.Cells[8].Value = "Сёмин Валентин Ростиславович";
            names = "Мукосеев Сергей Владимирович" + Environment.NewLine + "Потапов Моисей Тарасович" + Environment.NewLine + "Содовский Степан Якубович";
            row.Cells[9].Value = names;
            row.Cells[10].Value = "ОЛДЫ";
            row.Cells[11].Value = "Историко-политологический факультет";
            row.Cells[12].Value = "ПГНИУ";
            row.Cells[13].Value = "1.2.34";
            row.Cells[14].Value = "Поисковые исследования";
            row.Cells[15].Value = "Транспортные и космические системы";
            row.Cells[16].Value = "Черенчиков Кондрат Святославович";
            row.Cells[17].Value = "Военные науки";
            row.Cells[18].Value = "Услуга";
            row.Cells[19].Value = "НОЦ";

            rowId = mainDataGrid.Rows.Add();
            row = mainDataGrid.Rows[rowId];
            row.Cells[0].Value = "1.54";
            row.Cells[1].Value = "Текст";
            row.Cells[2].Value = "Немцев Нестор Владиславович";
            row.Cells[3].Value = "31.12.5678";
            row.Cells[4].Value = "14.9.2020";
            row.Cells[5].Value = "555";
            row.Cells[6].Value = "средства Российских фондов поддержки науки";
            row.Cells[7].Value = "555";
            row.Cells[8].Value = "Швечиков Даниил Никонович";
            names = "Яшагин Серафим Назарович" + Environment.NewLine + "nШиронин Рюрик Михеевич";
            row.Cells[9].Value = names;
            row.Cells[10].Value = "ВКХНР";
            row.Cells[11].Value = "Горно-нефтяной факультет";
            row.Cells[12].Value = "ПНИПУ";
            row.Cells[13].Value = "13.22.11";
            row.Cells[14].Value = "Прикладные исследования";
            row.Cells[15].Value = "Безопасность и противодействие терроризму";
            row.Cells[16].Value = "Тукай Тимофей Прокофиевич";
            row.Cells[17].Value = "Образование и педагогические науки";
            row.Cells[18].Value = "НИР";
            row.Cells[19].Value = "НОЦ";

            rowId = mainDataGrid.Rows.Add();
            row = mainDataGrid.Rows[rowId];
            row.Cells[0].Value = "56.54";
            row.Cells[1].Value = "Самолёт";
            row.Cells[2].Value = "Брынских Демьян Сократович";
            row.Cells[3].Value = "2.12.1230";
            row.Cells[4].Value = "14.9.8658";
            row.Cells[5].Value = "34536";
            row.Cells[6].Value = "средства хозяйствующих субъектов";
            row.Cells[7].Value = "34536";
            row.Cells[8].Value = "Мячин Филимон Тимурович";
            names = "Авилов Ерофей Леонидович" + Environment.NewLine + "Романенко Александр Самсонович";
            row.Cells[9].Value = names;
            row.Cells[10].Value = "ГАЧИ";
            row.Cells[11].Value = "Географический факультет";
            row.Cells[12].Value = "ПГНИУ";
            row.Cells[13].Value = "15.5.300";
            row.Cells[14].Value = "Фундаментальные исследования";
            row.Cells[15].Value = "Рациональное природопользование";
            row.Cells[16].Value = "Якубенко Адам Денисович";
            row.Cells[17].Value = "Здравоохранение и медицинские науки";
            row.Cells[18].Value = "НИР";
            row.Cells[19].Value = "НОЦ";




            rowId = peopleDataGridView.Rows.Add();
            row = peopleDataGridView.Rows[rowId];
            row.Cells[0].Value = "Брынских Д.С.";
            row.Cells[1].Value = "12.09.1990";
            row.Cells[2].Value = "Мужской";
            string works = "Профессор" + Environment.NewLine + "Инженер";
            row.Cells[3].Value = works;
            string salaryRates = "1.0" + Environment.NewLine + "0.35";
            row.Cells[4].Value = salaryRates;
            string Jobs = "Профессор гуманитарных наук" + Environment.NewLine + "Главный инженер";
            row.Cells[5].Value = Jobs;
            row.Cells[6].Value = "ППС";
            row.Cells[7].Value = "Доктор наук";
            row.Cells[8].Value = "Доцент";
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void mainDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            mainDataGrid.AutoResizeColumns();
            mainDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            mainDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            peopleDataGridView.AutoResizeColumns();
            peopleDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            peopleDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void filtersButton_Click(object sender, EventArgs e)
        {
            ShowFiltersForm();
        }

        private void sdfToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void mainDataGrid_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void создатьДоговорToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Contract contractForm = new Contract();
            contractForm.Show();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings();
            settingsForm.Show();
        }
    }
}
