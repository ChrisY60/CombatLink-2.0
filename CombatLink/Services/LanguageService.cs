using CombatLink.Repositories.IRepositories;
using CombatLink.Services.IServices;
using CombatLinkMVC.Models;

namespace CombatLink.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IUserRepository _userRepository;

        public LanguageService(ILanguageRepository languageRepository, IUserRepository userRepository)
        {
            _languageRepository = languageRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> CreateLanguageAsync(Language language)
        {
            return await _languageRepository.CreateLanguage(language);
        }

        public async Task<Language?> GetLanguageByIdAsync(int languageId)
        {
            return await _languageRepository.GetLanguageById(languageId);
        }

        public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
        {
            return await _languageRepository.GetAllLanguages();
        }

        public async Task<IEnumerable<Language>> GetLanguagesByUserIdAsync(int userId)
        {
            return await _languageRepository.GetLanguagesByUserId(userId);
        }

        public async Task<IEnumerable<User>> GetUsersByLanguageIdAsync(int languageId)
        {
            return await _languageRepository.GetUsersByLanguageId(languageId);
        }

        public async Task<bool> AddLanguagesToUserAsync(int userId, List<int> newLanguageIds)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
                return false;

            var currentLanguages = await _languageRepository.GetLanguagesByUserId(userId);
            var currentLanguageIds = currentLanguages.Select(l => l.Id).ToList();

            var languagesToAdd = newLanguageIds.Except(currentLanguageIds).ToList();
            var languagesToRemove = currentLanguageIds.Except(newLanguageIds).ToList();

            bool allSucceeded = true;

            foreach (int languageId in languagesToAdd)
            {
                var language = await _languageRepository.GetLanguageById(languageId);
                if (language == null)
                {
                    allSucceeded = false;
                    continue;
                }

                bool success = await _userRepository.AddLanguageToUser(language, user);
                if (!success)
                {
                    allSucceeded = false;
                }
            }

            foreach (int languageId in languagesToRemove)
            {
                bool success = await _userRepository.RemoveLanguageFromUser(userId, languageId);
                if (!success)
                {
                    allSucceeded = false;
                }
            }

            return allSucceeded;
        }



    }
}
