namespace TicTacToe.Back.Models.Players
{
    public abstract class BasePlayer
    {
        protected BasePlayer(string name)
        {
            PlayerName = name;
        }

        public string PlayerName { get; set; } = string.Empty;

        public string Sign { get; set; } = string.Empty;
    }
}