namespace FootballDataPlatform.Contracts.Competitions;

public record CompetitionResponse(Guid PublicId, string Name, string Country, string Code);