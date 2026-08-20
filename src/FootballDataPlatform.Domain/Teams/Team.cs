namespace FootballDataPlatform.Domain.Teams;

public class Team
{
    public long Id { get; private set; }
    public Guid PublicId { get; private set; }
    public string Name { get; private set; }
    public string Country { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? OfficialWebsiteUrl { get; private set; }

    public Team(
        string name,
        string country,
        string? logoUrl = null,
        string? officialWebsiteUrl = null)
    {
        ValidateName(name);
        ValidateCountry(country);
        ValidateOptionalUrl(logoUrl, nameof(logoUrl));
        ValidateOptionalUrl(officialWebsiteUrl, nameof(officialWebsiteUrl));

        PublicId = Guid.NewGuid();
        Name = name.Trim();
        Country = country.Trim();
        LogoUrl = NormalizeOptional(logoUrl);
        OfficialWebsiteUrl = NormalizeOptional(officialWebsiteUrl);
    }

    private Team() { }

    public void UpdateDetails(
        string newName,
        string newCountry,
        string? newLogoUrl = null,
        string? newOfficialWebsiteUrl = null)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Team name cannot be empty.", nameof(newName));

        if (string.IsNullOrWhiteSpace(newCountry))
            throw new ArgumentException("Team country cannot be empty.", nameof(newCountry));

        ValidateOptionalUrl(newLogoUrl, nameof(newLogoUrl));
        ValidateOptionalUrl(newOfficialWebsiteUrl, nameof(newOfficialWebsiteUrl));

        var trimmedName = newName.Trim();
        var trimmedCountry = newCountry.Trim();
        var normalizedLogoUrl = NormalizeOptional(newLogoUrl);
        var normalizedOfficialWebsiteUrl = NormalizeOptional(newOfficialWebsiteUrl);

        if (Name == trimmedName &&
            Country == trimmedCountry &&
            LogoUrl == normalizedLogoUrl &&
            OfficialWebsiteUrl == normalizedOfficialWebsiteUrl)
            return;

        Name = trimmedName;
        Country = trimmedCountry;
        LogoUrl = normalizedLogoUrl;
        OfficialWebsiteUrl = normalizedOfficialWebsiteUrl;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name is required.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Team name is too long (max 100 chars).", nameof(name));
    }

    private static void ValidateCountry(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Team country is required.", nameof(country));
    }

    private static void ValidateOptionalUrl(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("URL must be a valid absolute HTTP or HTTPS URL.", parameterName);
        }
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}