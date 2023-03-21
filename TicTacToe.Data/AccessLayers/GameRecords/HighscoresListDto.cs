namespace TicTacToe.Data.AccessLayers.GameRecords
{
    public class HighscoresListDto
    {
        public int Place { get; internal set; }
        public string PlayerName { get; internal set; } = null!;

        public int GamesAmount { get; internal set; }

        public double WinRatio { get; internal set; }

        internal double Punktation { get; set; }
    }
}