using CombatLinkMVC.Models;

namespace CombatLink.Services.IServices
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string email, string password);

        Task<int?> LogInUserAsync(string email, string password);

        Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience);
    
        Task<User?> GetUserById(int userId);
    }

}
