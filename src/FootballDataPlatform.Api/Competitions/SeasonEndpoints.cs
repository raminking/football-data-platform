using Carter;
using FootballDataPlatform.Application.Competitions;
using FootballDataPlatform.Contracts.Competitions;
using MediatR;

namespace FootballDataPlatform.Api.Competitions;

public sealed class SeasonEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/seasons", async (CreateSeasonRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateSeasonCommand(request.CompetitionId, request.Name, request.StartDate, request.EndDate), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Created($"/seasons/{result.Value}", new { id = result.Value });
        }).WithTags("Seasons").Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/seasons/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSeasonQuery(id), ct);
            if (!result.IsSuccess) return Results.NotFound();
            var season = result.Value!;
            return Results.Ok(new SeasonResponse(season.Id, season.CompetitionId, season.Name, season.StartDate, season.EndDate));
        }).WithTags("Seasons").Produces<SeasonResponse>().Produces(StatusCodes.Status404NotFound);

        app.MapPut("/seasons/{id:guid}", async (Guid id, CreateSeasonRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateSeasonCommand(id, request.Name, request.StartDate, request.EndDate), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Ok(new { id });
        }).WithTags("Seasons").Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/seasons/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteSeasonCommand(id), ct);
            if (!result.IsSuccess) return Results.NotFound();
            return Results.NoContent();
        }).WithTags("Seasons").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
    }
}