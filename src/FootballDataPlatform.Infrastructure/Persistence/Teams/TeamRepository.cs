using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Teams;

public class TeamRepository(FootballDataDbContext context) : ITeamRepository
{
    public Task<bool> ExistsByNameAsync(string name, string country, long? excludeId, CancellationToken cancellationToken) =>
        context.Teams.AnyAsync(team => team.Name == name && team.Country == country && (!excludeId.HasValue || team.Id != excludeId.Value), cancellationToken);

    public async Task CreateAsync(Team team, CancellationToken cancellationToken)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken)
    {
        context.Teams.Update(team);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Team?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        context.Teams.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Team?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken) =>
        context.Teams.FirstOrDefaultAsync(t => t.PublicId == publicId, cancellationToken);

    public async Task DeleteAsync(Team team, CancellationToken cancellationToken)
    {
        context.Teams.Remove(team);
        await context.SaveChangesAsync(cancellationToken);
    }
}