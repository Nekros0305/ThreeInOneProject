using Microsoft.EntityFrameworkCore;
using TicTacToe.Data.Settings;

namespace TicTacToe.Data.Contextes
{
    internal class GameRecordContextFactory
    {
        private static bool MigrationsDone = false;
        private readonly IDataBaseSettings _settings;

        public GameRecordContextFactory(IDataBaseSettings settings)
        {
            _settings = settings;
        }

        public GameRecordContext Create()
        {
            var result = CreateContext();

            if (!MigrationsDone)
            {
                result.Database.Migrate();
                MigrationsDone = true;
                result = CreateContext();
            }

            return result;
        }

        internal GameRecordContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<GameRecordContext>()
                .UseSqlite($"Data Source={Path.Combine(_settings.PathToFile, _settings.DBFileName)}")
                .Options;

            return new GameRecordContext(options);
        }
    }
}