using Jhv.DotNet.Core.Tool;
using NullSoftware.ToolKit;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace _515_ZF_LabelPrinter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TrayIcon _notifyIcon;
        private bool _isExit;
        private static Mutex _mutex = null;

        public bool IsExit { set { _isExit = value; } }

        protected override void OnStartup(StartupEventArgs ae)
        {
            try
            {
                const string appName = "515 - LabelPrinter";
                bool createdNew;

                _mutex = new Mutex(true, appName, out createdNew);

                if (!createdNew)
                {
                    //app is already running! Exiting the application  
                    JhvConsole.WriteLine("Aplikace už běží", JhvConsole.STATUS_TIP.ALWAYS);
                    Current.Shutdown();
                }
                //base.OnStartup(ae);

                MainWindow = new MainWindow();
                MainWindow.Tag = this;
                MainWindow.Closing += MainWindow_Closing;
                MainWindow.Show();
                

                string PathToIcon = "pack://application:,,,/_515_ZF_LabelPrinter;component/Images/Jhv.ico";
                //BitmapImage Icon = new BitmapImage(new Uri(PathToIcon, UriKind.Absolute));

                _notifyIcon = new TrayIcon()
                {
                    Title = "JHV - LabelPrinter"
                    // IconSource = Icon,     
                };
                _notifyIcon.Click += _notifyIcon_Click;

               //CreateContextMenu();

                JhvConsole.WriteLine("515 - LabelPrinter, version " + getVersion(), JhvConsole.STATUS_TIP.ALWAYS);
            }
            catch (Exception e)
            {
                JhvConsole.catchExeption(e);
            }
        }

        private void _notifyIcon_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowMainWindow();
        }

        private void CreateContextMenu()
        {
            Button show = new Button();
            show.Content = "Zobrazit";
            show.Click += Show_Click;

            Button exit = new Button();
            exit.Content = "Ukončit";
            exit.Click += Exit_Click;

            //Nefunguje
            _notifyIcon.ContextMenu = new ContextMenu();
            _notifyIcon.ContextMenu.Items.Add(show);
            _notifyIcon.ContextMenu.Items.Add(exit);
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }

        private string getVersion()
        {
            try
            {
                string ActVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return ActVersion;
            }
            catch (Exception)
            {
                return "Aplikace není nainstalována";
            }
        }

    }
}
