using FootballDataPlatform.Infrastructure.Persistence.ExternalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballDataPlatform.Infrastructure.Persistence.Configurations;

public sealed class ExternalIdentityConfiguration : IEntityTypeConfiguration<ExternalIdentity>
{
    public void Configure(EntityTypeBuilder<ExternalIdentity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Provider)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ExternalId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.InternalEntityId)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.Provider,
            x.EntityType,
            x.ExternalId
        })
        .IsUnique();
    }
}
