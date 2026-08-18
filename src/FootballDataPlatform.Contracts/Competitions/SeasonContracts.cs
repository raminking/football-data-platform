namespace FootballDataPlatform.Contracts.Competitions;

public record CreateSeasonRequest(Guid CompetitionId, string Name, DateOnly StartDate, DateOnly EndDate);
public record SeasonResponse(Guid Id, Guid CompetitionId, string Name, DateOnly StartDate, DateOnly EndDate);