using ThreeInOne.ViewModels;

namespace ThreeInOne.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _vm;

    public MainPage(MainPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
		Preferences.Default.Set("userName", string.Empty);

		var preference = Preferences.Default.Get("userName", string.Empty);
		if (string.IsNullOrWhiteSpace(preference))
		{
			var result = await DisplayPromptAsync("Small Question", "Whats your Name?");
			result = $"Hello to our Team, {result}";
			Preferences.Default.Set("userName", result);
			test.Text = result;
		}
    }
}