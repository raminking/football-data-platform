namespace FootballDataPlatform.Contracts.Teams;

public sealed record GetTeamResponse(
    Guid Id,
    string Name,
    string Country,
    string? LogoUrl,
    string? OfficialWebsiteUrl);