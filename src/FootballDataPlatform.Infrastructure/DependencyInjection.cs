using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Infrastructure.Persistence;
using FootballDataPlatform.Infrastructure.Persistence.Competitions;
using FootballDataPlatform.Infrastructure.Persistence.Match;
using FootballDataPlatform.Infrastructure.Persistence.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FootballDataPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FootballDataDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ICompetitionRepository, CompetitionRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        return services;
    }
}