namespace ThreeInOne.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _displaytext = "Click Me";

    [ObservableProperty]
    private string _name = "World!";

    private int _count = 0;
    [RelayCommand]
    private void Clicked()
    {
        Displaytext = $"clicked {++_count} times";
    }
}
