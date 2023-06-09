using _515_ZF_LabelPrinter.Data;
using Jhv.DotNet.Core.Tool;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace _515_ZF_LabelPrinter.Convertors
{
    public class PrinterConnectionStatusColorConvertor : IValueConverter
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
                            return SmartTool.HexConverter(Color.LightGreen);
                        case PrinterMonitorInformation.ConnectedOption.Disconnected:
                            return SmartTool.HexConverter(Color.OrangeRed);
                        case PrinterMonitorInformation.ConnectedOption.Unknown:
                        default:
                            return SmartTool.HexConverter(Color.LightGreen);
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
