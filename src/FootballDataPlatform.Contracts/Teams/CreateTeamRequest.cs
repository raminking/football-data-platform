namespace FootballDataPlatform.Contracts.Teams;

public sealed record CreateTeamRequest(
    string Name,
    string Country);