using FootballDataPlatform.Domain.Competitions;
using FootballDataPlatform.Domain.Match;
using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballDataPlatform.Infrastructure.Persistence.Configurations;

public sealed class MatchConfiguration : IEntityTypeConfiguration<Domain.Match.Match>
{
    public void Configure(EntityTypeBuilder<Domain.Match.Match> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScheduledAt).IsRequired();
        builder.Property(x => x.Stage).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Result).HasConversion<string>().HasMaxLength(20);

        builder.Property(x => x.HomeScore);
        builder.Property(x => x.AwayScore);
        builder.Property(x => x.HalfTimeHomeScore);
        builder.Property(x => x.HalfTimeAwayScore);

        builder.HasOne<Season>()
            .WithMany()
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(x => x.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(x => x.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(x => new { x.SeasonId, x.ScheduledAt });
        builder.HasIndex(x => x.HomeTeamId);
        builder.HasIndex(x => x.AwayTeamId);
    }
}