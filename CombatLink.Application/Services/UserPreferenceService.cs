using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;

namespace CombatLink.Application.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly IUserPreferenceRepository _preferenceRepository;
        private readonly IUserRepository _userRepository;

        public UserPreferenceService(IUserPreferenceRepository preferenceRepository, IUserRepository userRepository)
        {
            _preferenceRepository = preferenceRepository;
            _userRepository = userRepository;
        }

        public async Task<UserPreference?> GetByUserIdAsync(int userId)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(userId);
            if (preference != null)
            {
                var user = await _userRepository.GetUserById(userId);
                if (user != null)
                {
                    preference.RelatedUser = user;
                }
            }
            return preference;
        }

        public async Task<bool> CreateOrUpdateAsync(UserPreference preference)
        {
            if (preference.WeightMin.HasValue && preference.WeightMax.HasValue && preference.WeightMin > preference.WeightMax)
            {
                throw new ArgumentException("Minimum weight cannot be greater than maximum weight.");
            }

            if (preference.HeightMin.HasValue && preference.HeightMax.HasValue && preference.HeightMin > preference.HeightMax)
            {
                throw new ArgumentException("Minimum height cannot be greater than maximum height.");
            }

            if (preference.ExperienceMin.HasValue && preference.ExperienceMax.HasValue && preference.ExperienceMin > preference.ExperienceMax)
            {
                throw new ArgumentException("Minimum experience cannot be greater than maximum experience.");
            }


            var existing = await _preferenceRepository.GetByUserIdAsync(preference.RelatedUser.Id);

            if (existing == null)
            {
                return await _preferenceRepository.AddAsync(preference);
            }
            else
            {
                preference.Id = existing.Id;
                return await _preferenceRepository.UpdateAsync(preference);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _preferenceRepository.DeleteAsync(id);
        }
    }
}