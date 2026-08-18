using FootballDataPlatform.Domain.Competitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballDataPlatform.Infrastructure.Persistence.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();

        builder.HasOne<Competition>()
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CompetitionId, x.Name }).IsUnique();
    }
}