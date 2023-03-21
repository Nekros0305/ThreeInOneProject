using Microsoft.EntityFrameworkCore.Design;
using TicTacToe.Data.Settings;

namespace TicTacToe.Data.Contextes
{
    internal class GameRecordContextDesignFactory : IDesignTimeDbContextFactory<GameRecordContext>
    {
        public GameRecordContext CreateDbContext(string[] args)
        {
            var factory = new GameRecordContextFactory(new DefaultSettings());
            return factory.CreateContext();
        }
    }
}