using FootballDataPlatform.Domain.Competitions;
using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence;

public class FootballDataDbContext(DbContextOptions<FootballDataDbContext> options) : DbContext(options)
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Competition> Competitions => Set<Competition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FootballDataDbContext).Assembly);
    }
}