using FootballDataPlatform.Domain.Teams;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface ITeamRepository
{
    Task<bool> ExistsByNameAsync(string name, string country, long? excludeId, CancellationToken cancellationToken);
    Task CreateAsync(Team team, CancellationToken cancellationToken);
    Task UpdateAsync(Team team, CancellationToken cancellationToken);
    Task<Team?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Team?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken);
    Task DeleteAsync(Team team, CancellationToken cancellationToken);
}