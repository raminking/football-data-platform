using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Teams;
using Moq;

namespace FootballDataPlatform.Tests.Teams.Application;

public class GetTeamHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTeam_WhenTeamExists()
    {
        var team = new FootballDataPlatform.Domain.Teams.Team("Benfica", "Portugal");
        var repository = new Mock<ITeamRepository>();
        repository.Setup(x => x.GetByPublicIdAsync(team.PublicId, It.IsAny<CancellationToken>())).ReturnsAsync(team);
        var handler = new GetTeamHandler(repository.Object);

        var result = await handler.Handle(new GetTeamQuery(team.PublicId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(team.PublicId, result.PublicId);
        Assert.Equal("Benfica", result.Name);
    }
}
