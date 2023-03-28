using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Sudoku.Code;
using ThreeInOne.Models.Sudoku;

namespace ThreeInOne.ViewModels.Sudoku
{
    public partial class SudokuPageViewModel : ObservableObject,
        IRecipient<AddToBoardMessage>
    {
        private readonly IMessenger _messenger;
        private readonly ILogger<SudokuPageViewModel> _logger;

        public SudokuPageViewModel(
            IMessenger messenger,
            ILogger<SudokuPageViewModel> logger)
        {
            messenger.RegisterAll(this);
            _messenger = messenger;
            _logger = logger;
        }

        private Solver? _sudokuSolver;

        [ObservableProperty]
        private string[,] _board = new string[9, 9];

        [ObservableProperty]
        private bool _isSolved = false;

        [RelayCommand]
        private void FillBoard()
        {
            Board = new string[9, 9];
            try
            {
                _logger.LogInformation($"{nameof(SudokuPageViewModel)}: Filling Example");
                #region SetWikipediaExample
                _sudokuSolver = new();
                _sudokuSolver.SetValue(1, 0, 3);
                _sudokuSolver.SetValue(3, 1, 1);
                _sudokuSolver.SetValue(4, 1, 9);
                _sudokuSolver.SetValue(5, 1, 5);
                _sudokuSolver.SetValue(2, 2, 8);
                _sudokuSolver.SetValue(7, 2, 6);
                _sudokuSolver.SetValue(0, 3, 8);
                _sudokuSolver.SetValue(4, 3, 6);
                _sudokuSolver.SetValue(0, 4, 4);
                _sudokuSolver.SetValue(3, 4, 8);
                _sudokuSolver.SetValue(8, 4, 1);
                _sudokuSolver.SetValue(4, 5, 2);
                _sudokuSolver.SetValue(1, 6, 6);
                _sudokuSolver.SetValue(6, 6, 2);
                _sudokuSolver.SetValue(7, 6, 8);
                _sudokuSolver.SetValue(3, 7, 4);
                _sudokuSolver.SetValue(4, 7, 1);
                _sudokuSolver.SetValue(5, 7, 9);
                _sudokuSolver.SetValue(8, 7, 5);
                _sudokuSolver.SetValue(7, 8, 7);
                #endregion
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(SudokuPageViewModel)}: failed to fill example");
                Board = new string[9, 9];
            }

            UpdateBoard();
        }

        [RelayCommand]
        private void ClearBoard()
        {
            _sudokuSolver = null;
            Board = new string[9, 9];
            IsSolved = false;
        }

        [RelayCommand]
        private async Task SolveBoard()
        {
            IsSolved = true;
            _logger.LogInformation($"{nameof(SudokuPageViewModel)}: Started solving board");
            _sudokuSolver ??= new();
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    var value = !string.IsNullOrWhiteSpace(Board[x, y]) ? Convert.ToInt32(Board[x, y]) : 0;
                    if (value != 0)
                        _sudokuSolver.SetValue(x, y, value);
                }
            }

            var runningTask = Task.Run(() => _sudokuSolver.SolveBoard());
            while (!runningTask.IsCompleted)
                await Task.Delay(50);

            //TODO: try applying visual filling of board
            if (!runningTask.Result)
            {
                IsSolved = false;
                _logger.LogInformation($"{nameof(SudokuPageViewModel)}: Failed to Solve Board");
                _messenger.Send(new UnableToSolveMessage("Board is unsolvable"));
            }

            UpdateBoard();
        }

        private void UpdateBoard()
        {
            string[,] board = new string[9, 9];
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    var number = _sudokuSolver!.GetValue(x, y);
                    board[x, y] = number != 0 ? number.ToString() : string.Empty;
                }
            }
            Board = board;
            _logger.LogInformation($"{nameof(SudokuPageViewModel)}: Board updated successfully");
        }

        void IRecipient<AddToBoardMessage>.Receive(AddToBoardMessage message)
        {
            string[] values = message.Value.Split(",");

            var x = int.Parse(values[1]);
            var y = int.Parse(values[2]);

            Board[x, y] = values[0];
            _sudokuSolver ??= new();
            try
            {
                if (!string.IsNullOrWhiteSpace(values[0]))
                {
                    if (!int.TryParse(values[0], out var value))
                        throw new Exception("Invalide Input");

                    _sudokuSolver.SetValue(x, y, value);
                    _logger.LogInformation($"{nameof(SudokuPageViewModel)}: Setting value on Coordinates: {x},{y} was successfull");
                }
            }
            catch (Exception e)
            {
                _messenger.Send(new SudokuBoardErrorMessage($"Coordinates:\nX:{x}, Y:{y}\nReturned: {e.Message}"));
                _logger.LogWarning($"{nameof(SudokuPageViewModel)}: {e.Message}");
                Board[x, y] = string.Empty;
                OnPropertyChanged(nameof(Board));
            }
        }
    }
}