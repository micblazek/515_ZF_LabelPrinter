﻿using _515_ZF_LabelPrinter.Data;
using _515_ZF_LabelPrinter.SQL;
using _515_ZF_LabelPrinter.Windows;
using Jhv.DotNet.Core.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
                if(MsSqlCon.IsConnected())
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
            dbMonitorInfo.LastStart = DateTime.Now;
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
                    List<LabelData> InputKardexSpeakerComand = MsSqlCon.LoadKardexSpeakerComands("K_IN");

                    //Vstupní kardex
                    if (InputKardexSpeakerComand.Count > 0 && (LabelPrintWorker == null || !LabelPrintWorker.IsBusy))
                    {
                        LabelPrintWorker = new BackgroundWorker();
                        LabelPrintWorker.DoWork += LabelPrintWorker_DoWork;
                        LabelPrintWorker.RunWorkerCompleted += LabelPrintWorker_RunWorkerCompleted;

                        List<object> TmpInput = new List<object>();
                        TmpInput.Add(InputKardexSpeakerComand);
                        LabelPrintWorker.RunWorkerAsync(TmpInput);
                    }
                }
                catch (Exception ex)
                {
                    JhvLogger.CatchException(ex.Message, "DatabaseControl_Tick");
                }
            }
            dbMonitorInfo.LastEnd = DateTime.Now;
            dbMonitorInfo.DurationLast = dbMonitorInfo.LastEnd - dbMonitorInfo.LastStart;
        }


        #region WORKER - OutputKardex
        private void LabelPrintWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }

        private void LabelPrintWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
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