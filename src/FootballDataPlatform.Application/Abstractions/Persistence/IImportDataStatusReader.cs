namespace FootballDataPlatform.Application.Abstractions.Persistence;

public sealed record ImportDataStatus(
    long Competitions,
    long Seasons,
    long Teams,
    long Matches,
    long ExternalIdentities);

public interface IImportDataStatusReader
{
    Task<ImportDataStatus> GetAsync(CancellationToken cancellationToken = default);
}
