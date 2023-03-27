using IP2Location;
using Microsoft.Extensions.Logging;
using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;

//service usable only on windows cause programmer was unable to work with files on Android
namespace ThreeInOne.Services.SunInfo;
internal class LocationServiceFromIp : ILocationService
{
    private readonly IIpService _ipService;
    private readonly ILogger<LocationServiceFromIp> _logger;
    private readonly Component _dataBase;

    public LocationServiceFromIp(
        IIpService ipService,
        ILogger<LocationServiceFromIp> logger)
    {
        _ipService = ipService;
        _logger = logger;
        _dataBase = new Component();
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Others", "IP2LOCATION-LITE-DB.BIN");
        _dataBase.Open(path);
    }

    async Task<LocationInfo> ILocationService.GetLocation(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("LocationServiceIp: Starting search for Ip");
            string ip = await _ipService.GetCurrentIp(cancellationToken);

            IPResult result = _dataBase.IPQuery(ip);
            if (result.Status != "OK")
            {
                _logger.LogError("LocationServiceIp: Failed to find assigned location");
                throw new Exception(result.Status);
            }

            var location = new LocationInfo(
                 result.City,
                 result.Latitude,
                 result.Longitude);

            _logger.LogInformation("LocationServiceIp: Reading Location was successfull");
            return location;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LocationServiceIp: Failed in Getting Location");
            throw;
        }
    }
}