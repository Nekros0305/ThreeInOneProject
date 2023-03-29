using Newtonsoft.Json;
using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Models.SunInfo;

namespace ThreeInOne.Services.SunInfo;
internal class LocationServiceFromApi : ILocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationServiceFromApi> _logger;

    public LocationServiceFromApi(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationServiceFromApi> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<LocationInfo> GetLocation(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("LocationServiceApi: Starting Search");
            using var client = _httpClientFactory.CreateClient(nameof(LocationServiceFromApi));
            string url = $"/json";
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            LocationResultModel result = await response.Content.ReadAsAsync<LocationResultModel>();
            if (result.Status != "success")
                throw new Exception("LocationServiceApi: Api returned error");

            var location = new LocationInfo(
                result.City,
                result.Latitude,
                result.Longitute);

            _logger.LogInformation("LocationServiceApi: Search was successfull with response {@result}", result);
            return location;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LocationServiceApi: Failed To get Location");
            throw;
        }
    }

    private record LocationResultModel
    {
        [JsonProperty("lon")]
        public float Longitute { get; set; }
        [JsonProperty("lat")]
        public float Latitude { get; set; }
        public string City { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}