using Carter;
using FootballDataPlatform.Application.Teams;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public sealed class UpdateTeamModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/teams/update", async (
                UpdateTeamCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.BadRequest(result);

                return Results.Ok(result);
            })
            .WithName("UpdateTeam")
            .WithTags("Teams")
            .WithSummary("Update a team")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}