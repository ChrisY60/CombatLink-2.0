using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IServices
{
    public interface ILikeService
    {
        Task<bool> AddLikeAsync(int likerUserId, int likedUserId);
        Task<IEnumerable<Like>> GetLikesByUserIdAsync(int likedUserId);
        Task<bool> HasUserLikedAsync(int likerUserId, int likedUserId);
    }
}
