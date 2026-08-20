using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Infrastructure.ExternalData;

namespace FootballDataPlatform.Tests.Infrastructure.ExternalData;

public sealed class FootballDataSourceResolverTests
{
    [Fact]
    public void Resolve_WhenSourceIsRegistered_ReturnsSource()
    {
        var source = new FakeSource("football-data.org");
        var resolver = new FootballDataSourceResolver([source]);

        var result = resolver.Resolve("football-data.org");

        Assert.Same(source, result);
    }

    [Fact]
    public void Resolve_IsCaseInsensitive()
    {
        var source = new FakeSource("football-data.org");
        var resolver = new FootballDataSourceResolver([source]);

        var result = resolver.Resolve("FOOTBALL-DATA.ORG");

        Assert.Same(source, result);
    }

    [Fact]
    public void Resolve_WhenSourceIsUnknown_ThrowsKeyNotFoundException()
    {
        var resolver = new FootballDataSourceResolver([
            new FakeSource("football-data.org")
        ]);

        var exception = Assert.Throws<KeyNotFoundException>(() => resolver.Resolve("unknown"));

        Assert.Contains("unknown", exception.Message);
    }

    [Fact]
    public void Resolve_WhenSourceKeyIsEmpty_ThrowsArgumentException()
    {
        var resolver = new FootballDataSourceResolver([
            new FakeSource("football-data.org")
        ]);

        Assert.Throws<ArgumentException>(() => resolver.Resolve(" "));
    }

    [Fact]
    public void Constructor_WhenTwoSourcesUseSameKey_Throws()
    {
        var sources = new IFootballDataSource[]
        {
            new FakeSource("football-data.org"),
            new FakeSource("football-data.org")
        };

        Assert.Throws<ArgumentException>(() => new FootballDataSourceResolver(sources));
    }

    private sealed class FakeSource(string sourceKey) : IFootballDataSource
    {
        public string SourceKey { get; } = sourceKey;

        public Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExternalCompetition>>([]);

        public Task<IReadOnlyCollection<ExternalSeason>> GetSeasonsAsync(string competitionCode, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExternalSeason>>([]);

        public Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExternalTeam>>([]);

        public Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExternalMatch>>([]);
    }
}
