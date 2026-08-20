using Carter;
using FootballDataPlatform.Application.Teams;
using FootballDataPlatform.Contracts.Teams;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public sealed class GetTeamModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/teams/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken cancellationToken) =>
        {
            var team = await sender.Send(new GetTeamQuery(publicId), cancellationToken);
            if (team is null) return Results.NotFound();
            return Results.Ok(new GetTeamResponse(team.PublicId, team.Name, team.Country, team.LogoUrl, team.OfficialWebsiteUrl));
        })
        .WithName("GetTeam").WithTags("Teams").WithSummary("Get a team by its public identifier")
        .Produces<GetTeamResponse>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
    }
}