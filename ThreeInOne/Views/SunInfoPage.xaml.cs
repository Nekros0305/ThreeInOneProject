using System.ComponentModel;
using ThreeInOne.ViewModels.SunInfo;

namespace ThreeInOne.Views;

public partial class SunInfoPage : ContentPage
{

    private readonly SunInfoPageViewModel _vm;
    private readonly Animation _animation;

    public SunInfoPage(SunInfoPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        _vm = vm;
        _animation = new Animation(v => buttonRefresh.Rotation = v, 0, 360, Easing.Linear);
        _vm.PropertyChanged += Vm_PropertyChenged;
    }

    private void Vm_PropertyChenged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_vm.Executable))
        {
            if (_vm.Executable == false)
                _animation.Commit(this, "test", 16, 1000, Easing.Linear, (v, c) => buttonRefresh.Rotation = 0, () => true);
            else
                this.AbortAnimation("test");
        }
    }
}