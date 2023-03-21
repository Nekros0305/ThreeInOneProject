using TicTacToe.Back.Models.Enums;
using TicTacToe.Back.Models.Players.Bots.Tools;

namespace TicTacToe.Back.Models.Players.Bots.SubBots
{
    internal class Marco : BaseBot
    {
        public Marco(string name)
            : base(name)
        { }

        public Marco()
            : this(nameof(Marco))
        { }

        public override int GetBotMove(string[] board)
        {
            var spots = Game.GetFreeSpots(board);
            var player = spots.Count % 2 == 1 ? "X" : "O";
            return FindBestPossibleMove(board, player).Index;
        }

        private Move FindBestPossibleMove(string[] board, string player)
        {
            switch (Game.ControlGameState(board))
            {
                case GameState.PXWin:
                    return new Move(-1, 1);
                case GameState.POWin:
                    return new Move(-1, -1);
                case GameState.Draw:
                    return new Move(-1, 0);
            }

            List<Move> moves = new();
            foreach (var spot in Game.GetFreeSpots(board))
            {
                board[spot] = player;
                var recMove = FindBestPossibleMove(board, player == "X" ? "O" : "X");
                moves.Add(new Move(spot, recMove.Value));
                board[spot] = string.Empty;
            }

            var chosenValue = player == "X"
                ? moves.Max(x=> x.Value)
                : moves.Min(x => x.Value);
            moves.RemoveAll(x => x.Value != chosenValue);

            return moves[new Random().Next(0, moves.Count)];
        }
    }
}