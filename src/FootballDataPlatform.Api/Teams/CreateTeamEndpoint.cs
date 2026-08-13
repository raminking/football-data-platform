using FootballDataPlatform.Application.Teams.CreateTeam;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public static class CreateTeamEndpoint
{
    public static void MapCreateTeamEndpoint(this IEndpointRouteBuilder app)
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
        });
    }
}