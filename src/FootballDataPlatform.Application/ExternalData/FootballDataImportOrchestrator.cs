namespace FootballDataPlatform.Application.ExternalData;

public interface IFootballDataImportOrchestrator
{
    Task<FootballDataImportResult> ImportCompetitionAsync(
        string sourceKey,
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default);
}

public sealed record FootballDataImportResult(
    CompetitionImportResult Competition,
    SeasonImportResult Season,
    TeamImportResult Teams,
    MatchImportResult Matches)
{
    public int Created => Competition.Created + Season.Created + Teams.Created + Matches.Created;
    public int Updated => Competition.Updated + Season.Updated + Teams.Updated + Matches.Updated;
    public int Skipped => Competition.Skipped + Season.Skipped + Teams.Skipped + Matches.Skipped;
    public int Processed => Competition.Processed + Season.Processed + Teams.Processed + Matches.Processed;

    public IReadOnlyCollection<string> Errors =>
        Competition.Errors
            .Concat(Season.Errors)
            .Concat(Teams.Errors)
            .Concat(Matches.Errors)
            .ToArray();
}

public sealed class FootballDataImportOrchestrator(
    ICompetitionImportService competitionImportService,
    ISeasonImportService seasonImportService,
    ITeamImportService teamImportService,
    IMatchImportService matchImportService) : IFootballDataImportOrchestrator
{
    public async Task<FootballDataImportResult> ImportCompetitionAsync(
        string sourceKey,
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seasonYear);

        var competition = await competitionImportService.ImportAsync(
            sourceKey,
            cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        var season = await seasonImportService.ImportAsync(
            sourceKey,
            competitionCode,
            cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        var teams = await teamImportService.ImportAsync(
            sourceKey,
            competitionCode,
            seasonYear,
            cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        var matches = await matchImportService.ImportAsync(
            sourceKey,
            competitionCode,
            seasonYear,
            cancellationToken);

        return new FootballDataImportResult(competition, season, teams, matches);
    }
}
