using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

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

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.PathToLabelTemplate = openFileDialog.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void btnFolderForGeneratedlabels_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            openFileDialog.InitialDirectory = Properties.Settings.Default.FolderForGeneratedLabels;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.FolderForGeneratedLabels = openFileDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
