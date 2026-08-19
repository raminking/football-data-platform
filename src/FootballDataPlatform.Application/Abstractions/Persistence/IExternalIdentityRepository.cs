using FootballDataPlatform.Infrastructure.Persistence.ExternalData;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface IExternalIdentityRepository
{
    Task<ExternalIdentity?> FindAsync(
        string provider,
        string entityType,
        string externalId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ExternalIdentity identity,
        CancellationToken cancellationToken = default);
}
