using Jhv.DotNet.Core.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace _515_ZF_LabelPrinter.Windows
{
    /// <summary>
    /// Interakční logika pro AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            InitApplicationVersionLabel();
        }
        private void InitApplicationVersionLabel()
        {
            string version = null;
            try
            {
                ObservableCollection<PackeageInformation> PackageList = new ObservableCollection<PackeageInformation>();
                PackageList.Add(new PackeageInformation(typeof(Jhv.DotNet.Core.BuildInfo), Jhv.DotNet.Core.BuildInfo.Commit(), Jhv.DotNet.Core.BuildInfo.CommitDate(), Jhv.DotNet.Core.BuildInfo.Branch()));
                PackageList.Add(new PackeageInformation(typeof(Jhv.DotNet.MsSql.BuildInfo), Jhv.DotNet.MsSql.BuildInfo.Commit(), Jhv.DotNet.MsSql.BuildInfo.CommitDate(), Jhv.DotNet.MsSql.BuildInfo.Branch()));
                PackageList.Add(new PackeageInformation(typeof(App), ThisAssembly.Git.Commit, ThisAssembly.Git.CommitDate, ThisAssembly.Git.Branch));

                DataGridPackageList.ItemsSource = PackageList;
                version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            }
            catch (Exception)
            {
                version = "Není nainstalováno";
            }
            lblAplikationVersionValue.Content = version;
        }

        private void btnAppParametrs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Tool.Constants.DataFolderName + "\\",
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                JhvConsole.catchExeption(ex);
            }
        }

        private void btnErrorLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + JhvLogger.PROGRAM_LOG_FOLDER,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                JhvConsole.catchExeption(ex);
            }
        }
    }
}
