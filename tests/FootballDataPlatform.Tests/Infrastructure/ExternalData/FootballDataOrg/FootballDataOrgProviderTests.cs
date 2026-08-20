using System.Net;
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
        Assert.Equal("123456", match.ExternalId); Assert.Equal("2021", match.CompetitionExternalId); Assert.Equal("999", match.SeasonExternalId); Assert.Equal("64", match.HomeTeamExternalId); Assert.Equal("65", match.AwayTeamExternalId);
        Assert.Equal(new DateTimeOffset(2025, 8, 15, 19, 0, 0, TimeSpan.Zero), match.ScheduledAt); Assert.Equal("FINISHED", match.Status); Assert.Equal("REGULAR_SEASON", match.Stage);
        Assert.Equal(2, match.HomeScore); Assert.Equal(1, match.AwayScore); Assert.Equal(1, match.HalfTimeHomeScore); Assert.Equal(0, match.HalfTimeAwayScore);
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

    [Fact]
    public async Task GetTeamsAsync_ShouldThrowHttpRequestException_WhenApiFails()
    {
        var provider = CreateProvider(new FakeHttpMessageHandler(HttpStatusCode.Unauthorized, "{\"message\":\"Invalid API token\"}"));
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => provider.GetTeamsAsync("PL", 2025));
        Assert.Contains("401", exception.Message); Assert.Contains("Invalid API token", exception.Message);
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

    private static FootballDataOrgProvider CreateProvider(FakeHttpMessageHandler handler, string token = "test-token")
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.football-data.org/") };
        var options = Options.Create(new FootballDataOrgOptions { ApiToken = token, BaseUrl = "https://api.football-data.org/" });
        return new FootballDataOrgProvider(client, options);
    }

    private sealed class FakeHttpMessageHandler(HttpStatusCode statusCode, string responseBody) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(statusCode) { Content = new StringContent(responseBody, System.Text.Encoding.UTF8, "application/json") });
        }
    }
}
