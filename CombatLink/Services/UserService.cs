﻿using CombatLink.Repositories.IRepositories;
using CombatLink.Services.IServices;
using CombatLinkMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace CombatLink.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        IPasswordHasher<Object> _passwordHasher;
        public UserService(IUserRepository userRepository, IPasswordHasher<Object> passwordHasher)
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
                if(userId != null)
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
