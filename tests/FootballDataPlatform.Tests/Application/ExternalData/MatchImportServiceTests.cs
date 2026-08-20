using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.ExternalData;
using FootballDataPlatform.Domain.Competitions;
using FootballDataPlatform.Domain.Match;
using FootballDataPlatform.Domain.Teams;
using FootballDataPlatform.Tests.Helpers;
using Moq;
using MatchEntity = FootballDataPlatform.Domain.Match.Match;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class MatchImportServiceTests
{
    [Fact]
    public async Task ImportAsync_WhenMatchIsNew_CreatesMatchAndIdentity()
    {
        var fixture = CreateFixture();
        fixture.MatchRepository.Setup(x => x.CreateAsync(It.IsAny<MatchEntity>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = CreateService(fixture);
        var result = await service.ImportAsync("football-data.org", "PL", 2025);

        Assert.Equal(1, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(0, result.Skipped);
        Assert.Empty(result.Errors);
        fixture.MatchRepository.Verify(x => x.CreateAsync(
            It.Is<MatchEntity>(m =>
                m.SeasonId == fixture.Season.Id &&
                m.HomeTeamId == fixture.HomeTeam.Id &&
                m.AwayTeamId == fixture.AwayTeam.Id &&
                m.HomeScore == 2 &&
                m.AwayScore == 1 &&
                m.Status == MatchStatus.Finished),
            It.IsAny<CancellationToken>()), Times.Once);
        fixture.Identities.Verify(x => x.AddAsync(
            It.Is<ExternalIdentityRecord>(i =>
                i.EntityType == "Match" && i.ExternalId == "match-1"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_WhenMatchIdentityExists_UpdatesMatch()
    {
        var fixture = CreateFixture();
        var existing = new MatchEntity(
            fixture.Season.Id,
            fixture.HomeTeam.Id,
            fixture.AwayTeam.Id,
            DateTimeOffset.UtcNow.AddDays(-1),
            MatchStage.League,
            MatchStatus.Scheduled).WithId(5);

        fixture.Identities.Setup(x => x.FindAsync(
                "football-data.org", "Match", "match-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord(
                "football-data.org", "Match", "match-1", existing.Id, DateTimeOffset.UtcNow));
        fixture.MatchRepository.Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = CreateService(fixture);
        var result = await service.ImportAsync("football-data.org", "PL", 2025);

        Assert.Equal(0, result.Created);
        Assert.Equal(1, result.Updated);
        Assert.Equal(0, result.Skipped);
        Assert.Equal(MatchStatus.Finished, existing.Status);
        Assert.Equal(2, existing.HomeScore);
        Assert.Equal(1, existing.AwayScore);
        fixture.MatchRepository.Verify(x => x.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        fixture.Identities.Verify(x => x.AddAsync(It.IsAny<ExternalIdentityRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ImportAsync_WhenTeamIdentityIsMissing_SkipsMatch()
    {
        var fixture = CreateFixture();
        fixture.Identities.Setup(x => x.FindAsync(
                "football-data.org", "Team", "away-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalIdentityRecord?)null);

        var service = CreateService(fixture);
        var result = await service.ImportAsync("football-data.org", "PL", 2025);

        Assert.Equal(0, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.Equal(1, result.Skipped);
        Assert.Single(result.Errors);
        fixture.MatchRepository.Verify(x => x.CreateAsync(It.IsAny<MatchEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static MatchFixture CreateFixture()
    {
        var competition = new Competition("Premier League", "England", "PL").WithId(1);
        var season = new FootballDataPlatform.Domain.Competitions.Season(
            competition.Id, "2025/26", new DateOnly(2025, 8, 15), new DateOnly(2026, 5, 24)).WithId(2);
        var home = new Team("Liverpool", "England").WithId(3);
        var away = new Team("Arsenal", "England").WithId(4);

        var source = new Mock<IFootballDataSource>();
        source.SetupGet(x => x.SourceKey).Returns("football-data.org");
        source.Setup(x => x.GetCompetitionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new ExternalCompetition("2021", "Premier League", "PL", "England") });
        source.Setup(x => x.GetMatchesAsync("PL", 2025, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new ExternalMatch(
                    "match-1", "2021", "2817", "home-1", "away-1",
                    new DateTimeOffset(2025, 9, 1, 15, 0, 0, TimeSpan.Zero),
                    "FINISHED", 2, 1, 1, 0, "REGULAR_SEASON")
            });

        var resolver = new Mock<IFootballDataSourceResolver>();
        resolver.Setup(x => x.Resolve("football-data.org")).Returns(source.Object);

        var competitions = new Mock<ICompetitionRepository>();
        competitions.Setup(x => x.GetByIdAsync(competition.Id, It.IsAny<CancellationToken>())).ReturnsAsync(competition);

        var seasons = new Mock<ISeasonRepository>();
        seasons.Setup(x => x.GetByIdAsync(season.Id, It.IsAny<CancellationToken>())).ReturnsAsync(season);

        var teams = new Mock<ITeamRepository>();
        teams.Setup(x => x.GetByIdAsync(home.Id, It.IsAny<CancellationToken>())).ReturnsAsync(home);
        teams.Setup(x => x.GetByIdAsync(away.Id, It.IsAny<CancellationToken>())).ReturnsAsync(away);

        var matches = new Mock<IMatchRepository>();
        var identities = new Mock<IExternalIdentityRepository>();
        identities.Setup(x => x.FindAsync("football-data.org", "Competition", "2021", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Competition", "2021", competition.Id, DateTimeOffset.UtcNow));
        identities.Setup(x => x.FindAsync("football-data.org", "Season", "2817", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Season", "2817", season.Id, DateTimeOffset.UtcNow));
        identities.Setup(x => x.FindAsync("football-data.org", "Team", "home-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Team", "home-1", home.Id, DateTimeOffset.UtcNow));
        identities.Setup(x => x.FindAsync("football-data.org", "Team", "away-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalIdentityRecord("football-data.org", "Team", "away-1", away.Id, DateTimeOffset.UtcNow));
        identities.Setup(x => x.FindAsync("football-data.org", "Match", "match-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalIdentityRecord?)null);

        return new MatchFixture(competition, season, home, away, resolver, competitions, seasons, teams, matches, identities);
    }

    private static IMatchImportService CreateService(MatchFixture fixture) =>
        new MatchImportService(
            fixture.Resolver.Object,
            fixture.MatchRepository.Object,
            fixture.Identities.Object,
            fixture.CompetitionRepository.Object,
            fixture.SeasonRepository.Object,
            fixture.TeamRepository.Object);

    private sealed record MatchFixture(
        Competition Competition,
        FootballDataPlatform.Domain.Competitions.Season Season,
        Team HomeTeam,
        Team AwayTeam,
        Mock<IFootballDataSourceResolver> Resolver,
        Mock<ICompetitionRepository> CompetitionRepository,
        Mock<ISeasonRepository> SeasonRepository,
        Mock<ITeamRepository> TeamRepository,
        Mock<IMatchRepository> MatchRepository,
        Mock<IExternalIdentityRepository> Identities);
}
