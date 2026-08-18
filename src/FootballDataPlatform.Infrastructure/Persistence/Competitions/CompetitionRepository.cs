using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Competitions;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Competitions;

public class CompetitionRepository(FootballDataDbContext context) : ICompetitionRepository
{
    public Task<bool> ExistsByIdentityAsync(string name, string country, string code, Guid? excludeId, CancellationToken cancellationToken) =>
        context.Competitions.AnyAsync(x => x.Name == name && x.Country == country && x.Code == code && x.Id != excludeId, cancellationToken);

    public async Task CreateAsync(Competition competition, CancellationToken cancellationToken)
    {
        context.Competitions.Add(competition);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Competition?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Competitions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateAsync(Competition competition, CancellationToken cancellationToken)
    {
        context.Competitions.Update(competition);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Competition competition, CancellationToken cancellationToken)
    {
        context.Competitions.Remove(competition);
        await context.SaveChangesAsync(cancellationToken);
    }
}