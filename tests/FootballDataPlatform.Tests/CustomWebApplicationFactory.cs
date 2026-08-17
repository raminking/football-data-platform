using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using FootballDataPlatform.Infrastructure.Persistence;

namespace FootballDataPlatform.Tests;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("football_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _postgres
            .StartAsync()
            .GetAwaiter()
            .GetResult();

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        _postgres.GetConnectionString()
                });
        });

        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<FootballDataDbContext>();

            dbContext.Database.Migrate();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _postgres
                .DisposeAsync()
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }

        base.Dispose(disposing);
    }
}