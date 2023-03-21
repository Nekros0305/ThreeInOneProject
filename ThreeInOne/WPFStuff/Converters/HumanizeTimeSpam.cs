using Humanizer;
using System.Globalization;

namespace ThreeInOne.WPFStuff.Converters;
class HumanizeTimeSpam : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TimeSpan time)
            return string.Empty;

        if (!int.TryParse(parameter.ToString(), out int precision))
            return string.Empty;

        return time.Humanize(precision);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}