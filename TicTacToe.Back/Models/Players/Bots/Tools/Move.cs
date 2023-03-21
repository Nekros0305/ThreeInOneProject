namespace TicTacToe.Back.Models.Players.Bots.Tools
{
    internal readonly struct Move
    {
        public Move(int index, int value)
        {
            Index = index;
            Value = value;
        }

        internal int Index { get;}

        internal int Value { get;}
    }
}