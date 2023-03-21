namespace TicTacToe.Back.Models.Players.Bots
{
    internal abstract class BaseBot : BasePlayer
    {
        protected BaseBot(string name)
            : base(name)
        { }

        public abstract int GetBotMove(string[] board);
    }
}