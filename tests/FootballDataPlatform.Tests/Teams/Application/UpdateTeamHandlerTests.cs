using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Teams;
using Moq;
using TeamEntity = FootballDataPlatform.Domain.Teams.Team;

namespace FootballDataPlatform.Tests.Teams.Application;

public class UpdateTeamHandlerTests
{
    private readonly Mock<ITeamRepository> _mockRepository = new();

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsFailure()
    {
        var publicId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>())).ReturnsAsync((TeamEntity?)null);
        var result = await new UpdateTeamHandler(_mockRepository.Object).Handle(new UpdateTeamCommand(publicId, "New Name", "New Country"), CancellationToken.None);
        Assert.False(result.IsSuccess); Assert.Equal("Team not found", result.Error);
    }

    [Fact]
    public async Task Handle_DuplicateNameExists_ReturnsFailure()
    {
        var team = new TeamEntity("Old Name", "Old Country");
        _mockRepository.Setup(r => r.GetByPublicIdAsync(team.PublicId, It.IsAny<CancellationToken>())).ReturnsAsync(team);
        _mockRepository.Setup(r => r.ExistsByNameAsync("New Name", "New Country", team.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var result = await new UpdateTeamHandler(_mockRepository.Object).Handle(new UpdateTeamCommand(team.PublicId, "New Name", "New Country"), CancellationToken.None);
        Assert.False(result.IsSuccess); Assert.Contains("exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ValidUpdate_CallsRepositoryUpdate()
    {
        var team = new TeamEntity("Old Name", "Old Country");
        _mockRepository.Setup(r => r.GetByPublicIdAsync(team.PublicId, It.IsAny<CancellationToken>())).ReturnsAsync(team);
        _mockRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<string>(), team.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var result = await new UpdateTeamHandler(_mockRepository.Object).Handle(new UpdateTeamCommand(team.PublicId, "New Name", "New Country"), CancellationToken.None);
        Assert.True(result.IsSuccess); Assert.Equal(team.PublicId, result.Value);
        _mockRepository.Verify(r => r.UpdateAsync(team, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("New Name", team.Name); Assert.Equal("New Country", team.Country);
    }
}
