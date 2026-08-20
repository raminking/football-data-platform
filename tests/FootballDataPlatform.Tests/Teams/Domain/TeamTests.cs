using FootballDataPlatform.Domain.Teams;

namespace FootballDataPlatform.Tests.Teams.Domain;

public class TeamTests
{
    [Fact]
    public void CreateTeam_WithValidData_ShouldCreateTeam()
    {
        var team = new Team("Liverpool", "England");

        Assert.Equal(0, team.Id);
        Assert.NotEqual(Guid.Empty, team.PublicId);
        Assert.Equal("Liverpool", team.Name);
        Assert.Equal("England", team.Country);
        Assert.Null(team.LogoUrl);
        Assert.Null(team.OfficialWebsiteUrl);
    }

    [Fact]
    public void CreateTeam_WithMetadata_ShouldStoreMetadata()
    {
        var team = new Team("Liverpool", "England", "https://example.com/liverpool.png", "https://www.liverpoolfc.com");

        Assert.Equal("https://example.com/liverpool.png", team.LogoUrl);
        Assert.Equal("https://www.liverpoolfc.com", team.OfficialWebsiteUrl);
    }

    [Fact]
    public void CreateTeam_WithEmptyName_ShouldThrowException() =>
        Assert.Throws<ArgumentException>(() => new Team("", "England"));

    [Fact]
    public void CreateTeam_WithEmptyCountry_ShouldThrowException() =>
        Assert.Throws<ArgumentException>(() => new Team("Liverpool", ""));

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/logo.png")]
    public void CreateTeam_WithInvalidLogoUrl_ShouldThrowException(string logoUrl) =>
        Assert.Throws<ArgumentException>(() => new Team("Liverpool", "England", logoUrl, null));

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    public void CreateTeam_WithInvalidOfficialWebsiteUrl_ShouldThrowException(string websiteUrl) =>
        Assert.Throws<ArgumentException>(() => new Team("Liverpool", "England", null, websiteUrl));

    [Fact]
    public void UpdateDetails_WithMetadata_ShouldUpdateAllFields()
    {
        var team = new Team("Liverpool", "England");
        team.UpdateDetails("Liverpool FC", "England", "https://example.com/logo.png", "https://www.liverpoolfc.com");

        Assert.Equal("Liverpool FC", team.Name);
        Assert.Equal("England", team.Country);
        Assert.Equal("https://example.com/logo.png", team.LogoUrl);
        Assert.Equal("https://www.liverpoolfc.com", team.OfficialWebsiteUrl);
    }
}