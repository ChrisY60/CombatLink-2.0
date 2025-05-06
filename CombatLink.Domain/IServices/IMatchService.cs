using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IServices
{
    public interface IMatchService
    {
        Task<bool> CreateMatchIfNotExists(int user1Id, int user2Id);
        Task<IEnumerable<Match>> GetUserMatches(int userId);
    }
}
