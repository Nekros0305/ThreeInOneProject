using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using ThreeInOne.Models.TicTacToe;
using TicTacToe.Back;
using TicTacToe.Back.Models.Enums;
using TicTacToe.Back.Models.Players;
using TicTacToe.Back.Models.Players.Humans;
using TicTacToe.Data.AccessLayers.GameRecords;
using TicTacToe.Data.Settings;

namespace ThreeInOne.ViewModels.TicTacToe
{
    public partial class TicTacToePageViewModel : ObservableObject, IDrawable
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GameStartCommand))]
        [NotifyPropertyChangedFor(nameof(Records))]
        private Game? _currentGame;

        [ObservableProperty]
        private string[] _spots = new string[9];

        public List<PlayerType> Players => Game.GetPossiblePlayers()
            .Select(x => new PlayerType
            {
                PlayerName = x.Key,
                PlayerCat = x.Value,
            }).ToList();

        [ObservableProperty]
        private PlayerType? _playerX;
        partial void OnPlayerXChanged(PlayerType? value)
        {
            if (value.PlayerName != "Human")
                PlayerXName = value.PlayerName;
            PlayerXType = value.PlayerCat;
        }

        [ObservableProperty]
        private PlayerType? _playerO;
        partial void OnPlayerOChanged(PlayerType? value)
        {
            if (value.PlayerName != "Human")
                PlayerOName = value.PlayerName;
            PlayerOType = value.PlayerCat;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InGamePlayers))]
        [NotifyCanExecuteChangedFor(nameof(GameStartCommand))]
        private string? _playerXName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayerXNameNeeded))]
        [NotifyCanExecuteChangedFor(nameof(GameStartCommand))]
        private Type? _playerXType;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InGamePlayers))]
        [NotifyCanExecuteChangedFor(nameof(GameStartCommand))]
        private string? _playerOName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayerONameNeeded))]
        [NotifyCanExecuteChangedFor(nameof(GameStartCommand))]
        private Type? _playerOType;

        [ObservableProperty]
        private BasePlayer? _whoWon;

        private static WinningLine? _winLines;

        private IDataBaseSettings _settings = new DefaultSettings();
        public List<HighscoresListDto> Records => new GameRecordRepository(_settings).GetHighScores();

        [RelayCommand(CanExecute = nameof(CanStart))]
        private void GameStart()
        {
            Spots = new string[9];

            var playerX = (BasePlayer)Activator.CreateInstance(PlayerXType!)!;
            var playerO = (BasePlayer)Activator.CreateInstance(PlayerOType!)!;

            if (playerX is Human)
                playerX.PlayerName = PlayerXName!;
            else
                PlayerXName = playerX.PlayerName;

            if (playerO is Human)
                playerO.PlayerName = PlayerOName!;
            else
                PlayerOName = playerO.PlayerName;

            CurrentGame = new Game(playerX, playerO, true);

            UpdateBoard();
        }

        [RelayCommand(CanExecute = nameof(CanTakeSpot))]
        private void TakeSpot(object parameter)
        {
            var spot = Convert.ToInt32(parameter);
            if (!string.IsNullOrWhiteSpace(Spots[spot]))
                return;
            CurrentGame!.TakeSpot(spot);
            UpdateBoard();
        }

        private bool CanTakeSpot(object parameter)
            => CurrentGame is not null
            && !CurrentGame.IsFinished
            && string.IsNullOrWhiteSpace(Spots[Convert.ToInt32(parameter)]);

        private bool CanStart()
            => PlayerX != null && PlayerO != null
            && (PlayerXType != typeof(Human) || !string.IsNullOrWhiteSpace(PlayerXName))
            && (PlayerOType != typeof(Human) || !string.IsNullOrWhiteSpace(PlayerOName))
            && (CurrentGame is null || CurrentGame.IsFinished);

        private void UpdateBoard()
        {
            for (int i = 0; i < 9; i++)
                Spots[i] = CurrentGame!.GetSpotValue(i);
            OnPropertyChanged(nameof(Spots));

            if (CurrentGame!.IsFinished)
            {
                WhoWon = CurrentGame.GetWinner();
                GameStartCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CurrentGame));

                //TODO: tell view to update its graphicsView
                _winLines = CurrentGame?.GetResult()?.TheWinningLine;
                WeakReferenceMessenger.Default.Send(new GraphicUpdateMessage(string.Empty));
            }
            TakeSpotCommand.NotifyCanExecuteChanged();
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (_winLines is null)
                return;

            canvas.StrokeSize = 4;
            canvas.StrokeColor = Colors.Blue;

            if (_winLines.Value.HasFlag(WinningLine.Diagonal1))
                canvas.DrawLine(new Point(0, 0), new Point(dirtyRect.Width, dirtyRect.Height));
            if (_winLines.Value.HasFlag(WinningLine.Diagonal2))
                canvas.DrawLine(new Point(dirtyRect.Width, 0), new Point(0, dirtyRect.Height));

            if (_winLines.Value.HasFlag(WinningLine.Vertical1))
                canvas.DrawLine(new Point(dirtyRect.Width * 0.15, 0), new Point(dirtyRect.Width * 0.15, dirtyRect.Height));
            if (_winLines.Value.HasFlag(WinningLine.Vertical2))
                canvas.DrawLine(new Point(dirtyRect.Width * 0.5, 0), new Point(dirtyRect.Width * 0.5, dirtyRect.Height));
            if (_winLines.Value.HasFlag(WinningLine.Vertical3))
                canvas.DrawLine(new Point(dirtyRect.Width * 0.85, 0), new Point(dirtyRect.Width * 0.85, dirtyRect.Height));

            if (_winLines.Value.HasFlag(WinningLine.Horizontal1))
                canvas.DrawLine(new Point(0, dirtyRect.Height * 0.15), new Point(dirtyRect.Width, dirtyRect.Height * 0.15));
            if (_winLines.Value.HasFlag(WinningLine.Horizontal2))
                canvas.DrawLine(new Point(0, dirtyRect.Height * 0.5), new Point(dirtyRect.Width, dirtyRect.Height * 0.5));
            if (_winLines.Value.HasFlag(WinningLine.Horizontal3))
                canvas.DrawLine(new Point(0, dirtyRect.Height * 0.85), new Point(dirtyRect.Width, dirtyRect.Height * 0.85));
        }

        public string InGamePlayers => $"{PlayerXName} vs {PlayerOName}";
        public bool PlayerXNameNeeded => PlayerXType == typeof(Human);
        public bool PlayerONameNeeded => PlayerOType == typeof(Human);
    }
}