namespace TicTacToe.Data.Settings
{
    public class DefaultSettings : IDataBaseSettings
    {
        string IDataBaseSettings.Path { get => AppDomain.CurrentDomain.BaseDirectory; set { } }
        string IDataBaseSettings.FileName { get => "GameRecords.db"; set { } }
    }
}