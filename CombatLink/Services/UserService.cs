using CombatLink.Repositories.IRepositories;
using CombatLink.Services.IServices;
using CombatLinkMVC.Models;

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
    }
}
