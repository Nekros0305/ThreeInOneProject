using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Services.SunInfo;
public class SunInfoServiceFake : ISunInfoService
{
    private readonly ILocationService _locationService;

    public SunInfoServiceFake(
        ILocationService locationService)
    {
        _locationService = locationService;
    }
    public async Task<LocalSunInfo> GetSunInfo(CancellationToken cancellationToken)
    {
        await Task.Delay(3000, cancellationToken);

        var locationInfo = await _locationService.GetLocation(cancellationToken);

        return new LocalSunInfo(
            TimeSpan.FromHours(7.5),
            TimeSpan.FromHours(19.5),
            locationInfo.City,
            TimeSpan.FromHours(12.55),
            DateTime.Now);
    }
}