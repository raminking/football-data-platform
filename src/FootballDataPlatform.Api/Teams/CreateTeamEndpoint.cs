using Carter;
using FootballDataPlatform.Application.Teams.CreateTeam;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public sealed class CreateTeamModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/teams", async (
                CreateTeamCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.BadRequest(result);

                return Results.Ok(result);
            })
            .WithName("CreateTeam")
            .WithTags("Teams")
            .WithSummary("Create a new team")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}