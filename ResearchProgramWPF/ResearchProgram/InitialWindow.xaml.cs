using System.Windows;

namespace ResearchProgram.Forms
{
    public partial class InitialWindow : Window
    {
        public InitialWindow()
        {
            InitializeComponent();

            string ActualVersion = CRUDDataBase.GetActualVersion();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Show();
            MainWindow mainWindow = new MainWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };
            mainWindow.Closing += (senders, args) => { mainWindow.Owner = null; };
            mainWindow.Show();
            Hide();
        }
    }
}
