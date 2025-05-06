using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IRepositories
{
    public interface IMatchRepository
    {
        Task<bool> AddMatch(Match match);
        Task<IEnumerable<Match>> GetMatchesByUserId(int userId);
        Task<bool> MatchExists(int user1Id, int user2Id);
    }
}
