using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Competitions;

namespace FootballDataPlatform.Application.ExternalData;

public interface ISeasonImportService
{
    Task<SeasonImportResult> ImportAsync(string sourceKey, string competitionCode, CancellationToken cancellationToken = default);
}

public sealed record SeasonImportResult(int Created, int Updated, int Skipped, IReadOnlyCollection<string> Errors)
{
    public int Processed => Created + Updated + Skipped;
}

public sealed class SeasonImportService(
    IFootballDataSourceResolver sourceResolver,
    ICompetitionRepository competitionRepository,
    ISeasonRepository seasonRepository,
    IExternalIdentityRepository externalIdentityRepository) : ISeasonImportService
{
    private const string CompetitionEntityType = "Competition";
    private const string SeasonEntityType = "Season";

    public async Task<SeasonImportResult> ImportAsync(string sourceKey, string competitionCode, CancellationToken cancellationToken = default)
    {
        var source = sourceResolver.Resolve(sourceKey);
        if (string.IsNullOrWhiteSpace(competitionCode)) throw new ArgumentException("Competition code is required.", nameof(competitionCode));
        var code = competitionCode.Trim();
        var seasons = await source.GetSeasonsAsync(code, cancellationToken);
        var created = 0; var updated = 0; var skipped = 0; var errors = new List<string>();

        var competitionIdentity = await externalIdentityRepository.FindAsync(source.SourceKey, CompetitionEntityType, code, cancellationToken);
        if (competitionIdentity is null)
        {
            var competitions = await source.GetCompetitionsAsync(cancellationToken);
            var externalCompetition = competitions.FirstOrDefault(x => string.Equals(x.Code?.Trim(), code, StringComparison.OrdinalIgnoreCase));
            if (externalCompetition is not null)
                competitionIdentity = await externalIdentityRepository.FindAsync(source.SourceKey, CompetitionEntityType, externalCompetition.ExternalId.Trim(), cancellationToken);
        }

        if (competitionIdentity is null)
            return new SeasonImportResult(0, 0, seasons.Count, [$"Competition '{code}' has no persisted external identity."]);

        var competition = await competitionRepository.GetByIdAsync(competitionIdentity.InternalEntityId, cancellationToken);
        if (competition is null)
            return new SeasonImportResult(0, 0, seasons.Count, [$"Competition external identity '{competitionIdentity.ExternalId}' points to missing Competition '{competitionIdentity.InternalEntityId}'."]);

        if (!string.Equals(competition.Code, code, StringComparison.OrdinalIgnoreCase))
            return new SeasonImportResult(0, 0, seasons.Count, [$"Competition identity '{competitionIdentity.ExternalId}' does not match requested code '{code}'."]);

        foreach (var externalSeason in seasons)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(externalSeason.ExternalId) || string.IsNullOrWhiteSpace(externalSeason.Name) || externalSeason.StartDate == default || externalSeason.EndDate == default || externalSeason.EndDate < externalSeason.StartDate)
            { skipped++; errors.Add("Season import skipped because external ID, name, and a valid date range are required."); continue; }

            var identity = await externalIdentityRepository.FindAsync(source.SourceKey, SeasonEntityType, externalSeason.ExternalId.Trim(), cancellationToken);
            if (identity is null)
            {
                if (await seasonRepository.ExistsByIdentityAsync(competition.Id, externalSeason.Name.Trim(), null, cancellationToken))
                { skipped++; errors.Add($"Season '{externalSeason.Name}' already exists for competition '{competition.Code}'."); continue; }
                var season = new Season(competition.Id, externalSeason.Name, externalSeason.StartDate, externalSeason.EndDate);
                await seasonRepository.CreateAsync(season, cancellationToken);
                await externalIdentityRepository.AddAsync(new ExternalIdentityRecord(source.SourceKey, SeasonEntityType, externalSeason.ExternalId.Trim(), season.Id, DateTimeOffset.UtcNow), cancellationToken);
                created++; continue;
            }

            var existingSeason = await seasonRepository.GetByIdAsync(identity.InternalEntityId, cancellationToken);
            if (existingSeason is null)
            { skipped++; errors.Add($"External identity '{identity.ExternalId}' points to missing Season '{identity.InternalEntityId}'."); continue; }
            if (existingSeason.CompetitionId != competition.Id)
            { skipped++; errors.Add($"Season external identity '{identity.ExternalId}' belongs to another competition."); continue; }
            if (await seasonRepository.ExistsByIdentityAsync(competition.Id, externalSeason.Name.Trim(), existingSeason.Id, cancellationToken))
            { skipped++; errors.Add($"Season '{externalSeason.Name}' conflicts with another season for competition '{competition.Code}'."); continue; }

            existingSeason.UpdateDetails(externalSeason.Name, externalSeason.StartDate, externalSeason.EndDate);
            await seasonRepository.UpdateAsync(existingSeason, cancellationToken);
            updated++;
        }

        return new SeasonImportResult(created, updated, skipped, errors);
    }
}
