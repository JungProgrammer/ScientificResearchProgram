using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
    public partial class InitialWindow : Window
    {

#if DEBUG
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif

        string ServerIp = Settings.Default.ServerIP;
        public InitialWindow()
        {

            InitializeComponent();

            string ActualVersion = CRUDDataBase.GetActualVersion();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Show();
            if (DEBUG)
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
            else
            {
                if (ActualVersion != Settings.Default.ProgrammVersion)
                {

                    NewVersionDownloadInfo.Visibility = Visibility.Visible;
                    DownloadProgressBar.Visibility = Visibility.Visible;
                    MainInfoLabel.Content = "Найдна новая версия";
                    Console.WriteLine("НОВАЯ ВЕРСИЯ ХАХХАХАХА");
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(download_ProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(download_Completed);
                    string uri = "http://" + ServerIp + ":8000/download_update/" + ActualVersion;
                    client.DownloadFileAsync(new Uri(uri), "update.zip");
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
        }

        private void download_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressBar.Value = e.ProgressPercentage;
        }

        private void download_Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (Directory.Exists("updater"))
            {
                Console.WriteLine("Есть апдейтер");
                Process.Start(System.IO.Path.GetFullPath(@"updater\updater.exe"), "temp_myprogram ResearchProgram.exe");
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Console.WriteLine("Нет апдейтера, качаем");
                MainInfoLabel.Content = "Скачивание установщика";
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(download_ProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(UpdaterDownloadComplete);
                string uri = "http://" + ServerIp + ":8000/get_updater";
                client.DownloadFileAsync(new Uri(uri), "updater.zip");
            }
        }

        private void UpdaterDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            string archivePath = "updater.zip";
            using (ZipArchive zipArchive = ZipFile.OpenRead(archivePath))
            {
                ZipFile.ExtractToDirectory(archivePath, Directory.GetCurrentDirectory());
            }
            Process.Start(System.IO.Path.GetFullPath(@"updater\updater.exe"), "temp_myprogram ResearchProgram.exe");
            Process.GetCurrentProcess().Kill();
        }

    }
}
