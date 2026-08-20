using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Teams;
using Moq;
using TeamEntity = FootballDataPlatform.Domain.Teams.Team;

namespace FootballDataPlatform.Tests.Teams.Application;

public class DeleteTeamHandlerTests
{
    private readonly Mock<ITeamRepository> _mockRepository = new();

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsFailure()
    {
        var publicId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>())).ReturnsAsync((TeamEntity?)null);
        var result = await new DeleteTeamHandler(_mockRepository.Object).Handle(new DeleteTeamCommand(publicId), CancellationToken.None);
        Assert.False(result.IsSuccess); Assert.Equal("Team not found", result.Error);
    }

    [Fact]
    public async Task Handle_ValidDelete_CallsRepositoryDelete()
    {
        var team = new TeamEntity("Real Madrid", "Spain");
        _mockRepository.Setup(r => r.GetByPublicIdAsync(team.PublicId, It.IsAny<CancellationToken>())).ReturnsAsync(team);
        var result = await new DeleteTeamHandler(_mockRepository.Object).Handle(new DeleteTeamCommand(team.PublicId), CancellationToken.None);
        Assert.True(result.IsSuccess); Assert.Equal(team.PublicId, result.Value);
        _mockRepository.Verify(r => r.DeleteAsync(team, It.IsAny<CancellationToken>()), Times.Once);
    }
}
