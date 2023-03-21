using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ThreeInOne.Models.Sudoku;

class UnableToSolveMessage : ValueChangedMessage<string>
{
    public UnableToSolveMessage(string message)
        : base(message)
    { }
}