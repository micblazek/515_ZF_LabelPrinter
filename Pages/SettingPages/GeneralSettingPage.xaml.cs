using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _515_ZF_LabelPrinter.Pages.SettingPages
{
    /// <summary>
    /// Interakční logika pro GeneralSettingPage.xaml
    /// </summary>
    public partial class GeneralSettingPage : Page
    {
        public GeneralSettingPage()
        {
            InitializeComponent();
        }

        private void Autostart_Click(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox)
            {
                if((sender as CheckBox).IsChecked == true)
                {
                    AddApplicationToStartup();
                }
                else
                {
                    RemoveApplicationFromStartup();
                }
            }
        }

        public static void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                string FilePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                //string DllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                key.SetValue("515 - FileImporter", "\"" + FilePath + "\"");
            }
        }

        public static void RemoveApplicationFromStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue("515 - FileImporter", false);
            }
        }
    }
}
