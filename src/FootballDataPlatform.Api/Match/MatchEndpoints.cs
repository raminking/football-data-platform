using Carter;
using FootballDataPlatform.Application.Match;
using FootballDataPlatform.Contracts.Match;
using MediatR;

namespace FootballDataPlatform.Api.Match;

public sealed class MatchEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/matches", async (CreateMatchRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateMatchCommand(request.SeasonPublicId, request.HomeTeamPublicId, request.AwayTeamPublicId,
                request.ScheduledAt, request.Stage, request.Status, request.HomeScore, request.AwayScore, request.HalfTimeHomeScore, request.HalfTimeAwayScore), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Created($"/matches/{result.Value}", new { publicId = result.Value });
        }).WithTags("Matches").Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/matches/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMatchQuery(publicId), ct);
            if (!result.IsSuccess) return Results.NotFound();
            return Results.Ok(ToResponse(result.Value!));
        }).WithTags("Matches").Produces<MatchResponse>().Produces(StatusCodes.Status404NotFound);

        app.MapPut("/matches/{publicId:guid}", async (Guid publicId, CreateMatchRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateMatchCommand(publicId, request.ScheduledAt, request.Stage, request.Status,
                request.HomeScore, request.AwayScore, request.HalfTimeHomeScore, request.HalfTimeAwayScore), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Ok(new { publicId });
        }).WithTags("Matches").Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/matches/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteMatchCommand(publicId), ct);
            if (!result.IsSuccess) return Results.NotFound();
            return Results.NoContent();
        }).WithTags("Matches").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
    }

    private static MatchResponse ToResponse(Domain.Match.Match match) =>
        new(match.PublicId, match.ScheduledAt, match.Stage, match.Status, match.HomeScore, match.AwayScore,
            match.HalfTimeHomeScore, match.HalfTimeAwayScore, match.Result);
}