namespace TicTacToe.Data.Settings
{
    public class DefaultSettings : IDataBaseSettings
    {
        string IDataBaseSettings.PathToFile { get => AppDomain.CurrentDomain.BaseDirectory; set { } }
        string IDataBaseSettings.DBFileName { get => "GameRecords.db"; set { } }
    }
}