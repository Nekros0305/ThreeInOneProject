namespace TicTacToe.Back.Models.Players.Humans
{
    public class Human : BasePlayer
    {
        public Human(string name)
            : base(name)
        { }

        public Human()
            : this("Player")
        { }
    }
}