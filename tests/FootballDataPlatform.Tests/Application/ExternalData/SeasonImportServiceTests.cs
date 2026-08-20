using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.ExternalData;
using FootballDataPlatform.Domain.Competitions;
using Moq;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class SeasonImportServiceTests
{
    private static readonly ExternalSeason ExternalSeason =
        new("2817", "2021", "2025/26", new DateOnly(2025, 8, 15), new DateOnly(2026, 5, 24));

    [Fact]
    public async Task ImportAsync_WhenCompetitionAndSeasonAreNew_CreatesSeasonAndIdentity()
    {
        var competition = new Competition("Premier League", "England", "PL");
        var provider = CreateProvider();
        var competitions = new Mock<ICompetitionRepository>();
        var seasons = new Mock<ISeasonRepository>();
        var identities = new Mock<IExternalIdentityRepository>();

        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "PL", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalIdentityRecord?)null);
        provider.Setup(x => x.GetCompetitionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new ExternalCompetition("2021", "Premier League", "PL", "England") });
        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "2021", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Competition", "2021", competition.Id, DateTimeOffset.UtcNow));
        competitions.Setup(x => x.GetByIdAsync(competition.Id, It.IsAny<CancellationToken>())).ReturnsAsync(competition);
        identities.Setup(x => x.FindAsync("football-data.org", "Season", "2817", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalIdentityRecord?)null);
        seasons.Setup(x => x.ExistsByIdentityAsync(competition.Id, "2025/26", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var service = new SeasonImportService(provider.Object, competitions.Object, seasons.Object, identities.Object);
        var result = await service.ImportAsync("PL");

        Assert.Equal(1, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(0, result.Skipped);
        Assert.Empty(result.Errors);
        seasons.Verify(x => x.CreateAsync(It.Is<Season>(s => s.CompetitionId == competition.Id && s.Name == "2025/26"), It.IsAny<CancellationToken>()), Times.Once);
        identities.Verify(x => x.AddAsync(It.Is<ExternalIdentityRecord>(i => i.EntityType == "Season" && i.ExternalId == "2817"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_WhenSeasonIdentityExists_UpdatesSeason()
    {
        var competition = new Competition("Premier League", "England", "PL");
        var season = new Season(competition.Id, "2025/26", new DateOnly(2025, 8, 15), new DateOnly(2026, 5, 24));
        var provider = CreateProvider();
        var competitions = new Mock<ICompetitionRepository>();
        var seasons = new Mock<ISeasonRepository>();
        var identities = new Mock<IExternalIdentityRepository>();

        SetupCompetition(competition, provider, competitions, identities);
        identities.Setup(x => x.FindAsync("football-data.org", "Season", "2817", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Season", "2817", season.Id, DateTimeOffset.UtcNow));
        seasons.Setup(x => x.GetByIdAsync(season.Id, It.IsAny<CancellationToken>())).ReturnsAsync(season);
        seasons.Setup(x => x.ExistsByIdentityAsync(competition.Id, "2025/26", season.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var service = new SeasonImportService(provider.Object, competitions.Object, seasons.Object, identities.Object);
        var result = await service.ImportAsync("PL");

        Assert.Equal(0, result.Created);
        Assert.Equal(1, result.Updated);
        Assert.Empty(result.Errors);
        seasons.Verify(x => x.UpdateAsync(season, It.IsAny<CancellationToken>()), Times.Once);
        identities.Verify(x => x.AddAsync(It.IsAny<ExternalIdentityRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ImportAsync_WhenCompetitionCannotBeResolved_SkipsAllSeasons()
    {
        var provider = CreateProvider();
        var competitions = new Mock<ICompetitionRepository>();
        var seasons = new Mock<ISeasonRepository>();
        var identities = new Mock<IExternalIdentityRepository>();
        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "PL", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalIdentityRecord?)null);
        provider.Setup(x => x.GetCompetitionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ExternalCompetition>());

        var service = new SeasonImportService(provider.Object, competitions.Object, seasons.Object, identities.Object);
        var result = await service.ImportAsync("PL");

        Assert.Equal(0, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(1, result.Skipped);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task ImportAsync_WhenSeasonIsInvalid_SkipsIt()
    {
        var competition = new Competition("Premier League", "England", "PL");
        var provider = CreateProvider(new ExternalSeason("", "2021", "2025/26", new DateOnly(2025, 8, 15), new DateOnly(2026, 5, 24)));
        var competitions = new Mock<ICompetitionRepository>();
        var seasons = new Mock<ISeasonRepository>();
        var identities = new Mock<IExternalIdentityRepository>();
        SetupCompetition(competition, provider, competitions, identities);

        var service = new SeasonImportService(provider.Object, competitions.Object, seasons.Object, identities.Object);
        var result = await service.ImportAsync("PL");

        Assert.Equal(0, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(1, result.Skipped);
        Assert.Single(result.Errors);
    }

    private static Mock<IFootballDataProvider> CreateProvider(ExternalSeason? season = null)
    {
        var provider = new Mock<IFootballDataProvider>();
        provider.SetupGet(x => x.ProviderName).Returns("football-data.org");
        provider.Setup(x => x.GetSeasonsAsync("PL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { season ?? ExternalSeason });
        return provider;
    }

    private static void SetupCompetition(
        Competition competition,
        Mock<IFootballDataProvider> provider,
        Mock<ICompetitionRepository> competitions,
        Mock<IExternalIdentityRepository> identities)
    {
        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "PL", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalIdentityRecord?)null);
        provider.Setup(x => x.GetCompetitionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new ExternalCompetition("2021", "Premier League", "PL", "England") });
        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "2021", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Competition", "2021", competition.Id, DateTimeOffset.UtcNow));
        competitions.Setup(x => x.GetByIdAsync(competition.Id, It.IsAny<CancellationToken>())).ReturnsAsync(competition);
    }
}
