using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Services.SunInfo;
internal class SunInfoServiceReal : ISunInfoService
{
    private readonly ILocationService _locationService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SunInfoServiceReal> _logger;

    public SunInfoServiceReal(
        ILocationService locationService,
        IHttpClientFactory httpClientFactory,
        ILogger<SunInfoServiceReal> logger)
    {
        _locationService = locationService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<LocalSunInfo> GetSunInfo(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("SunInfoService: begginning gathering required informations");
            var location = await _locationService.GetLocation(cancellationToken);

            _logger.LogInformation("Creating Http Client");
            using var client = _httpClientFactory.CreateClient(nameof(SunInfoServiceReal));

            string url = $"/json?lat={location.Latitude}&lng={location.Longitude}&formatted=0";

            _logger.LogInformation("SunInfoService: Connecting To {DestinationUrl}", client.BaseAddress + url);
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            SunResultModel result = await response.Content.ReadAsAsync<SunResultModel>();
            if (result.Status != "OK")
            {
                _logger.LogError("SunInfoService: Failed at converting response");
                throw new Exception("Api returned error");
            }

            var sunInfo = new LocalSunInfo(
                result.Results.Sunrise.ToLocalTime().TimeOfDay,
                result.Results.Sunset.ToLocalTime().TimeOfDay,
                location.City,
                TimeSpan.FromSeconds(result.Results.DayLength),
                DateTime.Now);

            _logger.LogInformation("SunInfoService: Successed on creating LocalSunInfo");
            return sunInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SunInfoService: Failed at Loading SunInfo");
            throw;
        }
    }

    private record SunResultModel
    {
        public SunModel Results { get; init; } = null!;
        public string Status { get; init; } = null!;

    }
    private record SunModel
    {
        public DateTimeOffset Sunrise { get; init; }
        public DateTimeOffset Sunset { get; init; }

        [JsonProperty("day_length")]
        public long DayLength { get; init; }
    }
}