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
    public partial class Contract : Form
    {
        const int COUNT_OF_PANELS = 19;
        Panel[] propertyPanels;

        public Contract()
        {
            InitializeComponent();
            SetFormSize();
            GetPanels();
        }

        /// <summary>
        /// Установка размера формы
        /// </summary>
        void SetFormSize()
        {
            Size = new System.Drawing.Size(745, 690);
        }

        /// <summary>
        /// Инициализация панелей
        /// </summary>
        void GetPanels()
        {
            foreach (var elem in this.Controls.OfType<Panel>())
            {
                if (elem.Tag.ToString() == "PropertyPanel")
                {
                    MessageBox.Show(elem.Tag.ToString());
                    elem.Parent = panel1;
                    elem.Location = defaultPanel.Location;
                    elem.Visible = false;
                }
            }
        }

        /// <summary>
        /// Сохранение текущей информаци для поля, которое сейчас редактировал пользователь
        /// </summary>
        void SaveCurData(string curElement)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string element = listOfPropertisInCreatingContract.SelectedItem.ToString();
            SaveCurData(element);

            switch (element)
            {
                case "Исполнители":
                    panel2.Visible = true;
                    panel3.Visible = false;
                    break;
                case "Дата начала":
                    panel3.Visible = true;
                    panel2.Visible = false;
                    break;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
