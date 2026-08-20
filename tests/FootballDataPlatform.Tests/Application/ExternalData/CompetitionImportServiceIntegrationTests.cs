using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.ExternalData;
using FootballDataPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class CompetitionImportServiceIntegrationTests
{
    [Fact]
    public async Task ImportAsync_ShouldPersistCompetitionAndExternalIdentity_AndBeIdempotent()
    {
        await using var factory = new CompetitionImportWebApplicationFactory();
        using var scope = factory.Services.CreateScope();

        var importService = scope.ServiceProvider.GetRequiredService<ICompetitionImportService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FootballDataDbContext>();
        var provider = Assert.IsType<FakeFootballDataProvider>(scope.ServiceProvider.GetRequiredService<IFootballDataProvider>());

        var firstResult = await importService.ImportAsync();

        Assert.Equal(1, firstResult.Created);
        Assert.Equal(0, firstResult.Updated);
        Assert.Equal(0, firstResult.Skipped);
        Assert.Empty(firstResult.Errors);
        Assert.Equal(1, await dbContext.Competitions.CountAsync());
        Assert.Equal(1, await dbContext.ExternalIdentities.CountAsync());

        var competition = await dbContext.Competitions.SingleAsync();
        Assert.Equal("Premier League", competition.Name);
        Assert.Equal("England", competition.Country);
        Assert.Equal("PL", competition.Code);

        var identity = await dbContext.ExternalIdentities.SingleAsync();
        Assert.Equal("football-data.org", identity.Provider);
        Assert.Equal("Competition", identity.EntityType);
        Assert.Equal("2021", identity.ExternalId);
        Assert.Equal(competition.Id, identity.InternalEntityId);

        provider.SetCompetitions(new ExternalCompetition("2021", "Premier League Updated", "PL", "England"));
        var secondResult = await importService.ImportAsync();

        Assert.Equal(0, secondResult.Created);
        Assert.Equal(1, secondResult.Updated);
        Assert.Equal(0, secondResult.Skipped);
        Assert.Empty(secondResult.Errors);
        Assert.Equal(1, await dbContext.Competitions.CountAsync());
        Assert.Equal(1, await dbContext.ExternalIdentities.CountAsync());

        var updatedCompetition = await dbContext.Competitions.SingleAsync();
        Assert.Equal(competition.Id, updatedCompetition.Id);
        Assert.Equal("Premier League Updated", updatedCompetition.Name);
        Assert.Equal("England", updatedCompetition.Country);
        Assert.Equal("PL", updatedCompetition.Code);
    }

    private sealed class CompetitionImportWebApplicationFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IFootballDataProvider>();
                services.AddSingleton<IFootballDataProvider>(new FakeFootballDataProvider());
            });
        }
    }

    private sealed class FakeFootballDataProvider : IFootballDataProvider
    {
        private IReadOnlyCollection<ExternalCompetition> _competitions =
            [new ExternalCompetition("2021", "Premier League", "PL", "England")];

        public string ProviderName => "football-data.org";

        public void SetCompetitions(params ExternalCompetition[] competitions) => _competitions = competitions;

        public Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_competitions);

        public Task<IReadOnlyCollection<ExternalSeason>> GetSeasonsAsync(string competitionCode, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExternalSeason>>([]);

        public Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExternalTeam>>([]);

        public Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExternalMatch>>([]);
    }
}
