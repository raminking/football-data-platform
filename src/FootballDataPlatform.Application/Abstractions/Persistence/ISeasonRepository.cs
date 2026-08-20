using FootballDataPlatform.Domain.Competitions;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface ISeasonRepository
{
    Task<bool> CompetitionExistsAsync(long competitionId, CancellationToken cancellationToken);
    Task<bool> ExistsByIdentityAsync(long competitionId, string name, long? excludeId, CancellationToken cancellationToken);
    Task CreateAsync(Season season, CancellationToken cancellationToken);
    Task<Season?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Season?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken);
    Task UpdateAsync(Season season, CancellationToken cancellationToken);
    Task DeleteAsync(Season season, CancellationToken cancellationToken);
}