namespace FootballDataPlatform.Application.Abstractions.ExternalData;

/// <summary>
/// Provider-neutral representation of an external match.
/// </summary>
public sealed record ExternalMatch(
    string ExternalId,
    string ExternalCompetitionId,
    string ExternalSeasonId,
    string ExternalHomeTeamId,
    string ExternalAwayTeamId,
    DateTimeOffset UtcDate,
    string Status,
    int? FullTimeHome,
    int? FullTimeAway,
    int? HalfTimeHome,
    int? HalfTimeAway,
    string? Stage);
