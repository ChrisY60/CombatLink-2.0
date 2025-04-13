using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;

namespace CombatLink.Application.Services
{
    public class SportService : ISportService
    {
        private readonly ISportRepository _sportRepository;
        private readonly IUserRepository _userRepository;

        public SportService(ISportRepository sportRepository, IUserRepository userRepository)
        {
            _sportRepository = sportRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> CreateSportAsync(Sport sport)
        {
            return await _sportRepository.CreateSport(sport);
        }

        public async Task<Sport?> GetSportByIdAsync(int sportId)
        {
            return await _sportRepository.GetSportById(sportId);
        }

        public async Task<IEnumerable<Sport>> GetAllSportsAsync()
        {
            return await _sportRepository.GetAllSports();
        }

        public async Task<IEnumerable<Sport>> GetSportsByUserIdAsync(int userId)
        {
            return await _sportRepository.GetSportsByUserId(userId);
        }

        public async Task<IEnumerable<User>> GetUsersBySportIdAsync(int sportId)
        {
            return await _sportRepository.GetUsersBySportId(sportId);
        }

        public async Task<bool> AddSportsToUserAsync(int userId, List<int> newSportIds)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
                return false;

            var currentSports = await _sportRepository.GetSportsByUserId(userId);
            var currentSportIds = currentSports.Select(s => s.Id).ToList();

            var sportsToAdd = newSportIds.Except(currentSportIds).ToList();
            var sportsToRemove = currentSportIds.Except(newSportIds).ToList();

            bool allSucceeded = true;

            foreach (int sportId in sportsToAdd)
            {
                var sport = await _sportRepository.GetSportById(sportId);
                if (sport == null)
                {
                    allSucceeded = false;
                    continue;
                }
                bool success = await _userRepository.AddSportToUser(sport, user);
                if (!success)
                {
                    allSucceeded = false;
                }
            }

            foreach (int sportId in sportsToRemove)
            {
                bool success = await _userRepository.RemoveSportFromUser(userId, sportId);
                if (!success)
                {
                    allSucceeded = false;
                }
            }

            return allSucceeded;
        }


    }

}
