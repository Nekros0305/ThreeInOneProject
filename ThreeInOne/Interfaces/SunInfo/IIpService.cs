namespace ThreeInOne.Interfaces.SunInfo;
public interface IIpService
{
    Task<string> GetCurrentIp(CancellationToken cancellationToken);
}