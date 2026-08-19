using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballDataPlatform.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(team => team.Id);

        builder.Property(team => team.LogoUrl)
            .HasMaxLength(2048);

        builder.Property(team => team.OfficialWebsiteUrl)
            .HasMaxLength(2048);

        builder
            .HasIndex(team => new { team.Name, team.Country })
            .IsUnique();
    }
}