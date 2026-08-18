using FootballDataPlatform.Domain.Competitions;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface ISeasonRepository
{
    Task<bool> CompetitionExistsAsync(Guid competitionId, CancellationToken cancellationToken);
    Task<bool> ExistsByIdentityAsync(Guid competitionId, string name, CancellationToken cancellationToken);
    Task CreateAsync(Season season, CancellationToken cancellationToken);
    Task<Season?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Season season, CancellationToken cancellationToken);
    Task DeleteAsync(Season season, CancellationToken cancellationToken);
}