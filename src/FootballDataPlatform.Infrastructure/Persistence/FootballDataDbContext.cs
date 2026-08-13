using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;
namespace FootballDataPlatform.Infrastructure.Persistence;

public class FootballDataDbContext(DbContextOptions<FootballDataDbContext> options)
    : DbContext(options)
{
   
    public DbSet<Team> Teams => Set<Team>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(FootballDataDbContext).Assembly);
    }
}

