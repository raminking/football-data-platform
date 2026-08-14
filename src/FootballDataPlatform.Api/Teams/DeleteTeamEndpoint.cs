using Carter;
using FootballDataPlatform.Application.Teams;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public class DeleteTeamEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        
        app.MapDelete("/teams/{id}", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteTeamCommand(id);
                var result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.NotFound(result.Error); // 404 برای پیدا نشدن تیم

                return Results.NoContent(); // 204 برای موفقیت بدون محتوای بازگشتی
            })
            .WithName("DeleteTeam")
            .WithTags("Teams")
            .WithSummary("Delete a team by ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }
}