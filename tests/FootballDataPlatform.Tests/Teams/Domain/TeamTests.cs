using FootballDataPlatform.Domain.Teams;

namespace FootballDataPlatform.Tests.Teams.Domain;

public class TeamTests
{
    [Fact]
    public void CreateTeam_WithValidData_ShouldCreateTeam()
    {
        // Arrange
        var name = "Liverpool";
        var country = "England";

        // Act
        var team = new Team(name, country);

        // Assert
        Assert.NotEqual(Guid.Empty, team.Id);
        Assert.Equal(name, team.Name);
        Assert.Equal(country, team.Country);
    }
    [Fact]
    public void CreateTeam_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var name = "";
        var country = "England";

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new Team(name, country));
    }
    [Fact]
    public void CreateTeam_WithEmptyCountry_ShouldThrowException()
    {
        // Arrange
        var name = "Liverpool";
        var country = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new Team(name, country));
    }
}