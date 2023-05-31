using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace _515_ZF_LabelPrinter.Pages.SettingPages
{
    /// <summary>
    /// Interakční logika pro ImportedFilePage.xaml
    /// </summary>
    public partial class ImportedFilePage : Page
    {
        public ImportedFilePage()
        {
            InitializeComponent();
        }

        private void btnSelectOutputPathForInputKardex_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.InitialDirectory = Properties.Settings.Default.OutputPathForInputKardex;

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.OutputPathForInputKardex = openFolderDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void btnSelectOutputPathForOutputKardex_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.InitialDirectory = Properties.Settings.Default.OutputPathForOutputKardex;

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.OutputPathForOutputKardex = openFolderDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
