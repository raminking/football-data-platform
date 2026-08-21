using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Teams;

namespace FootballDataPlatform.Application.ExternalData;

public interface ITeamImportService
{
    Task<TeamImportResult> ImportAsync(string sourceKey, string competitionCode, int seasonYear, CancellationToken cancellationToken = default);
}

public sealed record TeamImportResult(int Created, int Updated, int Skipped, IReadOnlyCollection<string> Errors)
{
    public int Processed => Created + Updated + Skipped;
}

public sealed class TeamImportService(
    IFootballDataSourceResolver sourceResolver,
    ITeamRepository teamRepository,
    IExternalIdentityRepository externalIdentityRepository,
    IUnitOfWork unitOfWork) : ITeamImportService
{
    private const string EntityType = "Team";

    public async Task<TeamImportResult> ImportAsync(string sourceKey, string competitionCode, int seasonYear, CancellationToken cancellationToken = default)
    {
        var source = sourceResolver.Resolve(sourceKey);
        if (string.IsNullOrWhiteSpace(competitionCode)) throw new ArgumentException("Competition code is required.", nameof(competitionCode));
        if (seasonYear <= 0) throw new ArgumentOutOfRangeException(nameof(seasonYear));

        var teams = await source.GetTeamsAsync(competitionCode.Trim(), seasonYear, cancellationToken);
        var created = 0; var updated = 0; var skipped = 0; var errors = new List<string>();

        foreach (var externalTeam in teams)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(externalTeam.ExternalId) || string.IsNullOrWhiteSpace(externalTeam.Name) || string.IsNullOrWhiteSpace(externalTeam.Country))
            { skipped++; errors.Add("Team import skipped because external ID, name, and country are required."); continue; }

            var externalId = externalTeam.ExternalId.Trim();
            var identity = await externalIdentityRepository.FindAsync(source.SourceKey, EntityType, externalId, cancellationToken);
            if (identity is null)
            {
                var team = new Team(externalTeam.Name, externalTeam.Country);
                await unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await teamRepository.CreateAsync(team, cancellationToken);
                    await externalIdentityRepository.AddAsync(new ExternalIdentityRecord(source.SourceKey, EntityType, externalId, team.Id, DateTimeOffset.UtcNow), cancellationToken);
                }, cancellationToken);
                created++;
                continue;
            }

            var existingTeam = await teamRepository.GetByIdAsync(identity.InternalEntityId, cancellationToken);
            if (existingTeam is null)
            { skipped++; errors.Add($"External identity '{identity.ExternalId}' points to missing Team '{identity.InternalEntityId}'."); continue; }

            existingTeam.UpdateDetails(externalTeam.Name, externalTeam.Country, existingTeam.LogoUrl, existingTeam.OfficialWebsiteUrl);
            await unitOfWork.ExecuteInTransactionAsync(
                () => teamRepository.UpdateAsync(existingTeam, cancellationToken),
                cancellationToken);
            updated++;
        }

        return new TeamImportResult(created, updated, skipped, errors);
    }
}
