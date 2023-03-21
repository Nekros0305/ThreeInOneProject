using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ThreeInOne.Models.TicTacToe;

internal class GraphicUpdateMessage : ValueChangedMessage<string>
{
    public GraphicUpdateMessage(string message)
        : base(message)
    { }
}