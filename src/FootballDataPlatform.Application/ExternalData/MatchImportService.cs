using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Match;
using MatchEntity = FootballDataPlatform.Domain.Match.Match;

namespace FootballDataPlatform.Application.ExternalData;

public interface IMatchImportService
{
    Task<MatchImportResult> ImportAsync(
        string sourceKey,
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default);
}

public sealed record MatchImportResult(
    int Created,
    int Updated,
    int Skipped,
    IReadOnlyCollection<string> Errors)
{
    public int Processed => Created + Updated + Skipped;
}

public sealed class MatchImportService(
    IFootballDataSourceResolver sourceResolver,
    IMatchRepository matchRepository,
    IExternalIdentityRepository externalIdentityRepository,
    ICompetitionRepository competitionRepository,
    ISeasonRepository seasonRepository,
    ITeamRepository teamRepository) : IMatchImportService
{
    private const string CompetitionEntityType = "Competition";
    private const string SeasonEntityType = "Season";
    private const string TeamEntityType = "Team";
    private const string MatchEntityType = "Match";

    public async Task<MatchImportResult> ImportAsync(
        string sourceKey,
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        var source = sourceResolver.Resolve(sourceKey);

        if (string.IsNullOrWhiteSpace(competitionCode))
            throw new ArgumentException("Competition code is required.", nameof(competitionCode));

        if (seasonYear <= 0)
            throw new ArgumentOutOfRangeException(nameof(seasonYear));

        var matches = await source.GetMatchesAsync(
            competitionCode.Trim(),
            seasonYear,
            cancellationToken);

        var created = 0;
        var updated = 0;
        var skipped = 0;
        var errors = new List<string>();

        var competitionIdentity = await externalIdentityRepository.FindAsync(
            source.SourceKey,
            CompetitionEntityType,
            await ResolveCompetitionExternalIdAsync(source, competitionCode, cancellationToken),
            cancellationToken);

        if (competitionIdentity is null)
        {
            return new MatchImportResult(
                0,
                0,
                matches.Count,
                [$"Competition '{competitionCode}' is not imported for source '{source.SourceKey}'."]);
        }

        var competition = await competitionRepository.GetByIdAsync(
            competitionIdentity.InternalEntityId,
            cancellationToken);

        if (competition is null)
        {
            return new MatchImportResult(
                0,
                0,
                matches.Count,
                [$"Competition identity '{competitionIdentity.ExternalId}' points to missing Competition '{competitionIdentity.InternalEntityId}'."]);
        }

        var seasonExternalId = matches
            .Select(x => x.ExternalSeasonId)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (seasonExternalId is null)
        {
            return new MatchImportResult(
                0,
                0,
                matches.Count,
                ["Match source returned no valid season external identifier."]);
        }

        var seasonIdentity = await externalIdentityRepository.FindAsync(
            source.SourceKey,
            SeasonEntityType,
            seasonExternalId.Trim(),
            cancellationToken);

        if (seasonIdentity is null)
        {
            return new MatchImportResult(
                0,
                0,
                matches.Count,
                [$"Season '{seasonExternalId}' is not imported for source '{source.SourceKey}'."]);
        }

        var season = await seasonRepository.GetByIdAsync(
            seasonIdentity.InternalEntityId,
            cancellationToken);

        if (season is null || season.CompetitionId != competition.Id)
        {
            return new MatchImportResult(
                0,
                0,
                matches.Count,
                [$"Season identity '{seasonIdentity.ExternalId}' does not resolve to the requested Competition."]);
        }

        foreach (var externalMatch in matches)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!TryMapStatus(externalMatch.Status, out var status))
            {
                skipped++;
                errors.Add($"Match '{externalMatch.ExternalId}' skipped because status '{externalMatch.Status}' is unsupported.");
                continue;
            }

            var homeIdentity = await externalIdentityRepository.FindAsync(
                source.SourceKey,
                TeamEntityType,
                externalMatch.ExternalHomeTeamId,
                cancellationToken);

            var awayIdentity = await externalIdentityRepository.FindAsync(
                source.SourceKey,
                TeamEntityType,
                externalMatch.ExternalAwayTeamId,
                cancellationToken);

            if (homeIdentity is null || awayIdentity is null)
            {
                skipped++;
                errors.Add($"Match '{externalMatch.ExternalId}' skipped because one or both teams are not imported.");
                continue;
            }

            var homeTeam = await teamRepository.GetByIdAsync(homeIdentity.InternalEntityId, cancellationToken);
            var awayTeam = await teamRepository.GetByIdAsync(awayIdentity.InternalEntityId, cancellationToken);

            if (homeTeam is null || awayTeam is null)
            {
                skipped++;
                errors.Add($"Match '{externalMatch.ExternalId}' skipped because one or both team identities point to missing teams.");
                continue;
            }

            if (!TryMapStage(externalMatch.Stage, out var stage))
            {
                skipped++;
                errors.Add($"Match '{externalMatch.ExternalId}' skipped because stage '{externalMatch.Stage}' is unsupported.");
                continue;
            }

            try
            {
                var identity = await externalIdentityRepository.FindAsync(
                    source.SourceKey,
                    MatchEntityType,
                    externalMatch.ExternalId.Trim(),
                    cancellationToken);

                if (identity is null)
                {
                    var match = new MatchEntity(
                        season.Id,
                        homeTeam.Id,
                        awayTeam.Id,
                        externalMatch.UtcDate,
                        stage,
                        status,
                        externalMatch.FullTimeHome,
                        externalMatch.FullTimeAway,
                        externalMatch.HalfTimeHome,
                        externalMatch.HalfTimeAway);

                    await matchRepository.CreateAsync(match, cancellationToken);
                    await externalIdentityRepository.AddAsync(
                        new ExternalIdentityRecord(
                            source.SourceKey,
                            MatchEntityType,
                            externalMatch.ExternalId.Trim(),
                            match.Id,
                            DateTimeOffset.UtcNow),
                        cancellationToken);

                    created++;
                    continue;
                }

                var existingMatch = await matchRepository.GetByIdAsync(
                    identity.InternalEntityId,
                    cancellationToken);

                if (existingMatch is null)
                {
                    skipped++;
                    errors.Add($"External identity '{identity.ExternalId}' points to missing Match '{identity.InternalEntityId}'.");
                    continue;
                }

                if (existingMatch.SeasonId != season.Id ||
                    existingMatch.HomeTeamId != homeTeam.Id ||
                    existingMatch.AwayTeamId != awayTeam.Id)
                {
                    skipped++;
                    errors.Add($"Match '{externalMatch.ExternalId}' was not updated because its season or team identity changed.");
                    continue;
                }

                existingMatch.UpdateDetails(
                    externalMatch.UtcDate,
                    stage,
                    status,
                    externalMatch.FullTimeHome,
                    externalMatch.FullTimeAway,
                    externalMatch.HalfTimeHome,
                    externalMatch.HalfTimeAway);

                await matchRepository.UpdateAsync(existingMatch, cancellationToken);
                updated++;
            }
            catch (ArgumentException ex)
            {
                skipped++;
                errors.Add($"Match '{externalMatch.ExternalId}' skipped: {ex.Message}");
            }
        }

        return new MatchImportResult(created, updated, skipped, errors);
    }

    private async Task<string> ResolveCompetitionExternalIdAsync(
        IFootballDataSource source,
        string competitionCode,
        CancellationToken cancellationToken)
    {
        var competitions = await source.GetCompetitionsAsync(cancellationToken);
        var competition = competitions.FirstOrDefault(x =>
            string.Equals(x.Code, competitionCode.Trim(), StringComparison.OrdinalIgnoreCase));

        return competition?.ExternalId
            ?? throw new InvalidOperationException(
                $"Competition '{competitionCode}' was not found in source '{source.SourceKey}'.");
    }

    private static bool TryMapStatus(string? value, out MatchStatus status)
    {
        status = value?.Trim().ToUpperInvariant() switch
        {
            "SCHEDULED" => MatchStatus.Scheduled,
            "TIMED" => MatchStatus.Scheduled,
            "LIVE" => MatchStatus.InProgress,
            "IN_PLAY" => MatchStatus.InProgress,
            "PAUSED" => MatchStatus.InProgress,
            "FINISHED" => MatchStatus.Finished,
            "POSTPONED" => MatchStatus.Postponed,
            "SUSPENDED" => MatchStatus.Postponed,
            "CANCELLED" => MatchStatus.Cancelled,
            "CANCELED" => MatchStatus.Cancelled,
            "ABANDONED" => MatchStatus.Abandoned,
            _ => default
        };

        return Enum.IsDefined(status);
    }

    private static bool TryMapStage(string? value, out MatchStage stage)
    {
        stage = value?.Trim().ToUpperInvariant() switch
        {
            null or "" or "REGULAR_SEASON" => MatchStage.League,
            "LEAGUE_PHASE" or "LEAGUE_STAGE" => MatchStage.LeaguePhase,
            "GROUP_STAGE" => MatchStage.GroupStage,
            "PLAYOFF" or "PLAY_OFF" or "LAST_32" or "THIRD_PLACE" => MatchStage.Playoff,
            "ROUND_OF_16" => MatchStage.RoundOf16,
            "QUARTER_FINAL" or "QUARTER_FINALS" => MatchStage.QuarterFinal,
            "SEMI_FINAL" or "SEMI_FINALS" => MatchStage.SemiFinal,
            "FINAL" => MatchStage.Final,
            "FRIENDLY" => MatchStage.Friendly,
            _ => MatchStage.League
        };

        return true;
    }
}
