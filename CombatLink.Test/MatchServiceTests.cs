using Xunit;
using Moq;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using CombatLink.Application.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

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
        public async Task CreateMatchIfNotExists_ShouldCreateAndVerifyMatch_WhenMatchDoesNotExist()
        {
            _matchRepositoryMock.Setup(x => x.MatchExists(1, 2)).ReturnsAsync(false);

            Domain.Models.Match? capturedMatch = null;

            _matchRepositoryMock.Setup(x => x.AddMatch(It.IsAny<Domain.Models.Match>()))
                .Callback<Domain.Models.Match>(m => capturedMatch = m)
                .ReturnsAsync(true);

            var result = await _service.CreateMatchIfNotExists(1, 2);

            Assert.True(result);
            Assert.NotNull(capturedMatch);
            Assert.Equal(1, capturedMatch!.User1Id);
            Assert.Equal(2, capturedMatch.User2Id);
        }

        [Fact]
        public async Task GetUserMatches_ShouldReturnMatchesWithCorrectUsers()
        {
            var match1 = new Domain.Models.Match { Id = 1, User1Id = 1, User2Id = 2 };
            var match2 = new Domain.Models.Match { Id = 2, User1Id = 1, User2Id = 3 };
            var match3 = new Domain.Models.Match { Id = 3, User1Id = 4, User2Id = 1 };

            var matches = new List<Domain.Models.Match> { match1, match2, match3 };

            var user1 = new User { Id = 1, FirstName = "User1" };
            var user2 = new User { Id = 2, FirstName = "User2" };
            var user3 = new User { Id = 3, FirstName = "User3" };
            var user4 = new User { Id = 4, FirstName = "User4" };

            _matchRepositoryMock.Setup(x => x.GetMatchesByUserId(1)).ReturnsAsync(matches);

            _userRepositoryMock.Setup(x => x.GetUserById(1)).ReturnsAsync(user1);
            _userRepositoryMock.Setup(x => x.GetUserById(2)).ReturnsAsync(user2);
            _userRepositoryMock.Setup(x => x.GetUserById(3)).ReturnsAsync(user3);
            _userRepositoryMock.Setup(x => x.GetUserById(4)).ReturnsAsync(user4);

            var result = (await _service.GetUserMatches(1)).ToList();

            Assert.Equal(3, result.Count);

            Assert.NotNull(result[0]);
            Assert.NotNull(result[0].User1);
            Assert.NotNull(result[0].User2);
            Assert.Equal("User1", result[0].User1.FirstName);
            Assert.Equal("User2", result[0].User2.FirstName);

            Assert.NotNull(result[1]);
            Assert.NotNull(result[1].User1);
            Assert.NotNull(result[1].User2);
            Assert.Equal("User1", result[1].User1.FirstName);
            Assert.Equal("User3", result[1].User2.FirstName);

            Assert.NotNull(result[2]);
            Assert.NotNull(result[2].User1);
            Assert.NotNull(result[2].User2);
            Assert.Equal("User4", result[2].User1.FirstName);
            Assert.Equal("User1", result[2].User2.FirstName);
        }
    }
}
