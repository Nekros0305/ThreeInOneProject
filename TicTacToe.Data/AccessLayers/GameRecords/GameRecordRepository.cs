using TicTacToe.Data.Contextes;
using TicTacToe.Data.Models;
using TicTacToe.Data.Settings;

namespace TicTacToe.Data.AccessLayers.GameRecords
{
    public class GameRecordRepository
    {
        private readonly GameRecordContext _context;
        public GameRecordRepository(IDataBaseSettings settings)
        {
            var factory = new GameRecordContextFactory(settings);
            _context = factory.Create();
        }

        public List<HighscoresListDto> GetHighScores()
        {
            double GetWinRatioOfPlayer(string player, List<GameRecord> playerGames)
            {
                double winned = 0;
                foreach (var game in playerGames)
                    if ((game.PlayerX == player && game.State == 2) || (game.PlayerO == player && game.State == 3))
                        winned++;

                return winned / playerGames.Count;
            }

            //HERE: Filter Context
            //Actual: No Filter Applied
            var humanGames = _context.GameRecords.ToList();

            List<string> players = new();
            foreach (var game in humanGames)
            {
                if (!players.Contains(game.PlayerX))
                    players.Add(game.PlayerX);

                if (!players.Contains(game.PlayerO))
                    players.Add(game.PlayerO);
            }

            List<HighscoresListDto> highscores = new();
            foreach (var player in players)
            {
                List<GameRecord> playerGames = _context.GameRecords
                                .Where(x => x.PlayerX == player || x.PlayerO == player)
                                .ToList();

                HighscoresListDto highscore = new();
                highscore.PlayerName = player;
                highscore.GamesAmount = playerGames.Count;
                highscore.WinRatio = GetWinRatioOfPlayer(player, playerGames);
                highscore.Punktation = highscore.WinRatio * highscore.GamesAmount;

                highscores.Add(highscore);
            }

            highscores = highscores
                .Where(x => x.GamesAmount >= 5 && x.WinRatio >= 0.12)
                .OrderByDescending(x => x.Punktation)
                .ToList();

            for (int i = 1; i <= highscores.Count; i++)
                highscores[i - 1].Place = i;

            return highscores;
        }

        public void Add(GameRecord item)
        {
            _context.GameRecords.Add(item);
            _context.SaveChanges();
        }
    }
}