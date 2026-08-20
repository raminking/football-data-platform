using FootballDataPlatform.Infrastructure.Persistence;
using FootballDataPlatform.Infrastructure.Persistence.ExternalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FootballDataPlatform.Tests.Infrastructure.ExternalData;

public sealed class ExternalIdentityIntegrationTests
{
    [Fact]
    public async Task SaveChangesAsync_ShouldRejectDuplicateProviderEntityTypeAndExternalId()
    {
        await using var factory = new ExternalIdentityWebApplicationFactory();
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FootballDataDbContext>();

        var first = new ExternalIdentity("football-data.org", "Competition", "2021", 1);
        var duplicate = new ExternalIdentity("football-data.org", "Competition", "2021", 2);

        dbContext.ExternalIdentities.AddRange(first, duplicate);
        await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
    }

    private sealed class ExternalIdentityWebApplicationFactory : CustomWebApplicationFactory { }
}
