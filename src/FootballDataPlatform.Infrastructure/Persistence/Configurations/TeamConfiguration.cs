using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballDataPlatform.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(team => team.Id);

        builder
            .HasIndex(team => new { team.Name, team.Country })
            .IsUnique();
    }
}