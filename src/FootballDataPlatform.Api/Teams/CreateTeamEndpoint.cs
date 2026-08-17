using Carter;
using FootballDataPlatform.Application.Teams;
using FootballDataPlatform.Contracts.Teams;
using MediatR;

namespace FootballDataPlatform.Api.Teams;

public sealed class CreateTeamModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/teams", async (
                CreateTeamRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateTeamCommand(
                    request.Name,
                    request.Country);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                if (!result.IsSuccess)
                    return Results.BadRequest(result.Error);

                return Results.Ok(
                    new CreateTeamResponse(result.Value));
            })
            .WithName("CreateTeam")
            .WithTags("Teams")
            .WithSummary("Create a new team")
            .Produces<CreateTeamResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}