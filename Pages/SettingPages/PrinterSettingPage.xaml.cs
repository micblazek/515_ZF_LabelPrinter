using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
namespace _515_ZF_LabelPrinter.Pages.SettingPages
{
    /// <summary>
    /// Interakční logika pro ImportedFilePage.xaml
    /// </summary>
    public partial class PrinterSettingPage : Page
    {
        public PrinterSettingPage()
        {
            InitializeComponent();
        }

        private void btnSelectPathToLabelTemplate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Properties.Settings.Default.PathToLabelTemplate;

            if (openFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.PathToLabelTemplate = openFileDialog.FileName;
                Properties.Settings.Default.Save();
            }
        }
    }
}
