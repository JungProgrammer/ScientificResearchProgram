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
using System.Configuration;
namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

        }
        public bool isNDSCheckBoxChecked = Settings.Default.NDSKey;
        public bool IsNDSCheckBoxChecked
        {
            get { return isNDSCheckBoxChecked; }
            set { isNDSCheckBoxChecked = value; }
        }

        private void NDSCheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            Settings.Default.NDSKey = !Settings.Default.NDSKey;
            Settings.Default.Save();
            IsNDSCheckBoxChecked = Settings.Default.NDSKey;
        }
    }
}
