namespace TicTacToe.Data.Settings
{
    public interface IDataBaseSettings
    {
        public string Path { get; internal set; }
        public string FileName { get; internal set; }
    }
}