using CombatLinkMVC.Models;

namespace CombatLink.Repositories.IRepositories
{
    public interface IUserRepository
    {
        public Task<bool> RegisterUserAsync(string email, string passwordHash);

        public Task<int?> LogInUserAsync(string email, string passwordHash);

        public Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience);
    
        public Task<User?> GetUserById(int userId);
    }
}
