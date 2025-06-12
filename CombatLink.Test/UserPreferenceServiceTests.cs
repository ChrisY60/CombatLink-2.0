using Xunit;
using Moq;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using System.Threading.Tasks;
using CombatLink.Application.Services;
using CombatLink.Domain.IServices;
using System;

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
            var user = new User { Id = 1, FirstName = "John" };
            var preference = new UserPreference { Id = 1, RelatedUser = user };

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
            var newPref = new UserPreference
            {
                RelatedUser = new User { Id = 1 },
                WeightMin = 70,
                WeightMax = 90,
                HeightMin = 160,
                HeightMax = 190
            };

            UserPreference? captured = null;

            _preferenceRepoMock.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync((UserPreference?)null);
            _preferenceRepoMock.Setup(x => x.AddAsync(It.IsAny<UserPreference>()))
                .Callback<UserPreference>(p => captured = p)
                .ReturnsAsync(true);

            var result = await _service.CreateOrUpdateAsync(newPref);

            Assert.True(result);
            Assert.NotNull(captured);
            Assert.Equal(newPref.RelatedUser.Id, captured.RelatedUser.Id);
            Assert.Equal(newPref.WeightMin, captured.WeightMin);
            Assert.Equal(newPref.WeightMax, captured.WeightMax);
            Assert.Equal(newPref.HeightMin, captured.HeightMin);
            Assert.Equal(newPref.HeightMax, captured.HeightMax);
        }

        [Fact]
        public async Task CreateOrUpdateAsync_ShouldUpdate_WhenExistingPreferenceExists()
        {
            var existingPref = new UserPreference
            {
                Id = 5,
                RelatedUser = new User { Id = 1 }
            };

            var newPref = new UserPreference
            {
                RelatedUser = new User { Id = 1 },
                WeightMin = 60,
                WeightMax = 85
            };

            UserPreference? captured = null;

            _preferenceRepoMock.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync(existingPref);
            _preferenceRepoMock.Setup(x => x.UpdateAsync(It.IsAny<UserPreference>()))
                .Callback<UserPreference>(p => captured = p)
                .ReturnsAsync(true);

            var result = await _service.CreateOrUpdateAsync(newPref);

            Assert.True(result);
            Assert.NotNull(captured);
            Assert.Equal(existingPref.Id, captured.Id);
            Assert.Equal(existingPref.RelatedUser.Id, captured.RelatedUser.Id);
            Assert.Equal(60, captured.WeightMin);
            Assert.Equal(85, captured.WeightMax);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_AndVerifyId()
        {
            int? deletedId = null;
            _preferenceRepoMock.Setup(x => x.DeleteAsync(It.IsAny<int>()))
                .Callback<int>(id => deletedId = id)
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
            Assert.Equal(1, deletedId);
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
