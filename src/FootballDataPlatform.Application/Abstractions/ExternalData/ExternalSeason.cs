namespace FootballDataPlatform.Application.Abstractions.ExternalData;

/// <summary>
/// Provider-neutral representation of an external season.
/// </summary>
public sealed record ExternalSeason(
    string ExternalId,
    string ExternalCompetitionId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate);
