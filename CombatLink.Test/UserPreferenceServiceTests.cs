using Xunit;
using Moq;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using System.Threading.Tasks;
using CombatLink.Application.Services;
using CombatLink.Domain.IServices;

namespace CombatLink.Tests.Services
{
    public class UserPreferenceServiceTests
    {
        private readonly Mock<IUserPreferenceRepository> _preferenceRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserPreferenceService _service;

        public UserPreferenceServiceTests()
        {
            _preferenceRepoMock = new Mock<IUserPreferenceRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _service = new UserPreferenceService(_preferenceRepoMock.Object, _userRepoMock.Object);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnPreferenceWithUser_WhenExists()
        {
            var preference = new UserPreference { Id = 1, RelatedUser = null };
            var user = new User { Id = 1, FirstName = "John" };

            _preferenceRepoMock.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync(preference);
            _userRepoMock.Setup(x => x.GetUserById(1)).ReturnsAsync(user);

            var result = await _service.GetByUserIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(user, result.RelatedUser);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _preferenceRepoMock.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync((UserPreference?)null);

            var result = await _service.GetByUserIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrUpdateAsync_ShouldAdd_WhenNoExistingPreference()
        {
            var newPref = new UserPreference { RelatedUser = new User { Id = 1 } };

            _preferenceRepoMock.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync((UserPreference?)null);
            _preferenceRepoMock.Setup(x => x.AddAsync(newPref)).ReturnsAsync(true);

            var result = await _service.CreateOrUpdateAsync(newPref);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateOrUpdateAsync_ShouldUpdate_WhenExistingPreferenceExists()
        {
            var existingPref = new UserPreference { Id = 5, RelatedUser = new User { Id = 1 } };
            var newPref = new UserPreference { RelatedUser = new User { Id = 1 } };

            _preferenceRepoMock.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync(existingPref);
            _preferenceRepoMock.Setup(x => x.UpdateAsync(It.Is<UserPreference>(p => p.Id == 5))).ReturnsAsync(true);

            var result = await _service.CreateOrUpdateAsync(newPref);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            _preferenceRepoMock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateOrUpdateAsync_ShouldThrow_WhenWeightMinGreaterThanMax()
        {
            var pref = new UserPreference
            {
                RelatedUser = new User { Id = 1 },
                WeightMin = 100,
                WeightMax = 90
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrUpdateAsync(pref));
        }

        [Fact]
        public async Task CreateOrUpdateAsync_ShouldThrow_WhenHeightMinGreaterThanMax()
        {
            var pref = new UserPreference
            {
                RelatedUser = new User { Id = 1 },
                HeightMin = 200,
                HeightMax = 150
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrUpdateAsync(pref));
        }

        [Fact]
        public async Task CreateOrUpdateAsync_ShouldThrow_WhenExperienceMinGreaterThanMax()
        {
            var pref = new UserPreference
            {
                RelatedUser = new User { Id = 1 },
                ExperienceMin = 50,
                ExperienceMax = 30
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrUpdateAsync(pref));
        }
    }
}
