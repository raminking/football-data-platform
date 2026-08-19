using FootballDataPlatform.Domain.Competitions;
using MatchEntity = FootballDataPlatform.Domain.Match.Match;
using FootballDataPlatform.Domain.Teams;
using FootballDataPlatform.Infrastructure.Persistence.ExternalData;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence;

public class FootballDataDbContext(DbContextOptions<FootballDataDbContext> options) : DbContext(options)
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Competition> Competitions => Set<Competition>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<MatchEntity> Matches => Set<MatchEntity>();
    public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FootballDataDbContext).Assembly);
    }
}
