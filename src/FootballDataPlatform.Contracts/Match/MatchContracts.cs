using FootballDataPlatform.Domain.Match;

namespace FootballDataPlatform.Contracts.Match;

public sealed record CreateMatchRequest(
    Guid SeasonPublicId,
    Guid HomeTeamPublicId,
    Guid AwayTeamPublicId,
    DateTimeOffset ScheduledAt,
    MatchStage Stage,
    MatchStatus Status,
    int? HomeScore = null,
    int? AwayScore = null,
    int? HalfTimeHomeScore = null,
    int? HalfTimeAwayScore = null);

public sealed record MatchResponse(
    Guid PublicId,
    DateTimeOffset ScheduledAt,
    MatchStage Stage,
    MatchStatus Status,
    int? HomeScore,
    int? AwayScore,
    int? HalfTimeHomeScore,
    int? HalfTimeAwayScore,
    MatchResult? Result);