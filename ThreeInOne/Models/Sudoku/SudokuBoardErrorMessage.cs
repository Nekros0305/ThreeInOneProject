using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ThreeInOne.Models.Sudoku;

class SudokuBoardErrorMessage : ValueChangedMessage<string>
{
    public SudokuBoardErrorMessage(string message)
        : base(message)
    { }
}