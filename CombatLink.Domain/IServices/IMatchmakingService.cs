using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IServices
{
    public interface IMatchmakingService
    {
        Task<List<User>> GetRecommendedUsersForUserIdAsync(int userId);
    }
}
