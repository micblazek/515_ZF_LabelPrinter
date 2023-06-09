using _515_ZF_LabelPrinter.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace _515_ZF_LabelPrinter.Convertors
{
    public class PrinterConnectionStatusConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is PrinterMonitorInformation.ConnectedOption)
                {
                    switch ((PrinterMonitorInformation.ConnectedOption)value)
                    {
                        case PrinterMonitorInformation.ConnectedOption.Connected:
                            return "Připojeno";
                        case PrinterMonitorInformation.ConnectedOption.Disconnected:
                            return "Odpojeno";
                        case PrinterMonitorInformation.ConnectedOption.Unknown:
                        default:
                            return "Neznámý";
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
