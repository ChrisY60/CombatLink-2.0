using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IRepositories
{
    public interface ILikeRepository
    {
        Task<bool> AddLike(Like like);
        Task<IEnumerable<Like>> GetLikesByUserId(int likedUserId);
        Task<bool> HasUserLiked(int likerUserId, int likedUserId);
    }
}
