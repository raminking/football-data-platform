namespace FootballDataPlatform.Domain.Teams;


public class Team
{
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Country { get; private set; }

    
    public Team(string name, string country)
    {
        ValidateName(name);
        ValidateCountry(country);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Country = country.Trim();
    }
    
    public void UpdateDetails(string newName, string newCountry)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Team name cannot be empty.", nameof(newName));
        
        if (string.IsNullOrWhiteSpace(newCountry))
            throw new ArgumentException("Team country cannot be empty.", nameof(newCountry));

        // بهینه‌سازی: اگر مقداری تغییر نکرده، کاری نکنیم (Optional but good practice)
        var trimmedName = newName.Trim();
        var trimmedCountry = newCountry.Trim();

        if (Name == trimmedName && Country == trimmedCountry)
            return;

        // اعمال تغییرات
        Name = trimmedName;
        Country = trimmedCountry;
        
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
}