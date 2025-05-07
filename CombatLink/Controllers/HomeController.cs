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

        public HomeController(ILogger<HomeController> logger, IMatchmakingService matchmakingService, ILikeService likeService, IMatchService matchService, IUserService userService)
        {
            _logger = logger;
            _matchmakingService = matchmakingService;
            _likeService = likeService;
            _matchService = matchService;
            _userService = userService;
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

        public async Task<IActionResult> MatchesAndChats()
        {
            // Mock matched users
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<User> matchedUsers = new List<User>();
            List<Match> matches = (List<Match>)await _matchService.GetUserMatches(userId);
            if(matches.Any()){
                foreach (Match match in matches)
                {
                    if (match.User1Id == userId) {
                        User? userToAdd = await _userService.GetUserById(match.User2Id);
                        if(userToAdd != null)
                        {
                            matchedUsers.Add(userToAdd);
                        }
                    }
                    else
                    {
                        User? userToAdd = await _userService.GetUserById(match.User1Id);
                        if (userToAdd != null)
                        {
                            matchedUsers.Add(userToAdd);
                        }
                    }
                }
            }
            
                
            // Mock chats
            var chats = new List<ChatSummary>
            {
                new ChatSummary
                {
                    User = new User { Id = 1, FirstName = "Fighter", LastName = "One", ProfilePictureURL = "" },
                    LastMessage = "Hello, I will be able to train later today.",
                    UnreadCount = 3
                },
                new ChatSummary
                {
                    User = new User { Id = 2, FirstName = "Fighter", LastName = "Two", ProfilePictureURL = "" },
                    LastMessage = "See you tomorrow in the gym!",
                    UnreadCount = 0
                }
            };

            var viewModel = new MatchChatViewModel
            {
                MatchedUsers = matchedUsers,
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
