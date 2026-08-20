namespace FootballDataPlatform.Domain.Competitions;

public class Competition
{
    public long Id { get; private set; }
    public Guid PublicId { get; private set; }
    public string Name { get; private set; }
    public string Country { get; private set; }
    public string Code { get; private set; }

    public Competition(string name, string country, string code)
    {
        Validate(name, country, code);

        PublicId = Guid.NewGuid();
        Name = name.Trim();
        Country = country.Trim();
        Code = code.Trim().ToUpperInvariant();
    }

    private Competition() { }

    public void UpdateDetails(string name, string country, string code)
    {
        Validate(name, country, code);

        Name = name.Trim();
        Country = country.Trim();
        Code = code.Trim().ToUpperInvariant();
    }

    private static void Validate(string name, string country, string code)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Competition name is required.", nameof(name));
        if (name.Length > 150)
            throw new ArgumentException("Competition name is too long (max 150 chars).", nameof(name));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Competition country is required.", nameof(country));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Competition code is required.", nameof(code));
        if (code.Length > 20)
            throw new ArgumentException("Competition code is too long (max 20 chars).", nameof(code));
    }
}