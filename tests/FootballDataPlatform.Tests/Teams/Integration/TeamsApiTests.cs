using System.Net;
using System.Net.Http.Json;
using FootballDataPlatform.Contracts.Teams;

namespace FootballDataPlatform.Tests.Teams.Integration;

public class TeamsApiTests
{
    [Fact]
    public async Task CreateTeam_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest(
            "Sporting CP",
            "Portugal");

        // Act
        var response = await client.PostAsJsonAsync(
            "/teams",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<CreateTeamResponse>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);

        Assert.Equal(
            $"/teams/{result.Id}",
            response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task CreateTeam_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest(
            string.Empty,
            "Portugal");

        // Act
        var response = await client.PostAsJsonAsync(
            "/teams",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateTeam_WithEmptyCountry_ReturnsBadRequest()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest(
            "Sporting CP",
            string.Empty);

        // Act
        var response = await client.PostAsJsonAsync(
            "/teams",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateTeam_WithDuplicateNameAndCountry_ReturnsBadRequest()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest(
            "Sporting CP",
            "Portugal");

        var firstResponse = await client.PostAsJsonAsync(
            "/teams",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        // Act
        var duplicateResponse = await client.PostAsJsonAsync(
            "/teams",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            duplicateResponse.StatusCode);
    }

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
