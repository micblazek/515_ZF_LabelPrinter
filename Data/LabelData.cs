using System;
using System.ComponentModel;
using System.Data;

namespace _515_ZF_LabelPrinter.Data
{
    public class LabelData : INotifyPropertyChanged
    {
        private int _id;
        private int _status;
        private int _procesBoxId;

        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }
        public int Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }
        public int ProcesBoxId { get { return _procesBoxId; } set { _procesBoxId = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LabelData(DataRow row)
        {
            this.Id = String.IsNullOrEmpty(row[nameof(Id)].ToString()) ? (short)-1 : Convert.ToInt32(row[nameof(Id)]);
            this.Status = String.IsNullOrEmpty(row[nameof(Status)].ToString()) ? (short)-1 : Convert.ToInt32(row[nameof(Status)]);
            this.ProcesBoxId = String.IsNullOrEmpty(row[nameof(ProcesBoxId)].ToString()) ? (short)-1 : Convert.ToInt32(row[nameof(ProcesBoxId)]);
        }
    }
}