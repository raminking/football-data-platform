using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.ExternalData;
using FootballDataPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class TeamImportServiceIntegrationTests
{
    [Fact]
    public async Task ImportAsync_ShouldPersistTeamAndExternalIdentity_AndBeIdempotent()
    {
        await using var factory = new TeamImportWebApplicationFactory();
        using var scope = factory.Services.CreateScope();
        var importService = scope.ServiceProvider.GetRequiredService<ITeamImportService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FootballDataDbContext>();
        var source = Assert.IsType<FakeFootballDataSource>(scope.ServiceProvider.GetRequiredService<IFootballDataSource>());

        var firstResult = await importService.ImportAsync("football-data.org", "PL", 2025);
        Assert.Equal(1, firstResult.Created); Assert.Equal(0, firstResult.Updated); Assert.Equal(0, firstResult.Skipped); Assert.Empty(firstResult.Errors);
        Assert.Equal(1, await dbContext.Teams.CountAsync()); Assert.Equal(1, await dbContext.ExternalIdentities.CountAsync());
        var team = await dbContext.Teams.SingleAsync();
        Assert.Equal("Liverpool FC", team.Name); Assert.Equal("England", team.Country);
        var identity = await dbContext.ExternalIdentities.SingleAsync();
        Assert.Equal("football-data.org", identity.Provider); Assert.Equal("Team", identity.EntityType); Assert.Equal("64", identity.ExternalId); Assert.Equal(team.Id, identity.InternalEntityId);

        source.SetTeams(new ExternalTeam("64", "Liverpool", "England"));
        var secondResult = await importService.ImportAsync("football-data.org", "PL", 2025);
        Assert.Equal(0, secondResult.Created); Assert.Equal(1, secondResult.Updated); Assert.Equal(0, secondResult.Skipped); Assert.Empty(secondResult.Errors);
        Assert.Equal(1, await dbContext.Teams.CountAsync()); Assert.Equal(1, await dbContext.ExternalIdentities.CountAsync());
        Assert.Equal("Liverpool", (await dbContext.Teams.SingleAsync()).Name);
    }

    private sealed class TeamImportWebApplicationFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IFootballDataSource>();
                services.AddSingleton<IFootballDataSource>(new FakeFootballDataSource());
            });
        }
    }

    private sealed class FakeFootballDataSource : IFootballDataSource
    {
        private IReadOnlyCollection<ExternalTeam> _teams = [new ExternalTeam("64", "Liverpool FC", "England")];
        public string SourceKey => "football-data.org";
        public void SetTeams(params ExternalTeam[] teams) => _teams = teams;
        public Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExternalCompetition>>([]);
        public Task<IReadOnlyCollection<ExternalSeason>> GetSeasonsAsync(string competitionCode, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExternalSeason>>([]);
        public Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) => Task.FromResult(_teams);
        public Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExternalMatch>>([]);
    }
}
