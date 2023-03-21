using TicTacToe.Back.Models;
using TicTacToe.Back.Models.Enums;
using TicTacToe.Back.Models.Players;
using TicTacToe.Back.Models.Players.Bots;
using TicTacToe.Data.AccessLayers.GameRecords;
using TicTacToe.Data.Models;
using TicTacToe.Data.Settings;

namespace TicTacToe.Back
{
    public class Game
    {
        /// <summary>
        /// 1D array, represent our board
        /// grafical representation:
        /// 0 1 2
        /// 3 4 5
        /// 6 7 8
        /// </summary>
        private readonly string[] _board;

        /// <summary>
        /// Enum representing State of the game
        /// Possible States:
        /// Ongoing
        /// Draw
        /// PXWin
        /// POWin
        /// </summary>
        private GameState State { get; set; } = GameState.Ongoing;

        private readonly bool _saveInDB;
        private readonly GameRecordRepository GameRepo = new(new DefaultSettings());

        public GameResult Result { get; private set; }

        /// <summary>
        /// Access to first player with sign X
        /// </summary>
        public BasePlayer PlayerX { get; }

        /// <summary>
        /// Access to second player with sign O
        /// </summary>
        public BasePlayer PlayerO { get; }

        /// <summary>
        /// Access to Current Player
        /// </summary>
        public BasePlayer CurrentPlayer { get; private set; }

        public bool IsFinished => State != GameState.Ongoing;

        /// <summary>
        /// Constructor of Game, in case of current player being bot, makes his move automatically
        /// </summary>
        /// <param name="playerX"></param>
        /// <param name="playerO"></param>
        public Game(BasePlayer playerX, BasePlayer playerO, bool saveInDB = false)
        {
            _board = new string[9];
            _saveInDB = saveInDB;

            PlayerX = playerX;
            PlayerX.Sign = "X";

            CurrentPlayer = PlayerX;

            PlayerO = playerO;
            PlayerO.Sign = "O";

            if (CurrentPlayer is BaseBot bot)
                SetValue(bot.GetBotMove(_board));
        }

        /// <summary>
        /// Access to selected index on board
        /// </summary>
        /// <param name="x"></param>
        /// <returns>value of index</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public string GetSpotValue(int x)
            => x is < 0 or >= 9
            ? throw new IndexOutOfRangeException()
            : _board[x];

        /// <summary>
        /// Checks if the game has come to the end
        /// </summary>
        /// <returns>boolean value showing if game is finished</returns>

        /// <summary>
        /// Determins which player has won the game
        /// </summary>
        /// <returns>The Winner, or in case of draw null</returns>
        /// <exception cref="Exception">Denies work as long as the game is ongoing</exception>
        public BasePlayer? GetWinner()
        {
            if (State == GameState.Ongoing)
                throw new Exception("There will be no Winner until the end of the game");

            return State switch
            {
                GameState.PXWin => PlayerX,
                GameState.POWin => PlayerO,
                _ => null
            };
        }

        public static Dictionary<string, Type> GetPossiblePlayers()
        {
            return typeof(BasePlayer)
                .Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => x.IsSubclassOf(typeof(BasePlayer))).ToDictionary(x => x.Name, x => x);
        }

        /// <summary>
        /// Controls if spot is possible to be taken, and sets value if it is allowed
        /// </summary>
        /// <param name="x"></param>
        /// <exception cref="Exception">Does not allow to take already selected spots</exception>
        /// <exception cref="Exception">Denies ability to change the board, when the game is no longer ongoing</exception>
        public void TakeSpot(int x)
        {
            if (!string.IsNullOrEmpty(GetSpotValue(x)))
                throw new Exception("Selecting already taken spots is not allowed");

            if (IsFinished)
                throw new Exception("You cannot take any spot, while the game is not ongoing");

            SetValue(x);
        }

