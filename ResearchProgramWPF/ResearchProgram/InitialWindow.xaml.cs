using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

namespace ResearchProgram.Forms
{
    /// <summary>
    /// Логика взаимодействия для InitialWindow.xaml
    /// </summary>
    public partial class InitialWindow : Window
    {
        string ServerIp = Settings.Default.ServerIP;
        public InitialWindow()
        {
            InitializeComponent();
            string ActualVersion = CRUDDataBase.GetActualVersion();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Show();

            if (ActualVersion != Settings.Default.ProgrammVersion)
            {

                NewVersionDownloadInfo.Visibility = Visibility.Visible;
                DownloadProgressBar.Visibility = Visibility.Visible;
                MainInfoLabel.Content = "Найдна новая версия";
                Console.WriteLine("НОВАЯ ВЕРСИЯ ХАХХАХАХА");
                var client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(download_ProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(download_Completed);
                string uri = "http://" + ServerIp + ":8000/download_update/" + ActualVersion;
                client.DownloadFileAsync(new Uri(uri),"update.zip");
            }
            else
            {
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

        private void download_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressBar.Value = e.ProgressPercentage;
        }

        private void download_Completed(object sender, AsyncCompletedEventArgs e)
        {
            Process.Start(System.IO.Path.GetFullPath(@"updater\updater.exe"), "temp_myprogram ResearchProgram.exe");
            Process.GetCurrentProcess().Kill();
            Console.WriteLine("СКАЧАНО");
        }

    }
}
