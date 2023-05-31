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

        public List<LabelData> LoadKardexSpeakerComands(string KardexCode)
        {
            List<LabelData> tmp = new List<LabelData>();
            string cmd = "Select TOP(1) * from " + Constants.KardexSpeakerTableName
                + " WHERE Status = 5 AND Kardex = '" + KardexCode + "'"
                + " ORDER BY Timestamp desc";

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
            //string cmd = "UPDATE " + Constants.KardexSpeakerTableName + " SET " +
            //    "Status = 1 ," +
            //    "FileName = '" + Data.FileName + "'" +
            //    "WHERE ID = " + Data.Id;

            //ExecuteMSSQLCommand(cmd);
        }


    }
}
