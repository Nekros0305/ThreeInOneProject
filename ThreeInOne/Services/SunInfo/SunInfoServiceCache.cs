using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Services.SunInfo;

internal class SunInfoServiceCache : ISunInfoService
{
    private readonly ILocationService _locationService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SunInfoServiceReal> _logger;
    private readonly IMemoryCache _cache;

    public SunInfoServiceCache(
        ILocationService locationService,
        IHttpClientFactory httpClientFactory,
        ILogger<SunInfoServiceReal> logger,
        IMemoryCache cache)
    {
        _locationService = locationService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _cache = cache;
    }

    public async Task<LocalSunInfo> GetSunInfo(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("SunInfoService: begginning gathering required informations");
            var location = await _locationService.GetLocation(cancellationToken);

            var cacheKey = MakeCacheKey(DateTime.Today, location.Longitude, location.Latitude);

            if (_cache.Get<LocalSunInfo>(cacheKey) is { } cached)
            {
                _logger.LogInformation("SunInfoService: got value from cache");
                return cached;
            }

            _logger.LogInformation("Creating Http Client");
            using var client = _httpClientFactory.CreateClient(nameof(SunInfoServiceCache));

            string url = $"/json?lat={location.Latitude}&lng={location.Longitude}&formatted=0";

            _logger.LogInformation("SunInfoService: Connecting To {DestinationUrl}", client.BaseAddress + url);
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<SunResultModel>();
            if (result.Status != "OK")
            {
                _logger.LogError("SunInfoService: Failed at converting response");
                throw new Exception("Api returned error");
            }

            _logger.LogInformation("SunInfoService: Successed on creating LocalSunInfo");

            var sunInfo = new LocalSunInfo(
                result.Results.Sunrise.ToLocalTime().TimeOfDay,
                result.Results.Sunset.ToLocalTime().TimeOfDay,
                location.City,
                TimeSpan.FromSeconds(result.Results.DayLength),
                DateTime.Now);

            _logger.LogInformation("SunInfoService: Adding value to cache");
            _cache.Set(cacheKey, sunInfo, TimeSpan.FromDays(1));
            return sunInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SunInfoService: Failed at Loading SunInfo");
            throw;
        }
    }

    private string MakeCacheKey(DateTime time, float longitude, float latitude, int precision = 3)
        => $"{string.Format("{0:dd.mm.yy}", time)}|{Math.Round(longitude, precision)}|{Math.Round(latitude, precision)}";

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