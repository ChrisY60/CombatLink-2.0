using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.Services
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private readonly ISportRepository _sportRepository;
        private readonly ILanguageRepository _languageRepository;

        public MatchmakingService(IUserRepository userRepository, IUserPreferenceRepository userPreferenceRepository, ISportRepository sportRepository, ILanguageRepository languageRepository)
        {
            _userRepository = userRepository;
            _userPreferenceRepository = userPreferenceRepository;
            _sportRepository = sportRepository;
            _languageRepository = languageRepository;
        }

        public async Task<List<User>> GetRecommendedUsersForUserIdAsync(int userId)
        {
            List<User> recommendations = new List<User>();
            User currentUser = await _userRepository.GetUserById(userId);

            
            if (currentUser == null) { 
                return recommendations;
            }

            UserPreference? preference = await _userPreferenceRepository.GetByUserIdAsync(userId);
            currentUser.Sports = (await _sportRepository.GetSportsByUserId(currentUser.Id)).ToList();
            currentUser.Languages = (await _languageRepository.GetLanguagesByUserId(currentUser.Id)).ToList();

            List<User> allUsers = (List<User>)await _userRepository.GetAllUsersAsync();

            foreach (User user in allUsers)
            {
                if(user.Id == userId)
                {
                    continue;
                }
                user.Sports = (await _sportRepository.GetSportsByUserId(user.Id)).ToList();
                user.Languages = (await _languageRepository.GetLanguagesByUserId(user.Id)).ToList();

                var userSportIds = user.Sports.Select(s => s.Id);
                var currentUserSportIds = currentUser.Sports.Select(s => s.Id);
                bool hasMutualSport = userSportIds.Intersect(currentUserSportIds).Any();

                var userLanguageIds = user.Languages.Select(l => l.Id);
                var currentUserLanguageIds = currentUser.Languages.Select(l => l.Id);
                bool hasMutualLanguage = userLanguageIds.Intersect(currentUserLanguageIds).Any();

                if(!hasMutualLanguage || !hasMutualSport)
                {
                    continue;
                }
                if(preference != null)
                {
                    if (preference.WeightMin.HasValue && user.Weight < preference.WeightMin.Value)
                        continue;
                    if (preference.WeightMax.HasValue && user.Weight > preference.WeightMax.Value)
                        continue;
                    if (preference.HeightMin.HasValue && user.Height < preference.HeightMin.Value)
                        continue;
                    if (preference.HeightMax.HasValue && user.Height > preference.HeightMax.Value)
                        continue;
                    if (preference.ExperienceMin.HasValue && user.MonthsOfExperience < preference.ExperienceMin.Value)
                        continue;
                    if (preference.ExperienceMax.HasValue && user.MonthsOfExperience > preference.ExperienceMax.Value)
                        continue;
                }

                recommendations.Add(user);
            }
            
            return recommendations;
        }
    }
}
