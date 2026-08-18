namespace FootballDataPlatform.Domain.Competitions;

public class Season
{
    public Guid Id { get; private set; }
    public Guid CompetitionId { get; private set; }
    public string Name { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public Season(Guid competitionId, string name, DateOnly startDate, DateOnly endDate)
    {
        Validate(competitionId, name, startDate, endDate);

        Id = Guid.NewGuid();
        CompetitionId = competitionId;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    public void UpdateDetails(string name, DateOnly startDate, DateOnly endDate)
    {
        Validate(CompetitionId, name, startDate, endDate);

        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    private static void Validate(Guid competitionId, string name, DateOnly startDate, DateOnly endDate)
    {
        if (competitionId == Guid.Empty)
            throw new ArgumentException("Competition is required.", nameof(competitionId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Season name is required.", nameof(name));
        if (name.Length > 50)
            throw new ArgumentException("Season name is too long (max 50 chars).", nameof(name));
        if (endDate < startDate)
            throw new ArgumentException("Season end date cannot be before start date.", nameof(endDate));
    }
}