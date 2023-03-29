using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ThreeInOne.Models.Sudoku;

class AddToBoardMessage : ValueChangedMessage<string>
{
    public AddToBoardMessage(string message)
        : base(message)
    { }
}
