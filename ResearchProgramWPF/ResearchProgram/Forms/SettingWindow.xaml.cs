using System;
using System.Windows;
using System.Windows.Media;
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
        public event EventHandler ReloadGrantsTable;
        public bool isNDSCheckBoxChecked;
        public bool IsNDSCheckBoxChecked { get; set; } = Settings.Default.NDSKey;

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.NDSKey = NDSCheckBox.IsChecked.Value;
            Settings.Default.RowColor0 = colorPicker0.SelectedColor.Value.ToString();
            Settings.Default.RowColor1 = colorPicker1.SelectedColor.Value.ToString();
            Settings.Default.RowColor2 = colorPicker2.SelectedColor.Value.ToString();
            Settings.Default.RowColor3 = colorPicker3.SelectedColor.Value.ToString();
            Settings.Default.RowColor4 = colorPicker4.SelectedColor.Value.ToString();
            Settings.Default.RowColor5 = colorPicker5.SelectedColor.Value.ToString();
            Settings.Default.RowColor6 = colorPicker6.SelectedColor.Value.ToString();

            Settings.Default.Save();

            applyButton.IsEnabled = false;

            EventHandler handler = ReloadGrantsTable;
            handler(this, EventArgs.Empty);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (applyButton.IsEnabled)
            {
                MessageBoxResult close = MessageBox.Show("Введённые изменения не сохранятся. Вы хотите сохранить настройки?", "Настройки", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (close)
                {
                    case MessageBoxResult.Yes:
                        ApplyButton_Click(sender, e);
                        Close();
                        break;
                    case MessageBoxResult.No:
                        Close();
                        break;
                }
            }
            else
            {
                Close();
            }
        }

        private void NDSCheckBox_Click(object sender, RoutedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (applyButton != null)
                applyButton.IsEnabled = true;
        }
    }
}
