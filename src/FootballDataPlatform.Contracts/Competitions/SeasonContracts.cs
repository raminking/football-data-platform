namespace FootballDataPlatform.Contracts.Competitions;

public record CreateSeasonRequest(Guid CompetitionPublicId, string Name, DateOnly StartDate, DateOnly EndDate);
public record SeasonResponse(Guid PublicId, Guid CompetitionPublicId, string Name, DateOnly StartDate, DateOnly EndDate);