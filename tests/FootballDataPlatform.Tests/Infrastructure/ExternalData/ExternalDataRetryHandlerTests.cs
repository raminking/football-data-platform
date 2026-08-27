using System.Net;
using System.Net.Http.Headers;
using FootballDataPlatform.Infrastructure.ExternalData;
using Microsoft.Extensions.Options;

namespace FootballDataPlatform.Tests.Infrastructure.ExternalData;

public sealed class ExternalDataRetryHandlerTests
{
    [Fact]
    public async Task ShouldRetryServerErrorUntilSuccess()
    {
        var handler = new RecordingHandler(
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            new HttpResponseMessage(HttpStatusCode.OK));
        var client = CreateClient(handler, maxRetries: 2);

        var response = await client.GetAsync("test");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task ShouldRetryRateLimitAndRespectRetryAfter()
    {
        var first = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        first.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.Zero);
        var handler = new RecordingHandler(first, new HttpResponseMessage(HttpStatusCode.OK));
        var client = CreateClient(handler, maxRetries: 1);

        var response = await client.GetAsync("test");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, handler.RequestCount);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task ShouldNotRetryNonRetryableStatus(HttpStatusCode statusCode)
    {
        var handler = new RecordingHandler(new HttpResponseMessage(statusCode));
        var client = CreateClient(handler, maxRetries: 2);

        var response = await client.GetAsync("test");

        Assert.Equal(statusCode, response.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task ShouldStopAfterConfiguredRetryCount()
    {
        var handler = new RecordingHandler(
            new HttpResponseMessage(HttpStatusCode.InternalServerError),
            new HttpResponseMessage(HttpStatusCode.InternalServerError),
            new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var client = CreateClient(handler, maxRetries: 2);

        var response = await client.GetAsync("test");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(3, handler.RequestCount);
    }

    [Fact]
    public async Task ShouldRetryNetworkFailure()
    {
        var handler = new RecordingHandler(
            new HttpRequestException("temporary failure"),
            new HttpResponseMessage(HttpStatusCode.OK));
        var client = CreateClient(handler, maxRetries: 1);

        var response = await client.GetAsync("test");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task ShouldPreserveCancellation()
    {
        var handler = new RecordingHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var client = CreateClient(handler, maxRetries: 2);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.GetAsync("test", cts.Token));
    }

    private static HttpClient CreateClient(HttpMessageHandler innerHandler, int maxRetries)
    {
        var options = Options.Create(new ExternalDataRetryOptions
        {
            MaxRetries = maxRetries,
            BaseDelaySeconds = 0,
            MaxDelaySeconds = 1,
            JitterRatio = 0
        });

        var retryHandler = new ExternalDataRetryHandler(options) { InnerHandler = innerHandler };
        return new HttpClient(retryHandler) { BaseAddress = new Uri("https://example.test/") };
    }

    private sealed class RecordingHandler(params object[] outcomes) : HttpMessageHandler
    {
        private readonly object[] _outcomes = outcomes;
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            var outcome = _outcomes[Math.Min(RequestCount - 1, _outcomes.Length - 1)];
            return outcome switch
            {
                HttpResponseMessage response => Task.FromResult(response),
                Exception exception => Task.FromException<HttpResponseMessage>(exception),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
