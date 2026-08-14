using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Teams;

public class TeamRepository(FootballDataDbContext context)
    : ITeamRepository
{
    public Task<bool> ExistsByNameAsync(string name, string country,Guid? excludeId, 
        CancellationToken cancellationToken)
    {
        return context.Teams
            .AnyAsync(
                team => team.Name == name && team.Country == country && team.Id != excludeId,
                cancellationToken);
    }

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

    public async Task<Team?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await context.Teams.FirstOrDefaultAsync(
            t => t.Id == id,
            cancellationToken);
    }

    public async Task DeleteAsync(Team team, CancellationToken cancellationToken)
    {
        context.Teams.Remove(team);
        await context.SaveChangesAsync(cancellationToken);
    }
}