namespace FootballDataPlatform.Domain.Teams;


public class Team
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Country { get; private set; }

    public Team( string name, string country)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Team country is required.", nameof(country));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Country = country.Trim();
    }
}