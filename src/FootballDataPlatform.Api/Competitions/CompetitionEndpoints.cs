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
            return Results.Created($"/competitions/{result.Value}", new { publicId = result.Value });
        }).WithTags("Competitions").Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/competitions/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken ct) =>
        {
            var competition = await sender.Send(new GetCompetitionQuery(publicId), ct);
            if (competition is null) return Results.NotFound();
            return Results.Ok(new CompetitionResponse(competition.PublicId, competition.Name, competition.Country, competition.Code));
        }).WithTags("Competitions").Produces<CompetitionResponse>().Produces(StatusCodes.Status404NotFound);

        app.MapPut("/competitions/{publicId:guid}", async (Guid publicId, CreateCompetitionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateCompetitionCommand(publicId, request.Name, request.Country, request.Code), ct);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Ok(new { publicId = result.Value });
        }).WithTags("Competitions").Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/competitions/{publicId:guid}", async (Guid publicId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteCompetitionCommand(publicId), ct);
            if (!result.IsSuccess) return Results.NotFound(result.Error);
            return Results.NoContent();
        }).WithTags("Competitions").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
    }
}