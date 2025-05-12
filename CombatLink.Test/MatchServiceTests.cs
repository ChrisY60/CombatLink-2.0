using Xunit;
using Moq;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using CombatLink.Application.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CombatLink.Tests.Services
{
    public class MatchServiceTests
    {
        private readonly Mock<IMatchRepository> _matchRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MatchService _service;

        public MatchServiceTests()
        {
            _matchRepositoryMock = new Mock<IMatchRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MatchService(_matchRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateMatchIfNotExists_ShouldNotCreate_WhenMatchExists()
        {
            _matchRepositoryMock.Setup(x => x.MatchExists(1, 2)).ReturnsAsync(true);

            var result = await _service.CreateMatchIfNotExists(1, 2);

            Assert.False(result);
            _matchRepositoryMock.Verify(x => x.AddMatch(It.IsAny<Domain.Models.Match>()), Times.Never);
        }

        [Fact]
        public async Task CreateMatchIfNotExists_ShouldCreate_WhenMatchDoesNotExist()
        {
            _matchRepositoryMock.Setup(x => x.MatchExists(1, 2)).ReturnsAsync(false);
            _matchRepositoryMock.Setup(x => x.AddMatch(It.IsAny<Domain.Models.Match>())).ReturnsAsync(true);

            var result = await _service.CreateMatchIfNotExists(1, 2);

            Assert.True(result);
            _matchRepositoryMock.Verify(x => x.AddMatch(It.IsAny<Domain.Models.Match>()), Times.Once);
        }

        [Fact]
        public async Task GetUserMatches_ShouldReturnMatchesWithUsers()
        {
            var matches = new List<Domain.Models.Match>
            {
                new Domain.Models.Match { Id = 1, User1Id = 1, User2Id = 2 },
                new Domain.Models.Match { Id = 2, User1Id = 1, User2Id = 3 }
            };

            _matchRepositoryMock.Setup(x => x.GetMatchesByUserId(1)).ReturnsAsync(matches);

            _userRepositoryMock.Setup(x => x.GetUserById(It.IsAny<int>()))
                .ReturnsAsync((int id) => new User { Id = id });

            var result = await _service.GetUserMatches(1);

            Assert.Equal(2, result.Count());
            Assert.All(result, match =>
            {
                Assert.NotNull(match.User1);
                Assert.NotNull(match.User2);
            });
        }
    }
}
