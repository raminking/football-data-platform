using System.Net.Http.Headers;
using System.Text.Json;
using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg.Models;
using Microsoft.Extensions.Options;

namespace FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg;

public sealed class FootballDataOrgProvider : IFootballDataProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public FootballDataOrgProvider(
        HttpClient httpClient,
        IOptions<FootballDataOrgOptions> options)
    {
        _httpClient = httpClient;

        var token = options.Value.ApiToken;

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
        }
    }

    public string ProviderName => "football-data.org";

    public async Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<CompetitionResponse>(
            "v4/competitions",
            cancellationToken);

        return response.Competitions
            .Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new ExternalCompetition(
                ExternalId: x.Id.ToString(),
                Name: x.Name,
                Code: x.Code,
                Country: x.Area?.Name))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);

        if (seasonYear <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seasonYear),
                "Season year must be greater than zero.");
        }

        var path =
            $"v4/competitions/{Uri.EscapeDataString(competitionCode)}/teams?season={seasonYear}";

        var response = await GetAsync<TeamResponse>(
            path,
            cancellationToken);

        return response.Teams
            .Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new ExternalTeam(
                ExternalId: x.Id.ToString(),
                Name: x.Name,
                Country: x.Area?.Name))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);

        if (seasonYear <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seasonYear),
                "Season year must be greater than zero.");
        }

        var path =
            $"v4/competitions/{Uri.EscapeDataString(competitionCode)}/matches?season={seasonYear}";

        var response = await GetAsync<MatchResponse>(
            path,
            cancellationToken);

        return response.Matches
            .Where(IsValidMatch)
            .Select(MapMatch)
            .ToArray();
    }

    private async Task<T> GetAsync<T>(
        string relativePath,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(
            relativePath,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"football-data.org request failed with status " +
                $"{(int)response.StatusCode} ({response.ReasonPhrase}). " +
                $"Response: {body}");
        }

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        var result = await JsonSerializer.DeserializeAsync<T>(
            stream,
            JsonOptions,
            cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                $"football-data.org returned an empty response for '{relativePath}'.");
    }

    private static bool IsValidMatch(MatchDto match)
    {
        return match.Id > 0
               && match.Competition?.Id > 0
               && match.Season?.Id > 0
               && match.HomeTeam?.Id > 0
               && match.AwayTeam?.Id > 0
               && !string.IsNullOrWhiteSpace(match.Status);
    }

    private static ExternalMatch MapMatch(MatchDto match)
    {
        return new ExternalMatch(
            ExternalId: match.Id.ToString(),
            CompetitionExternalId: match.Competition!.Id.ToString(),
            SeasonExternalId: match.Season!.Id.ToString(),
            HomeTeamExternalId: match.HomeTeam!.Id.ToString(),
            AwayTeamExternalId: match.AwayTeam!.Id.ToString(),
            ScheduledAt: match.UtcDate,
            Status: match.Status,
            HomeScore: match.Score?.FullTime?.Home,
            AwayScore: match.Score?.FullTime?.Away,
            HalfTimeHomeScore: match.Score?.HalfTime?.Home,
            HalfTimeAwayScore: match.Score?.HalfTime?.Away,
            Stage: match.Stage);
    }
}