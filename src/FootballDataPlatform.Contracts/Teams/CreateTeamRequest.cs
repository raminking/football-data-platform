namespace FootballDataPlatform.Contracts.Teams;

public sealed record CreateTeamRequest(
    string Name,
    string Country,
    string? LogoUrl = null,
    string? OfficialWebsiteUrl = null);