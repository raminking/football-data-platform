using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Teams;

namespace FootballDataPlatform.Application.ExternalData;

public interface ITeamImportService
{
    Task<TeamImportResult> ImportAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default);
}

public sealed record TeamImportResult(
    int Created,
    int Updated,
    int Skipped,
    IReadOnlyCollection<string> Errors)
{
    public int Processed => Created + Updated + Skipped;
}

public sealed class TeamImportService(
    IFootballDataProvider provider,
    ITeamRepository teamRepository,
    IExternalIdentityRepository externalIdentityRepository) : ITeamImportService
{
    private const string EntityType = "Team";

    public async Task<TeamImportResult> ImportAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(competitionCode))
            throw new ArgumentException("Competition code is required.", nameof(competitionCode));

        if (seasonYear <= 0)
            throw new ArgumentOutOfRangeException(nameof(seasonYear));

        var teams = await provider.GetTeamsAsync(
            competitionCode.Trim(),
            seasonYear,
            cancellationToken);

        var created = 0;
        var updated = 0;
        var skipped = 0;
        var errors = new List<string>();

        foreach (var externalTeam in teams)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(externalTeam.ExternalId) ||
                string.IsNullOrWhiteSpace(externalTeam.Name) ||
                string.IsNullOrWhiteSpace(externalTeam.Country))
            {
                skipped++;
                errors.Add("Team import skipped because external ID, name, and country are required.");
                continue;
            }

            var identity = await externalIdentityRepository.FindAsync(
                provider.ProviderName,
                EntityType,
                externalTeam.ExternalId.Trim(),
                cancellationToken);

            if (identity is null)
            {
                var team = new Team(
                    externalTeam.Name,
                    externalTeam.Country);

                await teamRepository.CreateAsync(team, cancellationToken);

                await externalIdentityRepository.AddAsync(
                    new ExternalIdentityRecord(
                        provider.ProviderName,
                        EntityType,
                        externalTeam.ExternalId,
                        team.Id,
                        DateTimeOffset.UtcNow),
                    cancellationToken);

                created++;
                continue;
            }

            var existingTeam = await teamRepository.GetByIdAsync(
                identity.InternalEntityId,
                cancellationToken);

            if (existingTeam is null)
            {
                skipped++;
                errors.Add(
                    $"External identity '{identity.ExternalId}' points to missing Team '{identity.InternalEntityId}'.");
                continue;
            }

            existingTeam.UpdateDetails(
                externalTeam.Name,
                externalTeam.Country,
                existingTeam.LogoUrl,
                existingTeam.OfficialWebsiteUrl);

            await teamRepository.UpdateAsync(existingTeam, cancellationToken);
            updated++;
        }

        return new TeamImportResult(created, updated, skipped, errors);
    }
}
