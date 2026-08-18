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
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new CreateTeamRequest("Benfica", "Portugal");
        var createResponse = await client.PostAsJsonAsync("/teams", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTeam = await createResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        Assert.NotNull(createdTeam);

        var response = await client.GetAsync($"/teams/{createdTeam.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GetTeamResponse>();

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

        var response = await client.GetAsync($"/teams/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTeam_WithValidRequest_ReturnsOkAndUpdatesTeam()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/teams",
            new CreateTeamRequest("Benfica", "Portugal"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTeam = await createResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        Assert.NotNull(createdTeam);

        var updateRequest = new
        {
            Id = createdTeam.Id,
            Name = "SL Benfica",
            Country = "Portugal"
        };

        var updateResponse = await client.PostAsJsonAsync(
            "/teams/update",
            updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/teams/{createdTeam.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updatedTeam = await getResponse.Content.ReadFromJsonAsync<GetTeamResponse>();

        Assert.NotNull(updatedTeam);
        Assert.Equal(createdTeam.Id, updatedTeam.Id);
        Assert.Equal("SL Benfica", updatedTeam.Name);
        Assert.Equal("Portugal", updatedTeam.Country);
    }

    [Fact]
    public async Task UpdateTeam_WithUnknownId_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            Id = Guid.NewGuid(),
            Name = "Benfica",
            Country = "Portugal"
        };

        var response = await client.PostAsJsonAsync("/teams/update", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTeam_WithEmptyName_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/teams",
            new CreateTeamRequest("Benfica", "Portugal"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTeam = await createResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        Assert.NotNull(createdTeam);

        var response = await client.PostAsJsonAsync(
            "/teams/update",
            new
            {
                Id = createdTeam.Id,
                Name = string.Empty,
                Country = "Portugal"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTeam_WithEmptyCountry_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/teams",
            new CreateTeamRequest("Benfica", "Portugal"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTeam = await createResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        Assert.NotNull(createdTeam);

        var response = await client.PostAsJsonAsync(
            "/teams/update",
            new
            {
                Id = createdTeam.Id,
                Name = "SL Benfica",
                Country = string.Empty
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTeam_WithDuplicateNameAndCountry_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var firstResponse = await client.PostAsJsonAsync(
            "/teams",
            new CreateTeamRequest("Benfica", "Portugal"));

        var secondResponse = await client.PostAsJsonAsync(
            "/teams",
            new CreateTeamRequest("Porto", "Portugal"));

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);

        var firstTeam = await firstResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        Assert.NotNull(firstTeam);

        var response = await client.PostAsJsonAsync(
            "/teams/update",
            new
            {
                Id = firstTeam.Id,
                Name = "Porto",
                Country = "Portugal"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTeam_WithExistingId_ReturnsNoContentAndRemovesTeam()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/teams",
            new CreateTeamRequest("Benfica", "Portugal"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTeam = await createResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        Assert.NotNull(createdTeam);

        var deleteResponse = await client.DeleteAsync($"/teams/{createdTeam.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/teams/{createdTeam.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTeam_WithUnknownId_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync($"/teams/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
