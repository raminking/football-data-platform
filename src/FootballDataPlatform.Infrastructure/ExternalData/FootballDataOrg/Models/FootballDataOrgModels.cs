using System.Text.Json.Serialization;

namespace FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg.Models;

internal sealed class CompetitionResponse
{
    [JsonPropertyName("competitions")]
    public List<CompetitionDto> Competitions { get; init; } = [];
}

internal sealed class CompetitionDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("area")]
    public AreaDto? Area { get; init; }
}

internal sealed class SeasonResponse
{
    [JsonPropertyName("seasons")]
    public List<SeasonDto> Seasons { get; init; } = [];
}

internal sealed class SeasonDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("startDate")]
    public DateOnly StartDate { get; init; }

    [JsonPropertyName("endDate")]
    public DateOnly EndDate { get; init; }

    [JsonPropertyName("currentMatchday")]
    public int? CurrentMatchday { get; init; }
}

internal sealed class TeamResponse
{
    [JsonPropertyName("teams")]
    public List<TeamDto> Teams { get; init; } = [];
}

internal sealed class TeamDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("area")]
    public AreaDto? Area { get; init; }
}

internal sealed class MatchResponse
{
    [JsonPropertyName("matches")]
    public List<MatchDto> Matches { get; init; } = [];
}

internal sealed class MatchDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("utcDate")]
    public DateTimeOffset UtcDate { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("stage")]
    public string? Stage { get; init; }

    [JsonPropertyName("competition")]
    public CompetitionReferenceDto? Competition { get; init; }

    [JsonPropertyName("season")]
    public SeasonReferenceDto? Season { get; init; }

    [JsonPropertyName("homeTeam")]
    public TeamReferenceDto? HomeTeam { get; init; }

    [JsonPropertyName("awayTeam")]
    public TeamReferenceDto? AwayTeam { get; init; }

    [JsonPropertyName("score")]
    public ScoreDto? Score { get; init; }
}

internal sealed class AreaDto
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

internal sealed class CompetitionReferenceDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
}

internal sealed class TeamReferenceDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
}

internal sealed class SeasonReferenceDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
}

internal sealed class ScoreDto
{
    [JsonPropertyName("halfTime")]
    public ScorePartDto? HalfTime { get; init; }

    [JsonPropertyName("fullTime")]
    public ScorePartDto? FullTime { get; init; }
}

internal sealed class ScorePartDto
{
    [JsonPropertyName("home")]
    public int? Home { get; init; }

    [JsonPropertyName("away")]
    public int? Away { get; init; }
}
