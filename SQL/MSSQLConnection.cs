using System;
using System.Collections.Generic;
using System.Data;
using _515_ZF_LabelPrinter.Data;
using _515_ZF_LabelPrinter.Tool;

namespace _515_ZF_LabelPrinter.SQL
{
    public class MSSQLConnection : _515_ZF_Core.SQL.MSSQLConnection
    {
        public MSSQLConnection(string User, string Password, string Server, string Database) : base(User, Password, Server, Database)
        {
        }

        public List<LabelData> LoadKardexSpeakerComands()
        {
            List<LabelData> tmp = new List<LabelData>();
            string cmd = "Select TOP(1) * from " + Constants.PrinterProcessBoxRequestsTableName
                + " WHERE Status = 1"
                + " ORDER BY UpdateTimeStapm desc";

            DataTable da = FillDataTable(cmd, true);

            if (da != null)
            {
                for (int i = 0; i < da.Rows.Count; i++)
                {
                    tmp.Add(new LabelData(da.Rows[i]));
                }
            }
            return tmp;
        }

        public void MarkKardexSpeakerAsDone(LabelData Data)
        {
            string cmd = "UPDATE " + Constants.PrinterProcessBoxRequestsTableName + " SET " +
                "Status = 2 " +
                "WHERE Id = " + Data.Id;

            ExecuteMSSQLCommand(cmd);
        }


    }
}
