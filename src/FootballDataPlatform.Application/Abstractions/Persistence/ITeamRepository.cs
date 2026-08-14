using FootballDataPlatform.Domain.Teams;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface ITeamRepository
{
    Task<bool> ExistsByNameAsync(
        string name,
        string country,
        Guid? excludeId, 

        CancellationToken cancellationToken);

    Task CreateAsync(
        Team team,
        CancellationToken cancellationToken);
    Task UpdateAsync(
        Team team,
        CancellationToken cancellationToken);
    Task <Team?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}