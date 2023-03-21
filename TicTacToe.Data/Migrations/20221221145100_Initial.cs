using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    PlayerX = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PlayerXType = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerO = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PlayerOType = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Board = table.Column<string>(type: "TEXT", nullable: false),
                    WinningLine = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRecords");
        }
    }
}
