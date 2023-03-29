using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace ThreeInOne.Policies.Suninfo;

public static class HttpCientPolicy
{

    //.AddTransientHttpErrorPolicy(p =>
    //    p.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3)))

    public static IHttpClientBuilder SingleTimeoutWithRetryPolicy(this IHttpClientBuilder cb,
        TimeSpan? timeoutAfter = null, TimeSpan? retryTime = null, int attemps = 2)
    {
        timeoutAfter ??= TimeSpan.FromSeconds(10);
        retryTime ??= TimeSpan.FromSeconds(2);

        return cb.AddPolicyHandler(
        Policy.TimeoutAsync(timeoutAfter.Value)
        .AsAsyncPolicy<HttpResponseMessage>()
        .WrapAsync(
            HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(retryTime.Value, attemps))));
    }

    public static IHttpClientBuilder RetryWithTimeOutPolicy(this IHttpClientBuilder cb,
    TimeSpan? timeoutAfter = null, TimeSpan? retryTime = null, int attemps = 2)
    {
        timeoutAfter ??= TimeSpan.FromSeconds(10);
        retryTime ??= TimeSpan.FromSeconds(2);

        return cb.AddPolicyHandler(
            HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(retryTime.Value, attemps))
            .WrapAsync(
                Policy
                .TimeoutAsync(timeoutAfter.Value)
                .AsAsyncPolicy<HttpResponseMessage>()));
    }
}
