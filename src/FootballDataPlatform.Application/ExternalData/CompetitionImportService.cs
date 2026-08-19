using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Competitions;

namespace FootballDataPlatform.Application.ExternalData;

public interface ICompetitionImportService
{
    Task<CompetitionImportResult> ImportAsync(CancellationToken cancellationToken = default);
}

public sealed record CompetitionImportResult(
    int Created,
    int Updated,
    int Skipped,
    IReadOnlyCollection<string> Errors)
{
    public int Processed => Created + Updated + Skipped;
}

public sealed class CompetitionImportService(
    IFootballDataProvider provider,
    ICompetitionRepository competitionRepository,
    IExternalIdentityRepository externalIdentityRepository) : ICompetitionImportService
{
    private const string EntityType = "Competition";

    public async Task<CompetitionImportResult> ImportAsync(
        CancellationToken cancellationToken = default)
    {
        var competitions = await provider.GetCompetitionsAsync(cancellationToken);

        var created = 0;
        var updated = 0;
        var skipped = 0;
        var errors = new List<string>();

        foreach (var externalCompetition in competitions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(externalCompetition.ExternalId) ||
                string.IsNullOrWhiteSpace(externalCompetition.Name) ||
                string.IsNullOrWhiteSpace(externalCompetition.Country) ||
                string.IsNullOrWhiteSpace(externalCompetition.Code))
            {
                skipped++;
                errors.Add("Competition import skipped because external ID, name, country, and code are required.");
                continue;
            }

            var identity = await externalIdentityRepository.FindAsync(
                provider.ProviderName,
                EntityType,
                externalCompetition.ExternalId.Trim(),
                cancellationToken);

            if (identity is null)
            {
                var exists = await competitionRepository.ExistsByIdentityAsync(
                    externalCompetition.Name.Trim(),
                    externalCompetition.Country.Trim(),
                    externalCompetition.Code.Trim(),
                    null,
                    cancellationToken);

                if (exists)
                {
                    skipped++;
                    errors.Add($"Competition '{externalCompetition.Code}' already exists without an external identity.");
                    continue;
                }

                var competition = new Competition(
                    externalCompetition.Name,
                    externalCompetition.Country,
                    externalCompetition.Code);

                await competitionRepository.CreateAsync(competition, cancellationToken);

                await externalIdentityRepository.AddAsync(
                    new ExternalIdentityRecord(
                        provider.ProviderName,
                        EntityType,
                        externalCompetition.ExternalId.Trim(),
                        competition.Id,
                        DateTimeOffset.UtcNow),
                    cancellationToken);

                created++;
                continue;
            }

            var existingCompetition = await competitionRepository.GetByIdAsync(
                identity.InternalEntityId,
                cancellationToken);

            if (existingCompetition is null)
            {
                skipped++;
                errors.Add(
                    $"External identity '{identity.ExternalId}' points to missing Competition '{identity.InternalEntityId}'.");
                continue;
            }

            var duplicate = await competitionRepository.ExistsByIdentityAsync(
                externalCompetition.Name.Trim(),
                externalCompetition.Country.Trim(),
                externalCompetition.Code.Trim(),
                existingCompetition.Id,
                cancellationToken);

            if (duplicate)
            {
                skipped++;
                errors.Add($"Competition '{externalCompetition.Code}' conflicts with another existing competition.");
                continue;
            }

            existingCompetition.UpdateDetails(
                externalCompetition.Name,
                externalCompetition.Country,
                externalCompetition.Code);

            await competitionRepository.UpdateAsync(existingCompetition, cancellationToken);
            updated++;
        }

        return new CompetitionImportResult(created, updated, skipped, errors);
    }
}
