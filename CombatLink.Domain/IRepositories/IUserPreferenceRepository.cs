using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IRepositories
{
    public interface IUserPreferenceRepository
    {
        Task<UserPreference?> GetByUserIdAsync(int userId);
        Task<UserPreference?> GetByIdAsync(int id);
        Task<bool> AddAsync(UserPreference preference);
        Task<bool> UpdateAsync(UserPreference preference);
        Task<bool> DeleteAsync(int id);

    }
}
