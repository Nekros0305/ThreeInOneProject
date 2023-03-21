namespace TicTacToe.Back.Models.Enums
{
    [Flags]
    public enum WinningLine
    {
        None = 0,
        Horizontal1 = 1,
        Horizontal2 = 2,
        Horizontal3 = 4,
        Vertical1 = 8,
        Vertical2 = 16,
        Vertical3 = 32,
        Diagonal1 = 64,
        Diagonal2 = 128,
    }
}