namespace ThreeInOne;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        App.Current!.UserAppTheme = AppTheme.Light;
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

#if WINDOWS
        window.Height = 650;
        window.Width = 800;

        window.MinimumHeight = 650;
        window.MinimumWidth = 800;
#endif
        return window;
    }

}
