using FootballDataPlatform.Domain.Competitions;

namespace FootballDataPlatform.Tests.Competitions.Domain;

public class CompetitionTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesCompetition()
    {
        var competition = new Competition("Premier League", "England", "epl");

        Assert.Equal(0, competition.Id);
        Assert.NotEqual(Guid.Empty, competition.PublicId);
        Assert.Equal("Premier League", competition.Name);
        Assert.Equal("England", competition.Country);
        Assert.Equal("EPL", competition.Code);
    }

    [Fact]
    public void Constructor_WithEmptyName_Throws() => Assert.Throws<ArgumentException>(() => new Competition("", "England", "EPL"));
    [Fact]
    public void Constructor_WithEmptyCountry_Throws() => Assert.Throws<ArgumentException>(() => new Competition("Premier League", "", "EPL"));
    [Fact]
    public void Constructor_WithEmptyCode_Throws() => Assert.Throws<ArgumentException>(() => new Competition("Premier League", "England", ""));

    [Fact]
    public void UpdateDetails_NormalizesCode()
    {
        var competition = new Competition("Premier League", "England", "EPL");
        competition.UpdateDetails("Premier League", "England", " pl ");
        Assert.Equal("PL", competition.Code);
    }
}