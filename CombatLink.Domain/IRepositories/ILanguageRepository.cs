using CombatLink.Domain.Models;

namespace CombatLink.Domain.IRepositories
{
    public interface ILanguageRepository
    {
        public Task<bool> CreateLanguage(Language language);
        public Task<Language> GetLanguageById(int languageId);
        public Task<IEnumerable<Language>> GetAllLanguages();
        public Task<IEnumerable<Language>> GetLanguagesByUserId(int userId);
        public Task<IEnumerable<User>> GetUsersByLanguageId(int languageId);
    }
}
