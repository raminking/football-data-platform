using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.ExternalData;
using Microsoft.Extensions.DependencyInjection;

namespace FootballDataPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<ITeamImportService, TeamImportService>();
        services.AddScoped<ICompetitionImportService, CompetitionImportService>();
        services.AddScoped<ISeasonImportService, SeasonImportService>();
        services.AddScoped<IMatchImportService, MatchImportService>();

        return services;
    }
}
