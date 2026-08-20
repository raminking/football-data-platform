using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FootballDataPlatform.Infrastructure;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider.GetRequiredService<Persistence.FootballDataDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}