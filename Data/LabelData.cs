using _515_ZF_Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _515_ZF_LabelPrinter.Data
{
    public class LabelData : INotifyPropertyChanged
    {
        private int _id;
        private int _status;
        private string _serialNumber;

        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }
        public int Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }
        public string SerialNumber { get { return _serialNumber; } set { _serialNumber = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LabelData(DataRow row)
        {
            this.Id = String.IsNullOrEmpty(row[nameof(Id)].ToString()) ? (short)-1 : Convert.ToInt32(row[nameof(Id)]);
            this.Status = String.IsNullOrEmpty(row[nameof(Status)].ToString()) ? (short)-1 : Convert.ToInt32(row[nameof(Status)]);
            this.SerialNumber = String.IsNullOrEmpty(row[nameof(SerialNumber)].ToString()) ? string.Empty : row[nameof(SerialNumber)].ToString().Trim();
        }
    }
}