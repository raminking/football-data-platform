using System.Net;

namespace FootballDataPlatform.Tests.Teams.Integration;

public class TeamsApiTests
{
    [Fact]
    public async Task GetTeam_WithUnknownId_ReturnsNotFound()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();

        using var client = factory.CreateClient();

        var id = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/teams/{id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}