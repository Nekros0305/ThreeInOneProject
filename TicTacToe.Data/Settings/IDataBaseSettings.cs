namespace TicTacToe.Data.Settings
{
    public interface IDataBaseSettings
    {
        public string PathToFile { get; set; }
        public string DBFileName { get;  set; }
    }
}