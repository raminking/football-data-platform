using System.Net;
using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg;
using Microsoft.Extensions.Options;

namespace FootballDataPlatform.Tests.Infrastructure.ExternalData.FootballDataOrg;

public sealed class FootballDataOrgProviderTests
{
    [Fact]
    public async Task GetCompetitionsAsync_ShouldMapValidCompetitions()
    {
        const string json = """
        {"competitions":[{"id":2021,"name":"Premier League","code":"PL","area":{"name":"England"}},{"id":2001,"name":"UEFA Champions League","code":"CL","area":{"name":"Europe"}}]}
        """;
        var provider = CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, json));
        var result = await provider.GetCompetitionsAsync();
        Assert.Equal(2, result.Count);
        var premierLeague = result.Single(x => x.ExternalId == "2021");
        Assert.Equal("Premier League", premierLeague.Name); Assert.Equal("PL", premierLeague.Code); Assert.Equal("England", premierLeague.Country);
    }

    [Fact]
    public async Task GetCompetitionsAsync_ShouldIgnoreInvalidItems()
    {
        const string json = """{"competitions":[{"id":2021,"name":"Premier League","code":"PL"},{"id":0,"name":"Invalid Competition"},{"id":2022,"name":""}]}""";
        var result = await CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, json)).GetCompetitionsAsync();
        var competition = Assert.Single(result); Assert.Equal("2021", competition.ExternalId); Assert.Equal("Premier League", competition.Name);
    }

    [Fact]
    public async Task GetTeamsAsync_ShouldBuildExpectedRequestAndMapTeams()
    {
        const string json = """{"teams":[{"id":64,"name":"Liverpool FC","area":{"name":"England"}},{"id":65,"name":"Manchester City FC","area":{"name":"England"}}]}""";
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, json);
        var result = await CreateProvider(handler).GetTeamsAsync("PL", 2025);
        Assert.Equal(2, result.Count); Assert.Equal("/v4/competitions/PL/teams?season=2025", handler.LastRequest?.RequestUri?.PathAndQuery);
        var liverpool = result.Single(x => x.ExternalId == "64"); Assert.Equal("Liverpool FC", liverpool.Name); Assert.Equal("England", liverpool.Country);
    }

    [Fact]
    public async Task GetMatchesAsync_ShouldMapMatchAndScores()
    {
        const string json = """{"matches":[{"id":123456,"utcDate":"2025-08-15T19:00:00Z","status":"FINISHED","stage":"REGULAR_SEASON","competition":{"id":2021},"season":{"id":999},"homeTeam":{"id":64},"awayTeam":{"id":65},"score":{"halfTime":{"home":1,"away":0},"fullTime":{"home":2,"away":1}}}]}""";
        var result = await CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, json)).GetMatchesAsync("PL", 2025);
        var match = Assert.Single(result);
        Assert.Equal("123456", match.ExternalId); Assert.Equal("2021", match.ExternalCompetitionId); Assert.Equal("999", match.ExternalSeasonId); Assert.Equal("64", match.ExternalHomeTeamId); Assert.Equal("65", match.ExternalAwayTeamId);
        Assert.Equal(new DateTimeOffset(2025, 8, 15, 19, 0, 0, TimeSpan.Zero), match.UtcDate); Assert.Equal("FINISHED", match.Status); Assert.Equal("REGULAR_SEASON", match.Stage);
        Assert.Equal(2, match.FullTimeHome); Assert.Equal(1, match.FullTimeAway); Assert.Equal(1, match.HalfTimeHome); Assert.Equal(0, match.HalfTimeAway);
    }

    [Fact]
    public async Task GetMatchesAsync_ShouldIgnoreInvalidMatches()
    {
        const string json = """{"matches":[{"id":100,"utcDate":"2025-08-15T19:00:00Z","status":"FINISHED","competition":{"id":2021},"season":{"id":999},"homeTeam":{"id":64},"awayTeam":{"id":65}},{"id":101,"utcDate":"2025-08-16T19:00:00Z","status":"","competition":{"id":2021},"season":{"id":999},"homeTeam":{"id":64},"awayTeam":{"id":65}}]}""";
        var result = await CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, json)).GetMatchesAsync("PL", 2025);
        Assert.Equal("100", Assert.Single(result).ExternalId);
    }

    [Fact] public async Task GetTeamsAsync_ShouldRejectEmptyCompetitionCode() => await Assert.ThrowsAsync<ArgumentException>(() => CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"teams\":[]}")).GetTeamsAsync("", 2025));
    [Fact] public async Task GetTeamsAsync_ShouldRejectInvalidSeason() => await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"teams\":[]}")).GetTeamsAsync("PL", 0));
    [Fact] public async Task GetMatchesAsync_ShouldRejectInvalidSeason() => await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"matches\":[]}")).GetMatchesAsync("PL", 0));

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, ExternalDataErrorCategory.Authentication)]
    [InlineData(HttpStatusCode.Forbidden, ExternalDataErrorCategory.Authentication)]
    [InlineData(HttpStatusCode.TooManyRequests, ExternalDataErrorCategory.RateLimited)]
    [InlineData(HttpStatusCode.InternalServerError, ExternalDataErrorCategory.ServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable, ExternalDataErrorCategory.ServerError)]
    [InlineData(HttpStatusCode.BadRequest, ExternalDataErrorCategory.InvalidResponse)]
    public async Task GetTeamsAsync_ShouldClassifyHttpFailures(HttpStatusCode statusCode, ExternalDataErrorCategory expectedCategory)
    {
        var provider = CreateProvider(new FakeHttpMessageHandler(statusCode, "{\"message\":\"provider error\"}"));
        var exception = await Assert.ThrowsAsync<ExternalDataException>(() => provider.GetTeamsAsync("PL", 2025));
        Assert.Equal(expectedCategory, exception.Category);
        Assert.Equal("football-data.org", exception.SourceKey);
        Assert.Equal(nameof(provider.GetTeamsAsync), exception.Operation);
        Assert.Equal((int)statusCode, exception.StatusCode);
        Assert.DoesNotContain("test-token", exception.Message);
        Assert.DoesNotContain("provider error", exception.Message);
    }

    [Fact]
    public async Task GetTeamsAsync_ShouldClassifyInvalidJson()
    {
        var provider = CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, "not-json"));
        var exception = await Assert.ThrowsAsync<ExternalDataException>(() => provider.GetTeamsAsync("PL", 2025));
        Assert.Equal(ExternalDataErrorCategory.InvalidResponse, exception.Category);
        Assert.Equal("football-data.org", exception.SourceKey);
        Assert.Equal(nameof(provider.GetTeamsAsync), exception.Operation);
    }

    [Fact]
    public async Task GetTeamsAsync_ShouldClassifyTimeout()
    {
        var provider = CreateProvider(new FakeHttpMessageHandler(new TaskCanceledException("request timeout")));
        var exception = await Assert.ThrowsAsync<ExternalDataException>(() => provider.GetTeamsAsync("PL", 2025));
        Assert.Equal(ExternalDataErrorCategory.Timeout, exception.Category);
    }

    [Fact]
    public async Task GetTeamsAsync_ShouldClassifyNetworkFailure()
    {
        var provider = CreateProvider(new FakeHttpMessageHandler(new HttpRequestException("connection failed")));
        var exception = await Assert.ThrowsAsync<ExternalDataException>(() => provider.GetTeamsAsync("PL", 2025));
        Assert.Equal(ExternalDataErrorCategory.Network, exception.Category);
    }

    [Fact]
    public async Task GetTeamsAsync_ShouldPreserveCancellation()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var provider = CreateProvider(new FakeHttpMessageHandler(new OperationCanceledException()));

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => provider.GetTeamsAsync("PL", 2025, cts.Token));
        Assert.IsType<TaskCanceledException>(exception);
    }

    [Fact]
    public async Task Provider_ShouldSendAuthenticationToken()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"teams\":[]}");
        await CreateProvider(handler, "test-token").GetTeamsAsync("PL", 2025);
        Assert.NotNull(handler.LastRequest); Assert.True(handler.LastRequest!.Headers.TryGetValues("X-Auth-Token", out var values)); Assert.Equal("test-token", Assert.Single(values));
    }

    [Fact]
    public void SourceKey_ShouldBeFootballDataOrg()
    {
        var provider = CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, "{}"));
        Assert.Equal("football-data.org", provider.SourceKey);
    }

    private static FootballDataOrgProvider CreateProvider(HttpMessageHandler handler, string token = "test-token")
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.football-data.org/") };
        var options = Options.Create(new FootballDataOrgOptions { ApiToken = token, BaseUrl = "https://api.football-data.org/" });
        return new FootballDataOrgProvider(client, options);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode? _statusCode;
        private readonly string? _responseBody;
        private readonly Exception? _exception;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseBody)
        {
            _statusCode = statusCode;
            _responseBody = responseBody;
        }

        public FakeHttpMessageHandler(Exception exception) => _exception = exception;
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (_exception is not null)
                return Task.FromException<HttpResponseMessage>(_exception);

            return Task.FromResult(new HttpResponseMessage(_statusCode!.Value)
            {
                Content = new StringContent(_responseBody!, System.Text.Encoding.UTF8, "application/json")
            });
        }
    }
}
