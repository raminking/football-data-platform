using System.Net;
using System.Net.Http.Json;
using FootballDataPlatform.Contracts.Teams;

namespace FootballDataPlatform.Tests.Teams.Integration;

public class TeamsApiTests
{
    [Fact]
    public async Task CreateTeam_WithValidRequest_ReturnsCreated()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest("Sporting CP", "Portugal");

        var response = await client.PostAsJsonAsync("/teams", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CreateTeamResponse>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal($"/teams/{result.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task CreateTeam_WithEmptyName_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest(string.Empty, "Portugal");

        var response = await client.PostAsJsonAsync("/teams", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeam_WithEmptyCountry_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest("Sporting CP", string.Empty);

        var response = await client.PostAsJsonAsync("/teams", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeam_WithDuplicateNameAndCountry_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest("Sporting CP", "Portugal");

        var firstResponse = await client.PostAsJsonAsync("/teams", request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await client.PostAsJsonAsync("/teams", request);

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task GetTeam_WithExistingId_ReturnsTeam()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new CreateTeamRequest("Benfica", "Portugal");

        var createResponse = await client.PostAsJsonAsync(
            "/teams",
            createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTeam = await createResponse.Content
            .ReadFromJsonAsync<CreateTeamResponse>();

        Assert.NotNull(createdTeam);

        // Act
        var response = await client.GetAsync($"/teams/{createdTeam.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<GetTeamResponse>();

        Assert.NotNull(result);
        Assert.Equal(createdTeam.Id, result.Id);
        Assert.Equal(createRequest.Name, result.Name);
        Assert.Equal(createRequest.Country, result.Country);
    }

    [Fact]
    public async Task GetTeam_WithUnknownId_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var id = Guid.NewGuid();

        var response = await client.GetAsync($"/teams/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
