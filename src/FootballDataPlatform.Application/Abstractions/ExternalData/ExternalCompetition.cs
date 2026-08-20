namespace FootballDataPlatform.Application.Abstractions.ExternalData;

/// <summary>
/// Provider-neutral representation of an external competition.
/// </summary>
public sealed record ExternalCompetition(
    string ExternalId,
    string Name,
    string Code,
    string? Country);
