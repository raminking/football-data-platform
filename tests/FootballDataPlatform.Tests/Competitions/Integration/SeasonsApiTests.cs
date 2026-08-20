using System.Net;
using System.Net.Http.Json;
using FootballDataPlatform.Contracts.Competitions;

namespace FootballDataPlatform.Tests.Competitions.Integration;

public class SeasonsApiTests
{
    [Fact]
    public async Task CreateSeason_WithValidRequest_ReturnsCreated()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); var competitionId = await CreateCompetition(client);
        var response = await client.PostAsJsonAsync("/seasons", new CreateSeasonRequest(competitionId, "2025/26", new DateOnly(2025, 8, 1), new DateOnly(2026, 5, 31)));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode); var result = await response.Content.ReadFromJsonAsync<IdResponse>(); Assert.NotNull(result); Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreateSeason_WithUnknownCompetition_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/seasons", new CreateSeasonRequest(Guid.NewGuid(), "2025/26", new DateOnly(2025, 8, 1), new DateOnly(2026, 5, 31)))).StatusCode);
    }

    [Fact]
    public async Task CreateSeason_WithDuplicateName_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); var competitionId = await CreateCompetition(client); var request = new CreateSeasonRequest(competitionId, "2025/26", new DateOnly(2025, 8, 1), new DateOnly(2026, 5, 31));
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/seasons", request)).StatusCode); Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/seasons", request)).StatusCode);
    }

    [Fact]
    public async Task CreateSeason_WithInvalidDateRange_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); var competitionId = await CreateCompetition(client);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/seasons", new CreateSeasonRequest(competitionId, "2025/26", new DateOnly(2026, 5, 31), new DateOnly(2025, 8, 1)))).StatusCode);
    }

    [Fact]
    public async Task GetSeason_WithExistingId_ReturnsSeason()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); var competitionId = await CreateCompetition(client); var request = new CreateSeasonRequest(competitionId, "2025/26", new DateOnly(2025, 8, 1), new DateOnly(2026, 5, 31)); var created = await CreateSeason(client, request);
        var response = await client.GetAsync($"/seasons/{created.Id}"); Assert.Equal(HttpStatusCode.OK, response.StatusCode); var result = await response.Content.ReadFromJsonAsync<SeasonResponse>(); Assert.NotNull(result);
        Assert.Equal(created.Id, result.PublicId); Assert.Equal(request.Name, result.Name); Assert.Equal(request.StartDate, result.StartDate);
    }

    [Fact]
    public async Task GetSeason_WithUnknownId_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/seasons/{Guid.NewGuid()}")).StatusCode);
    }

    [Fact]
    public async Task UpdateSeason_WithValidRequest_ReturnsOkAndUpdatesSeason()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); var competitionId = await CreateCompetition(client); var created = await CreateSeason(client, new CreateSeasonRequest(competitionId, "2025/26", new DateOnly(2025, 8, 1), new DateOnly(2026, 5, 31))); var request = new CreateSeasonRequest(competitionId, "2025-26", new DateOnly(2025, 8, 15), new DateOnly(2026, 6, 1));
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync($"/seasons/{created.Id}", request)).StatusCode); var updated = await client.GetFromJsonAsync<SeasonResponse>($"/seasons/{created.Id}"); Assert.NotNull(updated); Assert.Equal(request.Name, updated.Name); Assert.Equal(request.StartDate, updated.StartDate);
    }

    [Fact]
    public async Task UpdateSeason_WithUnknownId_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); var competitionId = await CreateCompetition(client);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PutAsJsonAsync($"/seasons/{Guid.NewGuid()}", new CreateSeasonRequest(competitionId, "2025/26", new DateOnly(2025, 8, 1), new DateOnly(2026, 5, 31)))).StatusCode);
    }

    [Fact]
    public async Task DeleteSeason_WithExistingId_ReturnsNoContentAndRemovesSeason()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); var competitionId = await CreateCompetition(client); var created = await CreateSeason(client, new CreateSeasonRequest(competitionId, "2025/26", new DateOnly(2025, 8, 1), new DateOnly(2026, 5, 31)));
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/seasons/{created.Id}")).StatusCode); Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/seasons/{created.Id}")).StatusCode);
    }

    [Fact]
    public async Task DeleteSeason_WithUnknownId_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory(); using var client = factory.CreateClient(); Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/seasons/{Guid.NewGuid()}")).StatusCode);
    }

    private static async Task<Guid> CreateCompetition(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/competitions", new CreateCompetitionRequest($"Competition-{Guid.NewGuid():N}", "England", $"C{Random.Shared.Next(100000, 999999)}")); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }
    private static async Task<IdResponse> CreateSeason(HttpClient client, CreateSeasonRequest request)
    { var response = await client.PostAsJsonAsync("/seasons", request); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<IdResponse>())!; }
    private record IdResponse(Guid Id);
}
