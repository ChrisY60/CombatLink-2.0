using CombatLink.Repositories.IRepositories;
using CombatLink.Services.IServices;
using CombatLinkMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace CombatLink.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUserAsync(string email, string passwordHash)
        {
            return await _userRepository.RegisterUserAsync(email, passwordHash);
        }

        public async Task<int?> LogInUserAsync(string email, string passwordHash)
        {
            return await _userRepository.LogInUserAsync(email, passwordHash);
        }
        public async Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience)
        {
            Debug.WriteLine("Got here 2");
            return await _userRepository.UpdateUserProfile(userId, firstName, lastName, dateOfBirth, weight, height, monthsOfExperience);
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _userRepository.GetUserById(userId);
        }



    }
}
