using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Services.SunInfo;
public class LocationServiceFake : ILocationService
{
    public Task<LocationInfo> GetLocation(CancellationToken cancellationToken)
    {
        var result = new LocationInfo(
            "Nenzing",
            47.17318870243700f,
            9.75019220094923f);
        return Task.FromResult(result);
    }
}