using Carter;
using FootballDataPlatform.Application.Teams;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public class DeleteTeamEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/teams/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteTeamCommand(publicId), cancellationToken);
            if (!result.IsSuccess) return Results.NotFound(result.Error);
            return Results.NoContent();
        })
        .WithName("DeleteTeam").WithTags("Teams").WithSummary("Delete a team by its public identifier")
        .Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).ProducesValidationProblem();
    }
}