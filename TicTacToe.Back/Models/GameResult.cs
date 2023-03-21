using TicTacToe.Back.Models.Enums;

namespace TicTacToe.Back.Models
{
    public class GameResult
    {
        public DateTime PlayedAt { get; set; }
        public string PlayerX { get; set; } = string.Empty;
        public string PlayerO { get; set; } = string.Empty;
        public WinningLine TheWinningLine { get; set; }
        public int Result { get; set; }

        public string BoardState { get; set; } = null!;
    }
}