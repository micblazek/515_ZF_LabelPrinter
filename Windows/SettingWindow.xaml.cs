using _515_ZF_LabelPrinter.Pages.SettingPages;
using System.Windows;

namespace _515_ZF_LabelPrinter.Windows
{
    /// <summary>
    /// Interakční logika pro SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow(Window Owner)
        {
            InitializeComponent();
            this.Owner = Owner;
            frmGeneral.Content = new GeneralSettingPage();
            frmMSSQLSetting.Content = new MsSqlSettingPage();
            frmImportedFiles.Content = new PrinterSettingPage();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void Storno_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
