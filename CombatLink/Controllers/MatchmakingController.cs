using CombatLink.Application.Services;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CombatLink.Web.Controllers
{
    [Authorize]
    [Route("Matchmaking")]
    public class MatchmakingController : Controller
    {
        private readonly ILikeService _likeService;
        private readonly IMatchService _matchService;


        public MatchmakingController(ILikeService likeService, IMatchService matchService)
        {
            _likeService = likeService;
            _matchService = matchService;
        }

        [HttpPost("Like")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Like([FromQuery] int userId)
        {
            Debug.WriteLine("LIKE action triggered with userId: " + userId);
            int likerUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await _likeService.AddLikeAsync(likerUserId, userId))
            {
                if (await _likeService.HasUserLikedAsync(userId, likerUserId))
                {
                    await _matchService.CreateMatchIfNotExists(likerUserId, userId);

                    return Ok("MATCH!");
                }

                return Ok("User liked successfully.");
            }
            else
            {
                return BadRequest("Not liked");
            }
        }


        [HttpGet("GetLikes")]
        public async Task<IActionResult> GetLikes([FromQuery] int userId)
        {
            var likes = await _likeService.GetLikesByUserIdAsync(userId);
            return View(likes);
        }

        [HttpGet("GetMatches")]
        public async Task<IActionResult> GetMatches([FromQuery] int userId)
        {
            var likesReceived = await _likeService.GetLikesByUserIdAsync(userId);
            var mutualLikes = new List<Like>();

            foreach (var like in likesReceived)
            {
                if (await _likeService.HasUserLikedAsync(userId, like.LikerUserId))
                {
                    mutualLikes.Add(like);
                }
            }

            return View(mutualLikes);
        }
    }
}
