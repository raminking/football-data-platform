namespace FootballDataPlatform.Domain.Competitions;

public class Season
{
    public long Id { get; private set; }
    public Guid PublicId { get; private set; }
    public long CompetitionId { get; private set; }
    public string Name { get; private set; } = null!;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public Season(long competitionId, string name, DateOnly startDate, DateOnly endDate)
    {
        Validate(competitionId, name, startDate, endDate);

        PublicId = Guid.NewGuid();
        CompetitionId = competitionId;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    private Season() { }

    public void UpdateDetails(string name, DateOnly startDate, DateOnly endDate)
    {
        Validate(CompetitionId, name, startDate, endDate);

        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    private static void Validate(long competitionId, string name, DateOnly startDate, DateOnly endDate)
    {
        if (competitionId <= 0)
            throw new ArgumentException("Competition is required.", nameof(competitionId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Season name is required.", nameof(name));
        if (name.Length > 50)
            throw new ArgumentException("Season name is too long (max 50 chars).", nameof(name));
        if (endDate < startDate)
            throw new ArgumentException("Season end date cannot be before start date.", nameof(endDate));
    }
}