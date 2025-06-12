using Xunit;
using Moq;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using CombatLink.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task CreateLanguageAsync_ShouldReturnTrue_AndCaptureLanguage()
        {
            var language = new Language { Id = 1, Name = "English" };
            Language? captured = null;

            _languageRepoMock
                .Setup(x => x.CreateLanguage(It.IsAny<Language>()))
                .ReturnsAsync(true)
                .Callback<Language>(l => captured = l);

            var result = await _languageService.CreateLanguageAsync(language);

            Assert.True(result);
            Assert.NotNull(captured);
            Assert.Equal(1, captured.Id);
            Assert.Equal("English", captured.Name);
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
        public async Task GetAllLanguagesAsync_ShouldReturnMultipleLanguages()
        {
            var languages = new List<Language>
            {
                new Language { Id = 1, Name = "English" },
                new Language { Id = 2, Name = "French" },
                new Language { Id = 3, Name = "Spanish" }
            };

            _languageRepoMock.Setup(x => x.GetAllLanguages()).ReturnsAsync(languages);

            var result = (await _languageService.GetAllLanguagesAsync()).ToList();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, l => l.Name == "English" && l.Id == 1);
            Assert.Contains(result, l => l.Name == "French" && l.Id == 2);
            Assert.Contains(result, l => l.Name == "Spanish" && l.Id == 3);
        }

        [Fact]
        public async Task GetLanguagesByUserIdAsync_ShouldReturnMultipleLanguages()
        {
            var userLanguages = new List<Language>
            {
                new Language { Id = 1, Name = "German" },
                new Language { Id = 2, Name = "Dutch" }
            };

            _languageRepoMock.Setup(x => x.GetLanguagesByUserId(5)).ReturnsAsync(userLanguages);

            var result = (await _languageService.GetLanguagesByUserIdAsync(5)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, l => l.Name == "German" && l.Id == 1);
            Assert.Contains(result, l => l.Name == "Dutch" && l.Id == 2);
        }

        [Fact]
        public async Task GetUsersByLanguageIdAsync_ShouldReturnMultipleUsers()
        {
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe" },
                new User { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _languageRepoMock.Setup(x => x.GetUsersByLanguageId(1)).ReturnsAsync(users);

            var result = (await _languageService.GetUsersByLanguageIdAsync(1)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Id == 1 && u.FirstName == "John" && u.LastName == "Doe");
            Assert.Contains(result, u => u.Id == 2 && u.FirstName == "Jane" && u.LastName == "Smith");
        }


        [Fact]
        public async Task AddLanguagesToUserAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            _userRepoMock.Setup(x => x.GetUserById(10)).ReturnsAsync((User?)null);

            var result = await _languageService.AddLanguagesToUserAsync(10, new List<int> { 1, 2 });

            Assert.False(result);
        }

        [Fact]
        // this one might seem weird but i replace languages so that's why they are in the same test (so I can test the entire method)
        public async Task AddLanguagesToUserAsync_ShouldAddAndRemoveLanguagesCorrectly_AndCaptureObjects()
        {
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe" };
            var currentLanguages = new List<Language> { new Language { Id = 1, Name = "English" } };
            var newLanguage = new Language { Id = 2, Name = "French" };

            Language? addedLanguage = null;
            int? removedLanguageId = null;

            _userRepoMock.Setup(x => x.GetUserById(1)).ReturnsAsync(user);
            _languageRepoMock.Setup(x => x.GetLanguagesByUserId(1)).ReturnsAsync(currentLanguages);
            _languageRepoMock.Setup(x => x.GetLanguageById(2)).ReturnsAsync(newLanguage);

            _userRepoMock
                .Setup(x => x.AddLanguageToUser(It.IsAny<Language>(), user))
                .Callback<Language, User>((lang, usr) => addedLanguage = lang)
                .ReturnsAsync(true);

            _userRepoMock
                .Setup(x => x.RemoveLanguageFromUser(1, It.IsAny<int>()))
                .Callback<int, int>((userId, langId) => removedLanguageId = langId)
                .ReturnsAsync(true);

            var result = await _languageService.AddLanguagesToUserAsync(1, new List<int> { 2 });

            Assert.True(result);
            Assert.NotNull(addedLanguage);
            Assert.Equal(2, addedLanguage!.Id);
            Assert.Equal("French", addedLanguage.Name);

            Assert.NotNull(removedLanguageId);
            Assert.Equal(1, removedLanguageId);
        }
    }
}
