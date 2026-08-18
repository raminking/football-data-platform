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
            var result = await sender.Send(new CreateMatchCommand(
                request.SeasonId, request.HomeTeamId, request.AwayTeamId,
                request.ScheduledAt, request.Stage, request.Status,
                request.HomeScore, request.AwayScore,
                request.HalfTimeHomeScore, request.HalfTimeAwayScore), ct);

            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Created($"/matches/{result.Value}", new { id = result.Value });
        }).WithTags("Matches").Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/matches/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMatchQuery(id), ct);
            if (!result.IsSuccess) return Results.NotFound();
            return Results.Ok(ToResponse(result.Value!));
        }).WithTags("Matches").Produces<MatchResponse>().Produces(StatusCodes.Status404NotFound);

        app.MapPut("/matches/{id:guid}", async (Guid id, CreateMatchRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateMatchCommand(
                id, request.ScheduledAt, request.Stage, request.Status,
                request.HomeScore, request.AwayScore,
                request.HalfTimeHomeScore, request.HalfTimeAwayScore), ct);

            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Ok(new { id });
        }).WithTags("Matches").Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/matches/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteMatchCommand(id), ct);
            if (!result.IsSuccess) return Results.NotFound();
            return Results.NoContent();
        }).WithTags("Matches").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
    }

    private static MatchResponse ToResponse(FootballDataPlatform.Domain.Match.Match match) =>
        new(match.Id, match.SeasonId, match.HomeTeamId, match.AwayTeamId,
            match.ScheduledAt, match.Stage, match.Status,
            match.HomeScore, match.AwayScore,
            match.HalfTimeHomeScore, match.HalfTimeAwayScore,
            match.Result);
}