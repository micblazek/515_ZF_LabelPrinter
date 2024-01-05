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
        private string _position;
        private bool _isDuoControl;

        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }
        public int Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }
        public int ProcesBoxId { get { return _procesBoxId; } set { _procesBoxId = value; OnPropertyChanged(); } }
        public string Position { get { return _position; } set { _position = value; OnPropertyChanged(); } }
        public bool IsDuoControl { get { return _isDuoControl; } set { _isDuoControl = value; OnPropertyChanged(); } }

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
            this.Position = String.IsNullOrEmpty(row[nameof(Position)].ToString()) ? string.Empty : (row[nameof(Position)]).ToString().Trim();
            this.IsDuoControl = String.IsNullOrEmpty(row[nameof(IsDuoControl)].ToString()) ? false : Convert.ToBoolean(row[nameof(IsDuoControl)]);
        }
    }
}