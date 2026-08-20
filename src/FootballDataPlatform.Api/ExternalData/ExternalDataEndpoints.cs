using Carter;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.ExternalData;

namespace FootballDataPlatform.Api.ExternalData;

public sealed class ExternalDataEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/imports/{sourceKey}/{competitionCode}/{seasonYear:int}", async (
            string sourceKey,
            string competitionCode,
            int seasonYear,
            IFootballDataImportOrchestrator orchestrator,
            CancellationToken cancellationToken) =>
        {
            var result = await orchestrator.ImportCompetitionAsync(
                sourceKey,
                competitionCode,
                seasonYear,
                cancellationToken);

            return Results.Ok(new
            {
                result.Created,
                result.Updated,
                result.Skipped,
                result.Processed,
                result.Errors
            });
        })
        .WithTags("External Data")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/imports/status", async (
            IImportDataStatusReader statusReader,
            CancellationToken cancellationToken) =>
        {
            var status = await statusReader.GetAsync(cancellationToken);
            return Results.Ok(status);
        })
        .WithTags("External Data")
        .Produces<ImportDataStatus>(StatusCodes.Status200OK);
    }
}
