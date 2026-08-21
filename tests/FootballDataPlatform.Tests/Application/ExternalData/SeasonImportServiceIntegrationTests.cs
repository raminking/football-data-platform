using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.ExternalData;
using FootballDataPlatform.Infrastructure.Persistence;
using FootballDataPlatform.Infrastructure.Persistence.ExternalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class SeasonImportServiceIntegrationTests
{
    [Fact]
    public async Task ImportAsync_ShouldPersistSeasonAndExternalIdentity_AndBeIdempotent()
    {
        await using var factory = new SeasonImportWebApplicationFactory();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FootballDataDbContext>();
        var importService = scope.ServiceProvider.GetRequiredService<ISeasonImportService>();
        var source = Assert.IsType<FakeFootballDataSource>(scope.ServiceProvider.GetRequiredService<IFootballDataSource>());

        var competition = new Domain.Competitions.Competition("Premier League", "England", "PL");
        db.Competitions.Add(competition);
        await db.SaveChangesAsync();
        db.ExternalIdentities.Add(new ExternalIdentity("football-data.org", "Competition", "2021", competition.Id));
        await db.SaveChangesAsync();

        var first = await importService.ImportAsync("football-data.org", "PL");
        Assert.Equal(1, first.Created); Assert.Equal(0, first.Updated); Assert.Equal(0, first.Skipped); Assert.Empty(first.Errors);
        Assert.Equal(1, await db.Seasons.CountAsync()); Assert.Equal(2, await db.ExternalIdentities.CountAsync());
        var season = await db.Seasons.SingleAsync();
        Assert.Equal(competition.Id, season.CompetitionId); Assert.Equal("2025/26", season.Name);
        Assert.Equal(new DateOnly(2025, 8, 15), season.StartDate); Assert.Equal(new DateOnly(2026, 5, 24), season.EndDate);

        source.SetSeasons(new ExternalSeason("2817", "2021", "2025/26", new DateOnly(2025, 8, 15), new DateOnly(2026, 5, 24)));
        var second = await importService.ImportAsync("football-data.org", "PL");
        Assert.Equal(0, second.Created); Assert.Equal(1, second.Updated); Assert.Equal(0, second.Skipped); Assert.Empty(second.Errors);
        Assert.Equal(1, await db.Seasons.CountAsync()); Assert.Equal(2, await db.ExternalIdentities.CountAsync());
    }

    private sealed class SeasonImportWebApplicationFactory : CustomWebApplicationFactory
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
        private IReadOnlyCollection<ExternalSeason> _seasons = [new ExternalSeason("2817", "2021", "2025/26", new DateOnly(2025, 8, 15), new DateOnly(2026, 5, 24))];
        public string SourceKey => "football-data.org";
        public void SetSeasons(params ExternalSeason[] seasons) => _seasons = seasons;
        public Task<IReadOnlyCollection<ExternalCompetition>> GetCompetitionsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExternalCompetition>>([new ExternalCompetition("2021", "Premier League", "PL", "England")]);
        public Task<IReadOnlyCollection<ExternalSeason>> GetSeasonsAsync(string competitionCode, CancellationToken cancellationToken = default) => Task.FromResult(_seasons);
        public Task<IReadOnlyCollection<ExternalTeam>> GetTeamsAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExternalTeam>>([]);
        public Task<IReadOnlyCollection<ExternalMatch>> GetMatchesAsync(string competitionCode, int seasonYear, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExternalMatch>>([]);
    }
}