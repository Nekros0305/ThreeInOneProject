using System.Globalization;
using TicTacToe.Back;

namespace ThreeInOne.WPFStuff.Converters
{
    public class GameToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Game game)
                return string.Empty;

            if (!game.IsFinished)
                return string.Empty;

            if (game.GetWinner() is not { } winner)
                return "It's a Draw!";

            return $"The winner is {winner.PlayerName}!";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
