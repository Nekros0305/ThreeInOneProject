using CommunityToolkit.Mvvm.Messaging;
using ThreeInOne.Models.TicTacToe;
using ThreeInOne.ViewModels.TicTacToe;

namespace ThreeInOne.Views;

public partial class TicTacToePage : ContentPage, IRecipient<GraphicUpdateMessage>
{
	public TicTacToePage(TicTacToePageViewModel vm, IMessenger messenger)
	{
		InitializeComponent();
		BindingContext = vm;
		messenger.RegisterAll(this);
	}

    void IRecipient<GraphicUpdateMessage>.Receive(GraphicUpdateMessage message)
    {
		grap.Invalidate();
    }
}