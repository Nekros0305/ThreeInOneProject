using Microsoft.EntityFrameworkCore;
using TicTacToe.Data.Models;

namespace TicTacToe.Data.Contextes
{
    internal class GameRecordContext : DbContext
    {
        public GameRecordContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<GameRecord> GameRecords { get; set; }
    }
}