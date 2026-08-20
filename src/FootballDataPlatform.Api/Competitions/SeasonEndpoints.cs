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
            var result = await sender.Send(new CreateSeasonCommand(request.CompetitionPublicId, request.Name, request.StartDate, request.EndDate), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Created($"/seasons/{result.Value}", new { publicId = result.Value });
        }).WithTags("Seasons").Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/seasons/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSeasonQuery(publicId), ct);
            if (!result.IsSuccess) return Results.NotFound();
            var season = result.Value!;
            return Results.Ok(new SeasonResponse(season.PublicId, season.Name, season.StartDate, season.EndDate));
        }).WithTags("Seasons").Produces<SeasonResponse>().Produces(StatusCodes.Status404NotFound);

        app.MapPut("/seasons/{publicId:guid}", async (Guid publicId, CreateSeasonRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateSeasonCommand(publicId, request.Name, request.StartDate, request.EndDate), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Ok(new { publicId });
        }).WithTags("Seasons").Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/seasons/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteSeasonCommand(publicId), ct);
            if (!result.IsSuccess) return Results.NotFound();
            return Results.NoContent();
        }).WithTags("Seasons").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
    }
}