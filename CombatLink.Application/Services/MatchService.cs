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

        public MatchService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
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
            return await _matchRepository.GetMatchesByUserId(userId);
        }
    }
}
