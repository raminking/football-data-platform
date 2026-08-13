using FootballDataPlatform.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FootballDataPlatform.Infrastructure.Persistence;
using FootballDataPlatform.Infrastructure.Persistence.Teams;

namespace FootballDataPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FootballDataDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITeamRepository, TeamRepository>();
        return services;
    }
}