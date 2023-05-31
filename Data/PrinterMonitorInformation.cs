using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _515_ZF_LabelPrinter.Data
{
    public class PrinterMonitorInformation: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime _lastStart;
        private DateTime _lastEnd;
        private TimeSpan _durationLast;

        public DateTime LastStart { get { return _lastStart; } set { _lastStart = value; OnPropertyChanged(); } }
        public DateTime LastEnd { get { return _lastEnd; } set { _lastEnd = value; OnPropertyChanged(); } }
        public TimeSpan DurationLast { get { return _durationLast; } set { _durationLast = value; OnPropertyChanged(); } }

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
