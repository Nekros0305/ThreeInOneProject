using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Services.SunInfo;
public class SunInfoServiceError : ISunInfoService
{
    public Task<LocalSunInfo> GetSunInfo(CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Bob was here");
    }
}