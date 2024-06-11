using _515_ZF_Core.Data;
using _515_ZF_LabelPrinter.Data;
using _515_ZF_LabelPrinter.SQL;
using _515_ZF_LabelPrinter.Windows;
using Jhv.DotNet.Core.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UtfUnknown;

namespace _515_ZF_LabelPrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker LabelPrintWorker;

        private DispatcherTimer refreshMsSql;
        private MSSQLConnection MsSqlCon;

        private DispatcherTimer DatabaseControl;

        private PrinterMonitorInformation dbMonitorInfo;

        private readonly object DataPumpWorking = new object();

        public MainWindow()
        {
            InitializeComponent();
            JhvConsole.WriteLine("StartApp", JhvConsole.STATUS_TIP.ALWAYS);

            DatabaseControl = new DispatcherTimer();
            DatabaseControl.Interval = new TimeSpan(0, 0, 5);
            DatabaseControl.Tick += DatabaseControl_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dbMonitorInfo = new PrinterMonitorInformation();
            dmpCommunicationWithDatabase.DataContext = dbMonitorInfo;

            InitMsSqlConnection();
        }

        #region MS-SQL connection status
        private void InitMsSqlConnection()
        {
            MsSqlCon = new MSSQLConnection(Properties.Settings.Default.SQLUser, Properties.Settings.Default.SQLPassword,
        Properties.Settings.Default.SQLServerUrl, Properties.Settings.Default.SQLDatabaseName);
            MsSqlCon.StatusChange += MsSqlCon_StatusChange;
            InitRefreshMsSql();
            try
            {
                if (MsSqlCon.IsConnected())
                    DatabaseControl.Start();
            }
            catch (Exception)
            {
                //Nepřipojeno
            }

        }

        private void MsSqlCon_StatusChange(object? sender, EventArgs e)
        {
            MSSQLConnection.StatusOption ActualStatus = (MSSQLConnection.StatusOption)sender;

            switch (ActualStatus)
            {
                case MSSQLConnection.StatusOption.Connected:
                    ccMsSqlStatus.ActStatus = Jhv.DotNet.Core.GUI.UserControls.ConnectionControl.ConnectionStatus.Online;
                    break;
                case MSSQLConnection.StatusOption.Connecting:
                    ccMsSqlStatus.ActStatus = Jhv.DotNet.Core.GUI.UserControls.ConnectionControl.ConnectionStatus.Connecting;
                    break;
                case MSSQLConnection.StatusOption.Disconnected:
                    ccMsSqlStatus.ActStatus = Jhv.DotNet.Core.GUI.UserControls.ConnectionControl.ConnectionStatus.Offline;
                    break;
            }
        }
        private void InitRefreshMsSql()
        {
            refreshMsSql = new DispatcherTimer();
            refreshMsSql.Tick += refreshTimer_Tick;
            refreshMsSql.Interval = new TimeSpan(0, 0, 15);
            refreshMsSql.Start();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            refreshMsSql.Stop();
            try
            {
                bool WasOffline = MsSqlCon.ConnectionStatus == Jhv.DotNet.MsSql.MSSQLConnection.StatusOption.Disconnected || MsSqlCon.ConnectionStatus == Jhv.DotNet.MsSql.MSSQLConnection.StatusOption.Unknown;

                MsSqlCon.IsConnected();
                if (!DatabaseControl.IsEnabled && WasOffline)
                {
                    DatabaseControl.Start();
                }
            }
            catch (Exception)
            {
                //MS-SQL nepřipojeno
            }
            refreshMsSql.Start();
        }

        #endregion

        private void DatabaseControl_Tick(object sender, System.EventArgs e)
        {
            // DatabaseControl.Stop();
            StartContractImportAsync();
        }

        private async Task StartContractImportAsync()
        {
            lock (DataPumpWorking)
            {
                MSSQLConnection con;
                try
                {
                    con = PrepareMsSqlConnection();
                }
                catch (Exception)
                {
                    return;
                }

                try
                {
                    //Vstupní kardex
                    if (LabelPrintWorker == null || !LabelPrintWorker.IsBusy)
                    {
                        LabelPrintWorker = new BackgroundWorker();
                        LabelPrintWorker.DoWork += LabelPrintWorker_DoWork;
                        LabelPrintWorker.RunWorkerCompleted += LabelPrintWorker_RunWorkerCompleted;

                        List<object> TmpInput = new List<object>();
                        TmpInput.Add(con);
                        dbMonitorInfo.LastStart = DateTime.Now;
                        LabelPrintWorker.RunWorkerAsync(TmpInput);
                    }
                }
                catch (Exception ex)
                {
                    JhvLogger.CatchException(ex.Message, "DatabaseControl_Tick");
                }
            }
        }


        #region WORKER - Printning
        private void LabelPrintWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> TmpInput = (List<object>)e.Argument;
            MSSQLConnection con = (MSSQLConnection)TmpInput[0];

            LabelData tmp = con.LoadKardexPrinterComands();

            if (!SmartTool.PingHost(Properties.Settings.Default.PrinterIpAdress))
            {
                dbMonitorInfo.PrinterConnectionStatus = PrinterMonitorInformation.ConnectedOption.Disconnected;
                return;
            }

            dbMonitorInfo.PrinterConnectionStatus = PrinterMonitorInformation.ConnectedOption.Connected;


            if (tmp != null)
            {
                BoxWithMaterial ActualBox = con.LoadBoxWithMaterial(tmp.ProcesBoxId);

                if (ActualBox != null)
                {
                    Contract ActualContract = con.LoadContract(ActualBox.ContractId);

                    if (ActualContract != null)
                    {
                        if (SendLabelToPrinter(ActualBox, ActualContract, tmp))
                        {
                            con.MarkKardexSpeakerAsDone(tmp);
                        }
                    }
                    else
                    {
                        JhvLogger.LogInformation("CHYBA VYHLEDÁNÍ ZAKÁZKY (ContractId = " + ActualBox.ContractId + ")");
                    }
                }
                else
                {
                    JhvLogger.LogInformation("CHYBA VYHLEDÁNÍ BOXU S MATERIÁLEM (ProcesBoxId = "+ tmp.ProcesBoxId+")");
                }
            }
            else
            {
                JhvLogger.LogInformation("CHYBA PŘÍKAZU K TISKU");
            }
        }

        private void LabelPrintWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dbMonitorInfo.LastEnd = DateTime.Now;
            dbMonitorInfo.DurationLast = dbMonitorInfo.LastEnd - dbMonitorInfo.LastStart;
        }

        #endregion

        #region Printer

        private bool SendLabelToPrinter(BoxWithMaterial ActualBox, Contract ActualContract, LabelData Lable)
        {
            try
            {
                //Vytvoř a naplň label aktuálními daty
                string ActualLabel = CreatLabel(Properties.Settings.Default.PathToLabelTemplate, ActualBox, ActualContract, Lable);

                System.Net.Sockets.TcpClient TcpClient = new System.Net.Sockets.TcpClient(Properties.Settings.Default.PrinterIpAdress, Properties.Settings.Default.PrinterPort);
                System.Net.Sockets.NetworkStream NetworkStream = TcpClient.GetStream();

                System.IO.Stream FileStream = System.IO.File.OpenRead(ActualLabel);
                byte[] FileBuffer = new byte[FileStream.Length];

                FileStream.Read(FileBuffer, 0, (int)FileStream.Length);
                NetworkStream.Write(FileBuffer, 0, FileBuffer.GetLength(0));
                NetworkStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                JhvConsole.catchExeption(ex);
            }
            return false;
        }

        private string CreatLabel(string originalLabel, BoxWithMaterial ActualBox, Contract ActualContract, LabelData Lable)
        {
            try
            {
                List<FinalPart> FinalPartList = new List<FinalPart>();

                Encoding ActualEncoding = GetEncoding(originalLabel);

                string text = File.ReadAllText(originalLabel, ActualEncoding);


                text = text.Replace("[$ZAKAZKA$]", ActualContract.Name.ToString());
                text = text.Replace("[$MNOZSTVI$]", ActualBox.Quantity.ToString());
                text = text.Replace("[$FINALPART$]", ActualBox.FinalPart.Trim());
                text = text.Replace("[$INPUTPART$]", ActualBox.InputPart.Trim());
                text = text.Replace("[$PRACOVISTE$]", "-");

                string umisteni = "KX";
                if (ActualBox.Location != null)
                {
                    if (ActualBox.Location.Carrier >= 0)
                    {
                        umisteni += "-" + ActualBox.Location.Carrier;
                        if (ActualBox.Location.Slot >= 0)
                        {
                            umisteni += "-" + ActualBox.Location.Slot;
                        }
                    }
                }
                text = text.Replace("[$UMISTENI$]", Lable.Position);
                text = text.Replace("[$VYSKALDNENI$]", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                text = text.Replace("[$LASEROVANI$]", ActualBox.BoxInsertTime.ToString("dd.MM.yyyy HH:mm:ss"));

                if(ActualContract.Linka!=null && ActualContract.Linka.Length > 5 && !Lable.IsDuoControl)
                {
                    string tmpLinka ="P"+ActualContract.Linka.Trim().Substring(3,2);
                    text = text.Replace("[$LINKA$]", tmpLinka);
                }

                if (Lable.IsDuoControl)
                {
                    text = text.Replace("[$LINKA$]", "DUO");
                }

                string path = Path.Combine(Properties.Settings.Default.FolderForGeneratedLabels, "Label_"+Lable.Id+"_BoxId_" + ActualBox.Id + ".txt");

                using (StreamWriter outputFile = new StreamWriter(path))
                {

                    outputFile.WriteLine(text);
                }


                return path;
            }
            catch (FormatException fe)
            {
                JhvLogger.CatchException(fe.Message, "Chyba importu z txt");
            }
            return string.Empty;
        }

        private static Encoding GetEncoding(string path)
        {
            // Detect from File (NET standard 1.3+ or .NET 4+)
            DetectionResult result = CharsetDetector.DetectFromFile(path); // or pass FileInfo

            // Get the best Detection
            DetectionDetail resultDetected = result.Detected;

            // Get the alias of the found encoding
            string encodingName = resultDetected.EncodingName;

            // Get the System.Text.Encoding of the found encoding (can be null if not available)
            Encoding encoding = resultDetected.Encoding;
            return encoding;
        }

        #endregion

        private MSSQLConnection PrepareMsSqlConnection()
        {
            MSSQLConnection con = new MSSQLConnection(Properties.Settings.Default.SQLUser, Properties.Settings.Default.SQLPassword,
            Properties.Settings.Default.SQLServerUrl, Properties.Settings.Default.SQLDatabaseName);

            if (con.ConnectionStatus != MSSQLConnection.StatusOption.Connected)
            {
                con.CreateConnection();
                con.IsConnected();
            }
            return con;
        }

        #region MainButtons
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow mSetting = new SettingWindow(this);
            mSetting.Owner = this;
            mSetting.Show();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow mAbout = new AboutWindow();
            mAbout.Owner = this;
            mAbout.Show();
        }

        private void Abord_Click(object sender, RoutedEventArgs e)
        {
            if (this.Tag != null && this.Tag is App)
            {
                (this.Tag as App).IsExit = true;
            }


            Close();
        }

        #endregion


    }
}
