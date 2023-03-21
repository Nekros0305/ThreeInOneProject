using ThreeInOne.ViewModels.SunInfo;

namespace ThreeInOne.Views;

public partial class SunInfoPage : ContentPage
{
	public SunInfoPage(SunInfoPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}