using Xunit;
using Moq;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using CombatLink.Application.Services;

namespace CombatLink.Tests.Services
{
    public class LanguageServiceTests
    {
        private readonly Mock<ILanguageRepository> _languageRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly LanguageService _languageService;

        public LanguageServiceTests()
        {
            _languageRepoMock = new Mock<ILanguageRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _languageService = new LanguageService(_languageRepoMock.Object, _userRepoMock.Object);
        }

        [Fact]
        public async Task CreateLanguageAsync_ShouldReturnTrue_WhenLanguageCreated()
        {
            var language = new Language { Id = 1, Name = "English" };
            _languageRepoMock.Setup(x => x.CreateLanguage(language)).ReturnsAsync(true);

            var result = await _languageService.CreateLanguageAsync(language);

            Assert.True(result);
        }

        [Fact]
        public async Task GetLanguageByIdAsync_ShouldReturnLanguage_WhenExists()
        {
            var language = new Language { Id = 1, Name = "English" };
            _languageRepoMock.Setup(x => x.GetLanguageById(1)).ReturnsAsync(language);

            var result = await _languageService.GetLanguageByIdAsync(1);

            Assert.Equal(language, result);
        }

        [Fact]
        public async Task GetAllLanguagesAsync_ShouldReturnLanguages()
        {
            var languages = new List<Language> { new Language { Id = 1, Name = "English" } };
            _languageRepoMock.Setup(x => x.GetAllLanguages()).ReturnsAsync(languages);

            var result = await _languageService.GetAllLanguagesAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetLanguagesByUserIdAsync_ShouldReturnLanguages()
        {
            var languages = new List<Language> { new Language { Id = 1, Name = "English" } };
            _languageRepoMock.Setup(x => x.GetLanguagesByUserId(5)).ReturnsAsync(languages);

            var result = await _languageService.GetLanguagesByUserIdAsync(5);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetUsersByLanguageIdAsync_ShouldReturnUsers()
        {
            var users = new List<User> { new User { Id = 1, FirstName = "John", LastName = "Doe" } };
            _languageRepoMock.Setup(x => x.GetUsersByLanguageId(1)).ReturnsAsync(users);

            var result = await _languageService.GetUsersByLanguageIdAsync(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task AddLanguagesToUserAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            _userRepoMock.Setup(x => x.GetUserById(10)).ReturnsAsync((User?)null);

            var result = await _languageService.AddLanguagesToUserAsync(10, new List<int> { 1, 2 });

            Assert.False(result);
        }

        [Fact]
        public async Task AddLanguagesToUserAsync_ShouldAddAndRemoveLanguagesCorrectly()
        {
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe" };
            var currentLanguages = new List<Language> { new Language { Id = 1, Name = "English" } };

            _userRepoMock.Setup(x => x.GetUserById(1)).ReturnsAsync(user);
            _languageRepoMock.Setup(x => x.GetLanguagesByUserId(1)).ReturnsAsync(currentLanguages);
            _languageRepoMock.Setup(x => x.GetLanguageById(2)).ReturnsAsync(new Language { Id = 2, Name = "French" });
            _userRepoMock.Setup(x => x.AddLanguageToUser(It.IsAny<Language>(), user)).ReturnsAsync(true);
            _userRepoMock.Setup(x => x.RemoveLanguageFromUser(1, 1)).ReturnsAsync(true);

            var result = await _languageService.AddLanguagesToUserAsync(1, new List<int> { 2 });

            Assert.True(result);
        }
    }
}
