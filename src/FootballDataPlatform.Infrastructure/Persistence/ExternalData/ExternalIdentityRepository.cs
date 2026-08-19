using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.ExternalData;

public sealed class ExternalIdentityRepository(FootballDataDbContext dbContext) : IExternalIdentityRepository
{
    public async Task<ExternalIdentityRecord?> FindAsync(
        string provider,
        string entityType,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var identity = await dbContext.ExternalIdentities
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Provider == provider &&
                     x.EntityType == entityType &&
                     x.ExternalId == externalId,
                cancellationToken);

        return identity is null
            ? null
            : new ExternalIdentityRecord(
                identity.Provider,
                identity.EntityType,
                identity.ExternalId,
                identity.InternalEntityId,
                identity.CreatedAtUtc);
    }

    public async Task AddAsync(
        ExternalIdentityRecord identity,
        CancellationToken cancellationToken = default)
    {
        var entity = new ExternalIdentity(
            identity.Provider,
            identity.EntityType,
            identity.ExternalId,
            identity.InternalEntityId);

        dbContext.ExternalIdentities.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
