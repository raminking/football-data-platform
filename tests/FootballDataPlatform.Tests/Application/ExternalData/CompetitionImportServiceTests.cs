using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.ExternalData;
using FootballDataPlatform.Domain.Competitions;
using Moq;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class CompetitionImportServiceTests
{
    private static readonly ExternalCompetition ExternalCompetition =
        new("2021", "Premier League", "PL", "England");

    [Fact]
    public async Task ImportAsync_WhenIdentityDoesNotExist_CreatesCompetitionAndIdentity()
    {
        var source = CreateSource();
        var resolver = CreateResolver(source);
        var competitions = new Mock<ICompetitionRepository>();
        var identities = new Mock<IExternalIdentityRepository>();

        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "2021", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalIdentityRecord?)null);
        competitions.Setup(x => x.ExistsByIdentityAsync("Premier League", "England", "PL", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new CompetitionImportService(resolver.Object, competitions.Object, identities.Object);
        var result = await service.ImportAsync("football-data.org");

        Assert.Equal(1, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(0, result.Skipped);
        Assert.Empty(result.Errors);
        competitions.Verify(x => x.CreateAsync(It.Is<Competition>(c => c.Name == "Premier League" && c.Country == "England" && c.Code == "PL"), It.IsAny<CancellationToken>()), Times.Once);
        identities.Verify(x => x.AddAsync(It.Is<ExternalIdentityRecord>(i => i.Provider == "football-data.org" && i.EntityType == "Competition" && i.ExternalId == "2021"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_WhenIdentityExists_UpdatesExistingCompetition()
    {
        var competition = new Competition("Premier League", "England", "PL");
        var source = CreateSource();
        var resolver = CreateResolver(source);
        var competitions = new Mock<ICompetitionRepository>();
        var identities = new Mock<IExternalIdentityRepository>();

        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "2021", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Competition", "2021", competition.Id, DateTimeOffset.UtcNow));
        competitions.Setup(x => x.GetByIdAsync(competition.Id, It.IsAny<CancellationToken>())).ReturnsAsync(competition);
        competitions.Setup(x => x.ExistsByIdentityAsync("Premier League", "England", "PL", competition.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var service = new CompetitionImportService(resolver.Object, competitions.Object, identities.Object);
        var result = await service.ImportAsync("football-data.org");

        Assert.Equal(0, result.Created);
        Assert.Equal(1, result.Updated);
        Assert.Equal(0, result.Skipped);
        competitions.Verify(x => x.UpdateAsync(competition, It.IsAny<CancellationToken>()), Times.Once);
        competitions.Verify(x => x.CreateAsync(It.IsAny<Competition>(), It.IsAny<CancellationToken>()), Times.Never);
        identities.Verify(x => x.AddAsync(It.IsAny<ExternalIdentityRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ImportAsync_WhenIdentityPointsToMissingCompetition_SkipsAndReportsError()
    {
        var source = CreateSource();
        var resolver = CreateResolver(source);
        var competitions = new Mock<ICompetitionRepository>();
        var identities = new Mock<IExternalIdentityRepository>();

        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "2021", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Competition", "2021", Guid.NewGuid(), DateTimeOffset.UtcNow));
        competitions.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Competition?)null);

        var service = new CompetitionImportService(resolver.Object, competitions.Object, identities.Object);
        var result = await service.ImportAsync("football-data.org");

        Assert.Equal(0, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(1, result.Skipped);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task ImportAsync_WhenProviderCompetitionIsInvalid_SkipsIt()
    {
        var source = CreateSource(new ExternalCompetition("", "Premier League", "PL", "England"));
        var resolver = CreateResolver(source);
        var competitions = new Mock<ICompetitionRepository>();
        var identities = new Mock<IExternalIdentityRepository>();

        var service = new CompetitionImportService(resolver.Object, competitions.Object, identities.Object);
        var result = await service.ImportAsync("football-data.org");

        Assert.Equal(0, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(1, result.Skipped);
        competitions.Verify(x => x.CreateAsync(It.IsAny<Competition>(), It.IsAny<CancellationToken>()), Times.Never);
        identities.Verify(x => x.AddAsync(It.IsAny<ExternalIdentityRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IFootballDataSource> CreateSource(params ExternalCompetition[] competitions)
    {
        var source = new Mock<IFootballDataSource>();
        source.SetupGet(x => x.SourceKey).Returns("football-data.org");
        source.Setup(x => x.GetCompetitionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<ExternalCompetition>)(competitions.Length == 0 ? [ExternalCompetition] : competitions));
        return source;
    }

    private static Mock<IFootballDataSourceResolver> CreateResolver(Mock<IFootballDataSource> source)
    {
        var resolver = new Mock<IFootballDataSourceResolver>();
        resolver.Setup(x => x.Resolve("football-data.org")).Returns(source.Object);
        return resolver;
    }
}
