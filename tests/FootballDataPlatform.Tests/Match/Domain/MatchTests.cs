using FootballDataPlatform.Domain.Match;

namespace FootballDataPlatform.Tests.Match.Domain;

public class MatchTests
{
    private const long SeasonId = 1;
    private const long HomeTeamId = 10;
    private const long AwayTeamId = 20;
    private static readonly DateTimeOffset ScheduledAt = new(2026, 8, 18, 19, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_ShouldCalculateHomeWin()
    {
        var match = new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, AwayTeamId, ScheduledAt, MatchStage.League, MatchStatus.Finished, 3, 1, 1, 0);
        Assert.Equal(0, match.Id);
        Assert.NotEqual(Guid.Empty, match.PublicId);
        Assert.Equal(MatchResult.HomeWin, match.Result);
    }

    [Fact]
    public void Create_WithDraw_ShouldCalculateDraw()
    {
        var match = new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, AwayTeamId, ScheduledAt, MatchStage.League, MatchStatus.Finished, 1, 1, 0, 0);
        Assert.Equal(MatchResult.Draw, match.Result);
    }

    [Fact]
    public void Create_WithAwayWin_ShouldCalculateAwayWin()
    {
        var match = new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, AwayTeamId, ScheduledAt, MatchStage.League, MatchStatus.Finished, 0, 2, 0, 1);
        Assert.Equal(MatchResult.AwayWin, match.Result);
    }

    [Fact]
    public void Create_WithSameTeams_ShouldThrow() => Assert.Throws<ArgumentException>(() => new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, HomeTeamId, ScheduledAt, MatchStage.League, MatchStatus.Scheduled));

    [Fact]
    public void Create_WithNegativeScore_ShouldThrow() => Assert.Throws<ArgumentException>(() => new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, AwayTeamId, ScheduledAt, MatchStage.League, MatchStatus.Finished, -1, 0));

    [Fact]
    public void Create_FinishedWithoutFinalScore_ShouldThrow() => Assert.Throws<ArgumentException>(() => new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, AwayTeamId, ScheduledAt, MatchStage.League, MatchStatus.Finished));

    [Fact]
    public void Create_WithHalfTimeScoreGreaterThanFinal_ShouldThrow() => Assert.Throws<ArgumentException>(() => new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, AwayTeamId, ScheduledAt, MatchStage.League, MatchStatus.Finished, 1, 0, 2, 0));

    [Fact]
    public void Create_ScheduledWithoutScores_ShouldHaveNoResult()
    {
        var match = new FootballDataPlatform.Domain.Match.Match(SeasonId, HomeTeamId, AwayTeamId, ScheduledAt, MatchStage.League, MatchStatus.Scheduled);
        Assert.Null(match.HomeScore);
        Assert.Null(match.AwayScore);
        Assert.Null(match.Result);
    }
}