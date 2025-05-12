using CombatLink.Application.Services;
using CombatLink.Application.ViewModels;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CombatLink.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMatchmakingService _matchmakingService;
        private readonly ILikeService _likeService;
        private readonly IMatchService _matchService;
        private readonly IUserService _userService;
        private readonly IChatMessageService _chatMessageService;

        public HomeController(ILogger<HomeController> logger, IMatchmakingService matchmakingService, ILikeService likeService, IMatchService matchService, IUserService userService, IChatMessageService chatMessageService)
        {
            _logger = logger;
            _matchmakingService = matchmakingService;
            _likeService = likeService;
            _matchService = matchService;
            _userService = userService;
            _chatMessageService = chatMessageService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<User> users = await _matchmakingService.GetRecommendedUsersForUserIdAsync(userId);
            return View(new IndexViewModel
            {
                Users = users
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Chat(int matchId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var model = new ChatViewModel
            {
                MatchId = matchId,
                UserId = userId
            };

            return View(model);
        }


        public async Task<IActionResult> MatchesAndChats()
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var matches = (await _matchService.GetUserMatches(userId)).ToList();

            var chats = (await _chatMessageService.GetChatSummariesForUserAsync(userId)).ToList();

            foreach (var chat in chats)
            {
                if (chat.User != null && chat.User.FirstName == null)
                {
                    var fullUser = await _userService.GetUserById(chat.User.Id);
                    if (fullUser != null)
                        chat.User = fullUser;
                }
            }

            var viewModel = new MatchChatViewModel
            {
                Matches = matches,
                Chats = chats
            };

            return View(viewModel);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
