using System.Globalization;

namespace ThreeInOne.WPFStuff.Converters
{
    class WinRatioToPercentageValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            return string.Format("{0:0.## %}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}