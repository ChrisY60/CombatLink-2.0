using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Security.Claims;

namespace CombatLink.Application.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        IPasswordHasher<object> _passwordHasher;
        public UserService(IUserRepository userRepository, IPasswordHasher<object> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> RegisterUserAsync(string email, string password)
        {
            string hashedPassword = _passwordHasher.HashPassword(null, password);
            return await _userRepository.RegisterUserAsync(email, hashedPassword);
        }

        public async Task<int?> LogInUserAsync(string email, string password)
        {
            string? hashedPassword = await _userRepository.GetPasswordHashByEmail(email);
            bool result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
            if (result)
            {
                int? userId = await _userRepository.GetUserIdByEmail(email);
                if (userId != null)
                {
                    return userId;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        public async Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience, string? profilePictureUrl = null)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > 50)
                throw new ArgumentException("First name is required and cannot exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > 50)
                throw new ArgumentException("Last name is required and cannot exceed 50 characters.");

            if (dateOfBirth > DateTime.UtcNow)
                throw new ArgumentException("Date of birth cannot be in the future.");

            if (weight < 30 || weight > 200)
                throw new ArgumentException("Weight must be between 30 and 200 kg.");

            if (height < 100 || height > 250)
                throw new ArgumentException("Height must be between 100 and 250 cm.");

            if (monthsOfExperience < 0 || monthsOfExperience > 1000)
                throw new ArgumentException("Experience must be between 0 and 1000 months.");

            return await _userRepository.UpdateUserProfile(userId, firstName, lastName, dateOfBirth, weight, height, monthsOfExperience, profilePictureUrl);
        }


        public async Task<User?> GetUserById(int userId)
        {
            return await _userRepository.GetUserById(userId);
        }



    }
}
