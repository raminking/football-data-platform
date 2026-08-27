using System.Net;
using System.Text.Json;
using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg.Models;
using Microsoft.Extensions.Options;

namespace FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg;

public sealed class FootballDataOrgProvider : IFootballDataSource
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;

    public FootballDataOrgProvider(HttpClient httpClient, IOptions<FootballDataOrgOptions> options)
    {
        _httpClient = httpClient;
        var token = options.Value.ApiToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
        }
    }

    public string SourceKey => "football-data.org";

    public async Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<CompetitionResponse>("v4/competitions", nameof(GetCompetitionsAsync), cancellationToken);
        return response.Competitions
            .Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Code))
            .Select(x => new ExternalCompetition(x.Id.ToString(), x.Name, x.Code!, x.Area?.Name))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalSeason>> GetSeasonsAsync(string competitionCode, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);
        var path = $"v4/competitions/{Uri.EscapeDataString(competitionCode)}";
        var response = await GetAsync<CompetitionDetailDto>(path, nameof(GetSeasonsAsync), cancellationToken);
        return response.Seasons
            .Where(x => x.Id > 0 && x.StartDate != default && x.EndDate != default && x.EndDate >= x.StartDate)
            .Select(x => new ExternalSeason(x.Id.ToString(), response.Id.ToString(), BuildSeasonName(x.StartDate, x.EndDate), x.StartDate, x.EndDate))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);
        if (seasonYear <= 0) throw new ArgumentOutOfRangeException(nameof(seasonYear), "Season year must be greater than zero.");
        var path = $"v4/competitions/{Uri.EscapeDataString(competitionCode)}/teams?season={seasonYear}";
        var response = await GetAsync<TeamResponse>(path, nameof(GetTeamsAsync), cancellationToken);
        return response.Teams.Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new ExternalTeam(x.Id.ToString(), x.Name, x.Area?.Name)).ToArray();
    }

    public async Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competitionCode);
        if (seasonYear <= 0) throw new ArgumentOutOfRangeException(nameof(seasonYear), "Season year must be greater than zero.");
        var path = $"v4/competitions/{Uri.EscapeDataString(competitionCode)}/matches?season={seasonYear}";
        var response = await GetAsync<MatchResponse>(path, nameof(GetMatchesAsync), cancellationToken);
        return response.Matches.Where(IsValidMatch).Select(MapMatch).ToArray();
    }

    private async Task<T> GetAsync<T>(string relativePath, string operation, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetAsync(relativePath, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                var category = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => ExternalDataErrorCategory.Authentication,
                    HttpStatusCode.TooManyRequests => ExternalDataErrorCategory.RateLimited,
                    _ when statusCode >= 500 => ExternalDataErrorCategory.ServerError,
                    _ => ExternalDataErrorCategory.InvalidResponse
                };

                throw new ExternalDataException(
                    category,
                    SourceKey,
                    operation,
                    $"football-data.org request failed with status {statusCode} ({response.ReasonPhrase}).",
                    statusCode);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken)
                ?? throw new ExternalDataException(
                    ExternalDataErrorCategory.InvalidResponse,
                    SourceKey,
                    operation,
                    "football-data.org returned an empty response.");
        }
        catch (ExternalDataException)
        {
            throw;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ExternalDataException(
                ExternalDataErrorCategory.Timeout,
                SourceKey,
                operation,
                "The football-data.org request timed out.");
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalDataException(
                ExternalDataErrorCategory.Network,
                SourceKey,
                operation,
                "The request to football-data.org failed.",
                innerException: ex);
        }
        catch (JsonException ex)
        {
            throw new ExternalDataException(
                ExternalDataErrorCategory.InvalidResponse,
                SourceKey,
                operation,
                "football-data.org returned an invalid response.",
                innerException: ex);
        }
    }

    private static string BuildSeasonName(DateOnly startDate, DateOnly endDate) =>
        startDate.Year == endDate.Year ? startDate.Year.ToString() : $"{startDate.Year}/{endDate.Year % 100:00}";

    private static bool IsValidMatch(MatchDto match) =>
        match.Id > 0 && match.Competition?.Id > 0 && match.Season?.Id > 0 && match.HomeTeam?.Id > 0 && match.AwayTeam?.Id > 0 && !string.IsNullOrWhiteSpace(match.Status);

    private static ExternalMatch MapMatch(MatchDto match) => new(
        match.Id.ToString(), match.Competition!.Id.ToString(), match.Season!.Id.ToString(), match.HomeTeam!.Id.ToString(), match.AwayTeam!.Id.ToString(), match.UtcDate, match.Status,
        match.Score?.FullTime?.Home, match.Score?.FullTime?.Away, match.Score?.HalfTime?.Home, match.Score?.HalfTime?.Away, match.Stage);
}
