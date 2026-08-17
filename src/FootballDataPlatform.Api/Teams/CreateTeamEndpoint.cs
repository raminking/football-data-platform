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

                var response = new CreateTeamResponse(result.Value);

                return Results.Created(
                    $"/teams/{result.Value}",
                    response);
            })
            .WithName("CreateTeam")
            .WithTags("Teams")
            .WithSummary("Create a new team")
            .Produces<CreateTeamResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}