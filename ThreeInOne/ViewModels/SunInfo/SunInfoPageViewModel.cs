﻿using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;
using ThreeInOne.WPFStuff;

namespace ThreeInOne.ViewModels.SunInfo;

public partial class SunInfoPageViewModel : ObservableObject
{
    private readonly ISunInfoService _sunInfoService;
    private readonly ILogger<SunInfoPageViewModel> _logger;

    public SunInfoPageViewModel(
        ISunInfoService sunInfoService,
        ILogger<SunInfoPageViewModel> logger)
    {
        _sunInfoService = sunInfoService;
        _logger = logger;
    }

    private LazyProperty<LocalSunInfo>? _sunInfo;
    public LazyProperty<LocalSunInfo> SunInfo => _sunInfo ??= new LazyProperty<LocalSunInfo>(GetSunInfo);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private bool _executable;

    private async Task<LocalSunInfo> GetSunInfo(CancellationToken cancellationToken)
    {
        try
        {
            Executable = false;
            var result = await _sunInfoService.GetSunInfo(cancellationToken);
            await Task.Delay(1000, cancellationToken);
            Executable = true;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(SunInfoPageViewModel)}: Failed to get SunInfo");
            Executable = true;
            throw;
        }
    }

    [RelayCommand(CanExecute = nameof(Executable))]
    public void Refresh()
    {
        _sunInfo = null;
        OnPropertyChanged(nameof(SunInfo));
    }
}
