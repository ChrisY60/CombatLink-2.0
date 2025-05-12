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
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IUserRepository _userRepository;

        public MatchService(IMatchRepository matchRepository, IUserRepository userRepository)
        {
            _matchRepository = matchRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> CreateMatchIfNotExists(int user1Id, int user2Id)
        {
            if (await _matchRepository.MatchExists(user1Id, user2Id))
                return false;

            var match = new Match
            {
                User1Id = user1Id,
                User2Id = user2Id,
                TimeOfMatch = DateTime.UtcNow
            };

            return await _matchRepository.AddMatch(match);
        }

        public async Task<IEnumerable<Match>> GetUserMatches(int userId)
        {
            var matches = (await _matchRepository.GetMatchesByUserId(userId)).ToList();

            foreach (var match in matches)
            {
                match.User1 = await _userRepository.GetUserById(match.User1Id);
                match.User2 = await _userRepository.GetUserById(match.User2Id);
            }

            return matches;
        }
    }

}
