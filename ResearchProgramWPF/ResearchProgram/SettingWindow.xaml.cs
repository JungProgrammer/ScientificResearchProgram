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
        public bool isNDSCheckBoxChecked;
        public bool IsNDSCheckBoxChecked {get; set;} = Settings.Default.NDSKey;

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.NDSKey = NDSCheckBox.IsChecked.Value;
            Settings.Default.Save();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (applyButton.IsEnabled)
            {
                MessageBoxResult close = MessageBox.Show("Введённые изменения не сохранятся. Вы хотите сохранить настройки?", "Настройки", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (close)
                {
                    case MessageBoxResult.Yes:
                        applyButton_Click(sender, e);
                        Close();
                        break;
                    case MessageBoxResult.No:
                        Close();
                        break;
                }
            }
            else {
                Close();
            }
        }

        private void NDSCheckBox_Click(object sender, RoutedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }
    }
}
