namespace TicTacToe.Back.Models.Players.Bots.SubBots
{
    internal class Polo : BaseBot
    {
        public Polo(string name)
            : base(name)
        { }

        public Polo()
            : this(nameof(Polo))
        { }

        public override int GetBotMove(string[] board)
        {
            var freeSpots = Game.GetFreeSpots(board);
            var spot = new Random().Next(0, freeSpots.Count);
            return freeSpots[spot];
        }
    }
}