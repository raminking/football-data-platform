namespace FootballDataPlatform.Application.Abstractions.ExternalData;

/// <summary>
/// Provider-neutral contract for an external football data source.
/// </summary>
public interface IFootballDataSource
{
    string SourceKey { get; }

    Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ExternalSeason>> GetSeasonsAsync(
        string competitionCode,
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
