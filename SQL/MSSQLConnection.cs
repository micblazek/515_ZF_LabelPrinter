using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using _515_ZF_Core.Data;
using _515_ZF_LabelPrinter.Data;
using _515_ZF_LabelPrinter.Tool;

namespace _515_ZF_LabelPrinter.SQL
{
    public class MSSQLConnection : _515_ZF_Core.SQL.MSSQLConnection
    {
        public MSSQLConnection(string User, string Password, string Server, string Database) : base(User, Password, Server, Database)
        {
        }

        public LabelData LoadKardexPrinterComands()
        {
            string cmd = "Select TOP(1) * from " + Constants.PrinterProcessBoxRequestsTableName
                + " WHERE Status = 1"
                + " ORDER BY UpdateTimeStapm desc";

            DataTable da = FillDataTable(cmd, true);

            if (da != null)
            {
                for (int i = 0; i < da.Rows.Count; i++)
                {
                    return new LabelData(da.Rows[i]);
                }
            }
            return null;
        }

        public void MarkKardexSpeakerAsDone(LabelData Data)
        {
            string cmd = "UPDATE " + Constants.PrinterProcessBoxRequestsTableName + " SET " +
                "Status = 2 " +
                "WHERE Id = " + Data.Id;

            ExecuteMSSQLCommand(cmd);
        }

        public BoxWithMaterial LoadBoxWithMaterial(int ProcesBoxId)
        {
            try
            {
                string cmd = "select TOP(1) * from " + _515_ZF_Core.Tool.Constants.BOX_ALL_INFORMATION_VIEW + " WHERE Id = " + ProcesBoxId;

                DataTable da = FillDataTable(cmd, true);

                foreach (DataRow row in da.Rows)
                {
                    return new BoxWithMaterial(row);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Contract LoadContract(int ContractId)
        {
            try
            {
                string cmd = "select TOP(1) * from " + _515_ZF_Core.Tool.Constants.CONTRACTS_TABLE_NAME + " WHERE Id = " + ContractId;

                DataTable da = FillDataTable(cmd, true);

                foreach (DataRow row in da.Rows)
                {
                    return new Contract(row);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
