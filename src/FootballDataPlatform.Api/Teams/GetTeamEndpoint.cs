using Carter;
using FootballDataPlatform.Application.Teams.GetTeam;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public sealed class GetTeamModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/teams/{id:guid}", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetTeamQuery(id);

                var team = await sender.Send(query, cancellationToken);

                return team is null
                    ? Results.NotFound()
                    : Results.Ok(team);
            })
            .WithName("GetTeam")
            .WithTags("Teams")
            .WithSummary("Get a team by its identifier")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}