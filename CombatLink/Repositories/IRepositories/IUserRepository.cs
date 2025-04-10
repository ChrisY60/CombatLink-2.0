using CombatLinkMVC.Models;

namespace CombatLink.Repositories.IRepositories
{
    public interface IUserRepository
    {
        public Task<bool> RegisterUserAsync(string email, string passwordHash);

        public Task<string?> GetPasswordHashByEmail(string email);

        public Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience);
    
        public Task<User?> GetUserById(int userId);

        public Task<int?> GetUserIdByEmail(string email);

        public Task<bool> AddSportToUser(Sport sport, User user);
        public Task<bool> RemoveSportFromUser(int userId, int sportId);
        public Task<bool> AddLanguageToUser(Language language, User user);

        public Task<bool> RemoveLanguageFromUser(int userId, int languageId);

    }
}
