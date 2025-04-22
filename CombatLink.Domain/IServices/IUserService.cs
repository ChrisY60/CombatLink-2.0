using CombatLink.Domain.Models;

namespace CombatLink.Domain.IServices
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string email, string password);

        Task<int?> LogInUserAsync(string email, string password);

        Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience, string? profilePictureUrl = null);

        Task<User?> GetUserById(int userId);
    }

}
