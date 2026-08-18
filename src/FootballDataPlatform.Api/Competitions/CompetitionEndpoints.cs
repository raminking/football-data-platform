using Carter;
using FootballDataPlatform.Application.Competitions;
using FootballDataPlatform.Contracts.Competitions;
using MediatR;

namespace FootballDataPlatform.Api.Competitions;

public sealed class CompetitionEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/competitions", async (CreateCompetitionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateCompetitionCommand(request.Name, request.Country, request.Code), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Created($"/competitions/{result.Value}", new { id = result.Value });
        }).WithTags("Competitions").Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/competitions/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var competition = await sender.Send(new GetCompetitionQuery(id), ct);
            if (competition is null) return Results.NotFound();
            return Results.Ok(new CompetitionResponse(competition.Id, competition.Name, competition.Country, competition.Code));
        }).WithTags("Competitions").Produces<CompetitionResponse>().Produces(StatusCodes.Status404NotFound);

        app.MapPut("/competitions/{id:guid}", async (Guid id, CreateCompetitionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateCompetitionCommand(id, request.Name, request.Country, request.Code), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Ok(new { id = result.Value });
        }).WithTags("Competitions").Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/competitions/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteCompetitionCommand(id), ct);
            if (!result.IsSuccess) return Results.NotFound(result.Error);
            return Results.NoContent();
        }).WithTags("Competitions").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
    }
}