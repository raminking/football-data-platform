using System.Net;
using Microsoft.Extensions.Options;

namespace FootballDataPlatform.Infrastructure.ExternalData;

public sealed class ExternalDataRetryHandler(IOptions<ExternalDataRetryOptions> options) : DelegatingHandler
{
    private readonly ExternalDataRetryOptions _options = options.Value;
    private static readonly Random Random = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (true)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                if (!ShouldRetry(response, attempt))
                    return response;

                var delay = GetDelay(response, attempt);
                response.Dispose();
                await Task.Delay(delay, cancellationToken);
                attempt++;
            }
            catch (HttpRequestException) when (attempt < _options.MaxRetries && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(GetDelay(null, attempt), cancellationToken);
                attempt++;
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested && attempt < _options.MaxRetries)
            {
                await Task.Delay(GetDelay(null, attempt), cancellationToken);
                attempt++;
            }
        }
    }

    private bool ShouldRetry(HttpResponseMessage response, int attempt) =>
        attempt < _options.MaxRetries &&
        (response.StatusCode == HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500);

    private TimeSpan GetDelay(HttpResponseMessage? response, int attempt)
    {
        if (response?.Headers.RetryAfter?.Delta is { } retryAfter)
            return retryAfter <= TimeSpan.FromSeconds(_options.MaxDelaySeconds)
                ? retryAfter
                : TimeSpan.FromSeconds(_options.MaxDelaySeconds);

        var exponentialSeconds = Math.Min(
            _options.MaxDelaySeconds,
            _options.BaseDelaySeconds * Math.Pow(2, attempt));

        var jitter = exponentialSeconds * _options.JitterRatio * Random.Shared.NextDouble();
        return TimeSpan.FromSeconds(Math.Min(_options.MaxDelaySeconds, exponentialSeconds + jitter));
    }
}
