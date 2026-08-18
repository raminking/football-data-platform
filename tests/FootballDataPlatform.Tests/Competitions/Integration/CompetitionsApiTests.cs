using System.Net;
using System.Net.Http.Json;
using FootballDataPlatform.Contracts.Competitions;

namespace FootballDataPlatform.Tests.Competitions.Integration;

public class CompetitionsApiTests
{
    [Fact]
    public async Task CreateCompetition_WithValidRequest_ReturnsCreated()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/competitions", new CreateCompetitionRequest("Premier League", "England", "EPL"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreateCompetition_WithDuplicateIdentity_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = new CreateCompetitionRequest("Premier League", "England", "EPL");

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/competitions", request)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/competitions", request)).StatusCode);
    }

    [Fact]
    public async Task CreateCompetition_WithMissingCode_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/competitions", new CreateCompetitionRequest("Premier League", "England", ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCompetition_WithExistingId_ReturnsCompetition()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = new CreateCompetitionRequest("La Liga", "Spain", "LL");

        var create = await client.PostAsJsonAsync("/competitions", request);
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotNull(created);

        var response = await client.GetAsync($"/competitions/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CompetitionResponse>();
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Country, result.Country);
        Assert.Equal(request.Code, result.Code);
    }

    [Fact]
    public async Task GetCompetition_WithUnknownId_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.NotFound, await GetStatus(client, $"/competitions/{Guid.NewGuid()}"));
    }

    [Fact]
    public async Task UpdateCompetition_WithValidRequest_ReturnsOkAndUpdatesCompetition()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        var create = await client.PostAsJsonAsync("/competitions", new CreateCompetitionRequest("La Liga", "Spain", "LL"));
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotNull(created);

        var response = await client.PutAsJsonAsync($"/competitions/{created.Id}", new CreateCompetitionRequest("La Liga EA", "Spain", "LLA"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await client.GetFromJsonAsync<CompetitionResponse>($"/competitions/{created.Id}");
        Assert.NotNull(updated);
        Assert.Equal("La Liga EA", updated.Name);
        Assert.Equal("LLA", updated.Code);
    }

    [Fact]
    public async Task UpdateCompetition_WithUnknownId_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/competitions/{Guid.NewGuid()}", new CreateCompetitionRequest("La Liga", "Spain", "LL"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCompetition_WithExistingId_ReturnsNoContentAndRemovesCompetition()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        var create = await client.PostAsJsonAsync("/competitions", new CreateCompetitionRequest("Bundesliga", "Germany", "BL"));
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotNull(created);

        var delete = await client.DeleteAsync($"/competitions/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, await GetStatus(client, $"/competitions/{created.Id}"));
    }

    [Fact]
    public async Task DeleteCompetition_WithUnknownId_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.NotFound, await GetStatus(client, $"/competitions/{Guid.NewGuid()}"));
    }

    private static async Task<HttpStatusCode> GetStatus(HttpClient client, string url) =>
        (await client.GetAsync(url)).StatusCode;

    private record IdResponse(Guid Id);
}