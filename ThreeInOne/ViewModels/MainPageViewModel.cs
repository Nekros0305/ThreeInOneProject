using Microsoft.Windows.System;

namespace ThreeInOne.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _displaytext = "Click Me";

    [ObservableProperty]
    private string _user = Preferences.Default.Get("userName", string.Empty);

    private int _count = 0;
    [RelayCommand]
    private void Clicked()
    {
        Displaytext = $"clicked {++_count} times";
    }
}
