using FootballDataPlatform.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence;

public sealed class EfUnitOfWork(FootballDataDbContext dbContext) : IUnitOfWork
{
    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await operation();
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}
