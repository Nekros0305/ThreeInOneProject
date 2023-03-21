using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Interfaces.SunInfo;
public interface ISunInfoService
{
    Task<LocalSunInfo> GetSunInfo(CancellationToken cancellationToken);
}