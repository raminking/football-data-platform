using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Teams;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Teams;

public class TeamRepository(FootballDataDbContext context)
    : ITeamRepository
{
    public Task<bool> ExistsByNameAsync(string name, string country, CancellationToken cancellationToken)
    {
        return context.Teams
            .AnyAsync(
                team => team.Name == name && team.Country == country,
                cancellationToken);
    }

    public async Task CreateAsync(Team team, CancellationToken cancellationToken)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);
    }
 
}