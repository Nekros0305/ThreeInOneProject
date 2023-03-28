using CommunityToolkit.Mvvm.Messaging;
using System.Globalization;
using ThreeInOne.Models.Sudoku;

namespace ThreeInOne.WPFStuff.Converters;

class TwoDimentionalArrayReader : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string[,] arr)
            return string.Empty;

        if (parameter is not string sources)
            return string.Empty;

        string[] indexes = sources.Trim().Split(',');

        var x = int.Parse(indexes[0]);
        var y = int.Parse(indexes[1]);
        return arr[x, y] ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        WeakReferenceMessenger.Default.Send(new AddToBoardMessage($"{value},{parameter}"));
        return string.Empty;
    }
}