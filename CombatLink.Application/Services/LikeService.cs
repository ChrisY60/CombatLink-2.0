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
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;

        public LikeService(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public async Task<bool> AddLikeAsync(int likerUserId, int likedUserId)
        {
            if (await _likeRepository.HasUserLiked(likerUserId, likedUserId))
            {
                return false;
            }

            var like = new Like
            {
                LikerUserId = likerUserId,
                LikedUserId = likedUserId,
                TimeOfLike = DateTime.UtcNow
            };

            return await _likeRepository.AddLike(like);
        }

        public async Task<IEnumerable<Like>> GetLikesByUserIdAsync(int likedUserId)
        {
            return await _likeRepository.GetLikesByUserId(likedUserId);
        }

        public async Task<bool> HasUserLikedAsync(int likerUserId, int likedUserId)
        {
            return await _likeRepository.HasUserLiked(likerUserId, likedUserId);
        }
    }
}
