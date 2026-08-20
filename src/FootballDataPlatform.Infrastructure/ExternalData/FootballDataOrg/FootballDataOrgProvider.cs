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
        var response = await GetAsync<CompetitionResponse>("v4/competitions", cancellationToken);

        return response.Competitions
            .Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new ExternalCompetition(
                x.Id.ToString(), x.Name, x.Code, x.Area?.Name))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalSeason>> GetSeasonsAsync(
        string competitionCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);

        var path = $"v4/competitions/{Uri.EscapeDataString(competitionCode)}";
        var response = await GetAsync<SeasonResponse>(path, cancellationToken);

        return response.Seasons
            .Where(x => x.Id > 0 && x.StartDate != default && x.EndDate != default && x.EndDate >= x.StartDate)
            .Select(x => new ExternalSeason(
                ExternalId: x.Id.ToString(),
                CompetitionExternalId: competitionCode,
                Name: BuildSeasonName(x.StartDate, x.EndDate),
                StartDate: x.StartDate,
                EndDate: x.EndDate))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);
        if (seasonYear <= 0)
            throw new ArgumentOutOfRangeException(nameof(seasonYear), "Season year must be greater than zero.");

        var path = $"v4/competitions/{Uri.EscapeDataString(competitionCode)}/teams?season={seasonYear}";
        var response = await GetAsync<TeamResponse>(path, cancellationToken);

        return response.Teams
            .Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new ExternalTeam(x.Id.ToString(), x.Name, x.Area?.Name))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(
        string competitionCode,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);
        if (seasonYear <= 0)
            throw new ArgumentOutOfRangeException(nameof(seasonYear), "Season year must be greater than zero.");

        var path = $"v4/competitions/{Uri.EscapeDataString(competitionCode)}/matches?season={seasonYear}";
        var response = await GetAsync<MatchResponse>(path, cancellationToken);

        return response.Matches
            .Where(IsValidMatch)
            .Select(MapMatch)
            .ToArray();
    }

    private async Task<T> GetAsync<T>(string relativePath, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(relativePath, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"football-data.org request failed with status {(int)response.StatusCode} ({response.ReasonPhrase}). Response: {body}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException($"football-data.org returned an empty response for '{relativePath}'.");
    }

    private static string BuildSeasonName(DateOnly startDate, DateOnly endDate) =>
        startDate.Year == endDate.Year
            ? startDate.Year.ToString()
            : $"{startDate.Year}/{endDate.Year % 100:00}";

    private static bool IsValidMatch(MatchDto match) =>
        match.Id > 0 && match.Competition?.Id > 0 && match.Season?.Id > 0 &&
        match.HomeTeam?.Id > 0 && match.AwayTeam?.Id > 0 &&
        !string.IsNullOrWhiteSpace(match.Status);

    private static ExternalMatch MapMatch(MatchDto match) => new(
        match.Id.ToString(),
        match.Competition!.Id.ToString(),
        match.Season!.Id.ToString(),
        match.HomeTeam!.Id.ToString(),
        match.AwayTeam!.Id.ToString(),
        match.UtcDate,
        match.Status,
        match.Score?.FullTime?.Home,
        match.Score?.FullTime?.Away,
        match.Score?.HalfTime?.Home,
        match.Score?.HalfTime?.Away,
        match.Stage);
}
