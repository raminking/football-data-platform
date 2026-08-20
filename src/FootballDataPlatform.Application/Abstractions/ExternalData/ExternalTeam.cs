namespace FootballDataPlatform.Application.Abstractions.ExternalData;

/// <summary>
/// Provider-neutral representation of an external team.
/// </summary>
public sealed record ExternalTeam(
    string ExternalId,
    string Name,
    string? Country);
