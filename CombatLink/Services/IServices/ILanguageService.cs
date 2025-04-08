using CombatLinkMVC.Models;

namespace CombatLink.Services.IServices
{
    public interface ILanguageService
    {
        Task<bool> CreateLanguageAsync(Language language);
        Task<Language?> GetLanguageByIdAsync(int languageId);
        Task<IEnumerable<Language>> GetAllLanguagesAsync();
        Task<IEnumerable<Language>> GetLanguagesByUserIdAsync(int userId);
        Task<IEnumerable<User>> GetUsersByLanguageIdAsync(int languageId);
        Task<bool> AddLanguagesToUserAsync(int userId, List<int> languagesIds);
    }
}
