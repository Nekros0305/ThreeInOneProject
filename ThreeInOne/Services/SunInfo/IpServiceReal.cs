using Microsoft.Extensions.Options;
using ThreeInOne.Configuration.SunInfo;
using ThreeInOne.Interfaces.SunInfo;

namespace ThreeInOne.Services.SunInfo;

public class IpServiceReal : IIpService
{
    private readonly HttpClient _client;
    private readonly ILogger<IpServiceReal> _logger;

    public IpServiceReal(
        HttpClient client,
        //IConfiguration config,
        //IpServiceConfig ipServiceConfig,
        IOptions<IpServiceConfig> options,
        ILogger<IpServiceReal> logger)
    {
        _client = client;
        _logger = logger;
        //_client.BaseAddress = new Uri(config.GetValue<string>("IpServiceConfig:Url")!);
        //_client.BaseAddress = new Uri(ipServiceConfig.Url);
        _client.BaseAddress = new Uri(options.Value.Url);
    }

    public async Task<string> GetCurrentIp(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("IpService: Starting Connection");
            using HttpResponseMessage responseMessage = await _client.GetAsync(string.Empty, cancellationToken);
            responseMessage.EnsureSuccessStatusCode();

            string result = await responseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(result))
            {
                _logger.LogError("IpService: Failed to get Ip");
                throw new Exception("IpService was unable to get Ip");
            }

            _logger.LogInformation("IpService: Connection was successfull");
            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IpService: Failed on getting Ip");
            throw;
        }
    }
}