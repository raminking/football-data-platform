using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Infrastructure.ExternalData;
using FootballDataPlatform.Infrastructure.ExternalData.FootballDataOrg;
using FootballDataPlatform.Infrastructure.Persistence;
using FootballDataPlatform.Infrastructure.Persistence.Competitions;
using FootballDataPlatform.Infrastructure.Persistence.ExternalData;
using FootballDataPlatform.Infrastructure.Persistence.Match;
using FootballDataPlatform.Infrastructure.Persistence.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FootballDataPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FootballDataDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ICompetitionRepository, CompetitionRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IExternalIdentityRepository, ExternalIdentityRepository>();
        services.AddScoped<IImportDataStatusReader, ImportDataStatusReader>();

        services.AddOptions<FootballDataOrgOptions>()
            .Bind(configuration.GetSection(FootballDataOrgOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _), "FootballDataOrg:BaseUrl must be a valid absolute URI.")
            .ValidateOnStart();

        services.AddOptions<ExternalDataRetryOptions>()
            .Bind(configuration.GetSection(ExternalDataRetryOptions.SectionName))
            .Validate(options => options.MaxRetries >= 0, "ExternalData:Retry:MaxRetries must not be negative.")
            .Validate(options => options.BaseDelaySeconds >= 0, "ExternalData:Retry:BaseDelaySeconds must not be negative.")
            .Validate(options => options.MaxDelaySeconds >= options.BaseDelaySeconds, "ExternalData:Retry:MaxDelaySeconds must be greater than or equal to BaseDelaySeconds.")
            .Validate(options => options.JitterRatio >= 0 && options.JitterRatio <= 1, "ExternalData:Retry:JitterRatio must be between 0 and 1.")
            .ValidateOnStart();

        services.AddTransient<ExternalDataRetryHandler>();
        services.AddHttpClient<FootballDataOrgProvider>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FootballDataOrgOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<ExternalDataRetryHandler>();

        services.AddScoped<IFootballDataSource>(serviceProvider =>
            serviceProvider.GetRequiredService<FootballDataOrgProvider>());
        services.AddScoped<IFootballDataSourceResolver, FootballDataSourceResolver>();

        return services;
    }
}
