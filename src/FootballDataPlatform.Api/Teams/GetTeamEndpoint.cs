using Carter;
using FootballDataPlatform.Application.Teams;
using FootballDataPlatform.Contracts.Teams;
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

                if (team is null)
                    return Results.NotFound();

                var response = new GetTeamResponse(
                    team.Id,
                    team.Name,
                    team.Country,
                    team.LogoUrl,
                    team.OfficialWebsiteUrl);

                return Results.Ok(response);
            })
            .WithName("GetTeam")
            .WithTags("Teams")
            .WithSummary("Get a team by its identifier")
            .Produces<GetTeamResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}