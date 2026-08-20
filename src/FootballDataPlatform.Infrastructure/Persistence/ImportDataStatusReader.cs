using FootballDataPlatform.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence;

public sealed class ImportDataStatusReader(FootballDataDbContext dbContext) : IImportDataStatusReader
{
    public async Task<ImportDataStatus> GetAsync(CancellationToken cancellationToken = default)
    {
        var competitions = await dbContext.Competitions.LongCountAsync(cancellationToken);
        var seasons = await dbContext.Seasons.LongCountAsync(cancellationToken);
        var teams = await dbContext.Teams.LongCountAsync(cancellationToken);
        var matches = await dbContext.Matches.LongCountAsync(cancellationToken);
        var externalIdentities = await dbContext.ExternalIdentities.LongCountAsync(cancellationToken);

        return new ImportDataStatus(competitions, seasons, teams, matches, externalIdentities);
    }
}
