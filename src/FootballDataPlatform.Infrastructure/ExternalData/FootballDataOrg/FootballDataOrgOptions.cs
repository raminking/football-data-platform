namespace FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg;

public sealed class FootballDataOrgOptions
{
    public const string SectionName = "FootballDataOrg";

    public string BaseUrl { get; init; } = "https://api.football-data.org/";

    public string ApiToken { get; init; } = string.Empty;
}