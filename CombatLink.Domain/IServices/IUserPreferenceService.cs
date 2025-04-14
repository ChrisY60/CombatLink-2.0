using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IServices
{
    public interface IUserPreferenceService
    {
        Task<UserPreference?> GetByUserIdAsync(int userId);
        Task<bool> CreateOrUpdateAsync(UserPreference preference);
        Task<bool> DeleteAsync(int id);
    }

}
