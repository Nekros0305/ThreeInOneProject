using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Interfaces.SunInfo;
public interface ILocationService
{
    Task<LocationInfo> GetLocation(CancellationToken cancellationToken);
}