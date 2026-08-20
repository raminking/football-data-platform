using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.ExternalData;
using FootballDataPlatform.Domain.Teams;
using FootballDataPlatform.Tests.Helpers;
using Moq;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class TeamImportServiceTests
{
    private static readonly ExternalTeam ExternalTeam = new("64", "Liverpool FC", "England");

    [Fact]
    public async Task ImportAsync_WhenIdentityDoesNotExist_CreatesTeamAndIdentity()
    {
        var resolver = CreateResolver(); var teams = new Mock<ITeamRepository>(); var identities = new Mock<IExternalIdentityRepository>();
        var service = new TeamImportService(resolver.Object, teams.Object, identities.Object);
        identities.Setup(x => x.FindAsync("football-data.org", "Team", "64", It.IsAny<CancellationToken>())).ReturnsAsync((ExternalIdentityRecord?)null);
        var result = await service.ImportAsync("football-data.org", "PL", 2026);
        Assert.Equal(1, result.Created); Assert.Equal(0, result.Updated); Assert.Equal(0, result.Skipped); Assert.Empty(result.Errors);
        teams.Verify(x => x.CreateAsync(It.Is<Team>(t => t.Name == "Liverpool FC" && t.Country == "England"), It.IsAny<CancellationToken>()), Times.Once);
        identities.Verify(x => x.AddAsync(It.Is<ExternalIdentityRecord>(i => i.Provider == "football-data.org" && i.EntityType == "Team" && i.ExternalId == "64" && i.InternalEntityId > 0), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_WhenIdentityExists_UpdatesExistingTeamWithoutCreatingIdentity()
    {
        var team = new Team("Liverpool", "England").WithId(1); var resolver = CreateResolver(); var teams = new Mock<ITeamRepository>(); var identities = new Mock<IExternalIdentityRepository>();
        var service = new TeamImportService(resolver.Object, teams.Object, identities.Object);
        identities.Setup(x => x.FindAsync("football-data.org", "Team", "64", It.IsAny<CancellationToken>())).ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Team", "64", team.Id, DateTimeOffset.UtcNow));
        teams.Setup(x => x.GetByIdAsync(team.Id, It.IsAny<CancellationToken>())).ReturnsAsync(team);
        var result = await service.ImportAsync("football-data.org", "PL", 2026);
        Assert.Equal(0, result.Created); Assert.Equal(1, result.Updated); Assert.Equal("Liverpool FC", team.Name);
        teams.Verify(x => x.UpdateAsync(team, It.IsAny<CancellationToken>()), Times.Once); teams.Verify(x => x.CreateAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never); identities.Verify(x => x.AddAsync(It.IsAny<ExternalIdentityRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ImportAsync_WhenIdentityPointsToMissingTeam_SkipsAndReportsError()
    {
        var resolver = CreateResolver(); var teams = new Mock<ITeamRepository>(); var identities = new Mock<IExternalIdentityRepository>(); var service = new TeamImportService(resolver.Object, teams.Object, identities.Object);
        identities.Setup(x => x.FindAsync("football-data.org", "Team", "64", It.IsAny<CancellationToken>())).ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Team", "64", 1, DateTimeOffset.UtcNow));
        teams.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Team?)null);
        var result = await service.ImportAsync("football-data.org", "PL", 2026);
        Assert.Equal(0, result.Created); Assert.Equal(0, result.Updated); Assert.Equal(1, result.Skipped); Assert.Single(result.Errors);
    }

    [Fact]
    public async Task ImportAsync_WhenProviderTeamIsInvalid_SkipsIt()
    {
        var resolver = CreateResolver(new ExternalTeam("", "Liverpool FC", "England")); var teams = new Mock<ITeamRepository>(); var identities = new Mock<IExternalIdentityRepository>(); var service = new TeamImportService(resolver.Object, teams.Object, identities.Object);
        var result = await service.ImportAsync("football-data.org", "PL", 2026);
        Assert.Equal(0, result.Created); Assert.Equal(0, result.Updated); Assert.Equal(1, result.Skipped);
        teams.Verify(x => x.CreateAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never); identities.Verify(x => x.AddAsync(It.IsAny<ExternalIdentityRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IFootballDataSourceResolver> CreateResolver(params ExternalTeam[] teams)
    {
        var source = new Mock<IFootballDataSource>(); source.SetupGet(x => x.SourceKey).Returns("football-data.org");
        source.Setup(x => x.GetTeamsAsync("PL", 2026, It.IsAny<CancellationToken>())).ReturnsAsync((IReadOnlyCollection<ExternalTeam>)(teams.Length == 0 ? [ExternalTeam] : teams));
        var resolver = new Mock<IFootballDataSourceResolver>(); resolver.Setup(x => x.Resolve("football-data.org")).Returns(source.Object); return resolver;
    }
}