        /// <summary>
        /// extension to TakeSpot
        /// Sets spot on board with sign of current player
        /// changes player and if new current is a bot, makes his move
        /// </summary>
        /// <param name="x"></param>
        private void SetValue(int x)
        {
            _board[x] = CurrentPlayer.Sign;
            State = ControlGameState(_board);
            if (State == GameState.Ongoing)
            {
                CurrentPlayer =
                    CurrentPlayer == PlayerX
                    ? PlayerO
                    : PlayerX;

                if (CurrentPlayer is BaseBot bot)
                    SetValue(bot.GetBotMove(_board));
            }
            else
            {
                if (_saveInDB)
                    SaveInDB();
            }
        }

        private void SaveInDB()
        {
            var result = GetResult()!;

            GameRepo.Add(new GameRecord
            {
                PlayedAt = DateTimeOffset.Now.ToUnixTimeSeconds(),
                PlayerX = result.PlayerX,
                PlayerXType = PlayerX.GetType().Name,
                PlayerO = result.PlayerO,
                PlayerOType = PlayerO.GetType().Name,
                Board = result.BoardState,
                State = result.Result,
                WinningLine = (int)result.TheWinningLine,
            });
        }

        /// <summary>
        /// Determinates the new state of board
        /// </summary>
        /// <param name="board">string Array with lenght of 9</param>
        /// <returns>State of the Board</returns>
        /// <exception cref="Exception">does not allow any other board that has not length of 9</exception>
        internal static GameState ControlGameState(string[] board)
        {
            GameState SelectWinner(int spot)
                => board[spot] == "X"
                ? GameState.PXWin
                : GameState.POWin;

            if (board.Length != 9)
                throw new Exception("Accepts only boards in size of 9 indexes");

            //rows
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(board[i * 3]) && board[i * 3] == board[i * 3 + 1] && board[i * 3 + 1] == board[i * 3 + 2])
                    return SelectWinner(i * 3);
            }

            //colums
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(board[i]) && board[i] == board[i + 3] && board[i + 3] == board[i + 6])
                    return SelectWinner(i);
            }

            //left top -> right bottom
            if (!string.IsNullOrEmpty(board[0]) && board[0] == board[4] && board[4] == board[8])
                return SelectWinner(0);

            //right top -> left bottom
            if (!string.IsNullOrEmpty(board[2]) && board[2] == board[4] && board[4] == board[6])
                return SelectWinner(2);

            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(board[i]))
                    return GameState.Ongoing;
            }

            return GameState.Draw;
        }

        public GameResult? GetResult()
        {
            if (!IsFinished)
                return null;

            return new GameResult
            {
                PlayedAt = DateTime.Now,
                PlayerX = PlayerX.PlayerName,
                PlayerO = PlayerO.PlayerName,
                Result = (int)State,
                BoardState = string.Join(',', _board),
                TheWinningLine = GetWinningLine(),
            };
        }

        public WinningLine GetWinningLine()
        {
            var result = WinningLine.None;
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(_board[i * 3]) && _board[i * 3] == _board[i * 3 + 1] && _board[i * 3 + 1] == _board[i * 3 + 2])
                {
                    result |= i == 0 ? WinningLine.Horizontal1 : i == 1 ? WinningLine.Horizontal2 : WinningLine.Horizontal3;
                }
            }

            //colums
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(_board[i]) && _board[i] == _board[i + 3] && _board[i + 3] == _board[i + 6])
                    result |= i == 0 ? WinningLine.Vertical1 : i == 1 ? WinningLine.Vertical2 : WinningLine.Vertical3;
            }

            //left top -> right bottom
            if (!string.IsNullOrEmpty(_board[0]) && _board[0] == _board[4] && _board[4] == _board[8])
                result |= WinningLine.Diagonal1;

            //right top -> left bottom
            if (!string.IsNullOrEmpty(_board[2]) && _board[2] == _board[4] && _board[4] == _board[6])
                result |= WinningLine.Diagonal2;

            return result;
        }

        /// <summary>
        /// Enumarate the board in search of free spots
        /// </summary>
        /// <param name="board"></param>
        /// <returns>List with numbers representing free indexes on board</returns>
        internal static List<int> GetFreeSpots(string[] board)
        {
            List<int> spots = new();
            for (int i = 0; i < board.Length; i++)
            {
                if (string.IsNullOrEmpty(board[i]))
                    spots.Add(i);
            }
            return spots;
        }
    }
}