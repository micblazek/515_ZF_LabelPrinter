using _515_ZF_LabelPrinter.SQL;
using Jhv.DotNet.Core.Tool;
using Jhv.DotNet.MsSql.Exceptions;
using System.Windows;
using System.Windows.Controls;


namespace _515_ZF_LabelPrinter.Pages.SettingPages
{
    /// <summary>
    /// Interakční logika pro MsSqlSettingPage.xaml
    /// </summary>
    public partial class MsSqlSettingPage : Page
    {
        private MSSQLConnection con;
        public MsSqlSettingPage()
        {
            InitializeComponent();
            pbPassword.Password = Properties.Settings.Default.SQLPassword;
        }

        private void BtnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SQLPassword = pbPassword.Password;
            Properties.Settings.Default.Save();

            if (con == null)
            {
                con = new MSSQLConnection(Properties.Settings.Default.SQLUser, Properties.Settings.Default.SQLPassword,
                    Properties.Settings.Default.SQLServerUrl, Properties.Settings.Default.SQLDatabaseName);
            }

            try
            {
                if (con.IsConnected())
                {
                    JhvConsole.MsgBoxShowInfo("Připojení k databázi bylo úspěšné", "Info");
                }
                else
                {
                    JhvConsole.MsgBoxShowInfo("Připojení k databázi nebylo navázáno. Pokuď si jsi jistý správností, zkus restartovat aplikaci.", "Info");
                }
            }
            catch (ConnectionException ce)
            {
                JhvConsole.MsgBoxShowInfo(ce.Message, "Info");
            }
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SQLPassword = pbPassword.Password;
        }
    }
}
