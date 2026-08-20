namespace FootballDataPlatform.Domain.Match;

public enum MatchStage { League, GroupStage, LeaguePhase, Playoff, RoundOf16, QuarterFinal, SemiFinal, Final, Friendly }
public enum MatchStatus { Scheduled, InProgress, Finished, Postponed, Cancelled, Abandoned }
public enum MatchResult { HomeWin, Draw, AwayWin }

public sealed class Match
{
    public long Id { get; private set; }
    public Guid PublicId { get; private set; }
    public long SeasonId { get; private set; }
    public long HomeTeamId { get; private set; }
    public long AwayTeamId { get; private set; }
    public DateTimeOffset ScheduledAt { get; private set; }
    public MatchStage Stage { get; private set; }
    public MatchStatus Status { get; private set; }
    public int? HomeScore { get; private set; }
    public int? AwayScore { get; private set; }
    public int? HalfTimeHomeScore { get; private set; }
    public int? HalfTimeAwayScore { get; private set; }
    public MatchResult? Result { get; private set; }

    public Match(long seasonId, long homeTeamId, long awayTeamId, DateTimeOffset scheduledAt, MatchStage stage, MatchStatus status,
        int? homeScore = null, int? awayScore = null, int? halfTimeHomeScore = null, int? halfTimeAwayScore = null)
    {
        ValidateIdentity(seasonId, homeTeamId, awayTeamId);
        ValidateScores(homeScore, awayScore, halfTimeHomeScore, halfTimeAwayScore);
        PublicId = Guid.NewGuid();
        SeasonId = seasonId;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        ScheduledAt = scheduledAt;
        Stage = stage;
        Status = status;
        HomeScore = homeScore;
        AwayScore = awayScore;
        HalfTimeHomeScore = halfTimeHomeScore;
        HalfTimeAwayScore = halfTimeAwayScore;
        Result = CalculateResult(homeScore, awayScore);
        ValidateFinishedState();
    }

    private Match() { }

    public void UpdateDetails(DateTimeOffset scheduledAt, MatchStage stage, MatchStatus status, int? homeScore, int? awayScore,
        int? halfTimeHomeScore, int? halfTimeAwayScore)
    {
        ValidateScores(homeScore, awayScore, halfTimeHomeScore, halfTimeAwayScore);
        ScheduledAt = scheduledAt; Stage = stage; Status = status; HomeScore = homeScore; AwayScore = awayScore;
        HalfTimeHomeScore = halfTimeHomeScore; HalfTimeAwayScore = halfTimeAwayScore;
        Result = CalculateResult(homeScore, awayScore);
        ValidateFinishedState();
    }

    private void ValidateFinishedState()
    {
        if (Status == MatchStatus.Finished && (HomeScore is null || AwayScore is null))
            throw new ArgumentException("Finished matches require final scores.", nameof(Status));
    }

    private static void ValidateIdentity(long seasonId, long homeTeamId, long awayTeamId)
    {
        if (seasonId <= 0) throw new ArgumentException("Season is required.", nameof(seasonId));
        if (homeTeamId <= 0) throw new ArgumentException("Home team is required.", nameof(homeTeamId));
        if (awayTeamId <= 0) throw new ArgumentException("Away team is required.", nameof(awayTeamId));
        if (homeTeamId == awayTeamId) throw new ArgumentException("Home and away teams must be different.", nameof(awayTeamId));
    }

    private static void ValidateScores(int? homeScore, int? awayScore, int? halfTimeHomeScore, int? halfTimeAwayScore)
    {
        if (homeScore < 0 || awayScore < 0 || halfTimeHomeScore < 0 || halfTimeAwayScore < 0)
            throw new ArgumentException("Scores cannot be negative.");
        if (halfTimeHomeScore.HasValue && homeScore.HasValue && halfTimeHomeScore > homeScore)
            throw new ArgumentException("Half-time home score cannot exceed final home score.");
        if (halfTimeAwayScore.HasValue && awayScore.HasValue && halfTimeAwayScore > awayScore)
            throw new ArgumentException("Half-time away score cannot exceed final away score.");
        if (halfTimeHomeScore.HasValue != halfTimeAwayScore.HasValue)
            throw new ArgumentException("Both half-time scores must be provided together.");
        if (homeScore.HasValue != awayScore.HasValue)
            throw new ArgumentException("Both final scores must be provided together.");
    }

    private static MatchResult? CalculateResult(int? homeScore, int? awayScore) =>
        !homeScore.HasValue || !awayScore.HasValue ? null : homeScore > awayScore ? MatchResult.HomeWin : homeScore < awayScore ? MatchResult.AwayWin : MatchResult.Draw;
}