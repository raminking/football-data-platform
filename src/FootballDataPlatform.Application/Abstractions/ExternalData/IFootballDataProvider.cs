namespace FootballDataPlatform.Application.Abstractions.ExternalData;

public interface IFootballDataProvider
{
    string ProviderName { get; }

    Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default);
}

public sealed record ExternalCompetition(
    string ExternalId,
    string Name,
    string? Code,
    string? Country);

public sealed record ExternalTeam(
    string ExternalId,
    string Name,
    string? Country);

public sealed record ExternalMatch(
    string ExternalId,
    string CompetitionExternalId,
    string SeasonExternalId,
    string HomeTeamExternalId,
    string AwayTeamExternalId,
    DateTimeOffset ScheduledAt,
    string Status,
    int? HomeScore,
    int? AwayScore,
    int? HalfTimeHomeScore,
    int? HalfTimeAwayScore,
    string? Stage);
