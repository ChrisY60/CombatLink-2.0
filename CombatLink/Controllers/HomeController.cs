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
        private readonly ISparringSessionProposalService _sparringSessionProposalService;

        public HomeController(ILogger<HomeController> logger, IMatchmakingService matchmakingService, ILikeService likeService, IMatchService matchService, IUserService userService, IChatMessageService chatMessageService, ISparringSessionProposalService sparringSessionProposalService)
        {
            _logger = logger;
            _matchmakingService = matchmakingService;
            _likeService = likeService;
            _matchService = matchService;
            _userService = userService;
            _chatMessageService = chatMessageService;
            _sparringSessionProposalService = sparringSessionProposalService;
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

        public async Task<IActionResult> ChatAsync(int matchId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var match = await _matchService.GetMatchById(matchId);
            if (match == null) return NotFound();

            if (match.User1Id != userId && match.User2Id != userId)
                return StatusCode(403, "You are not authorized to access this chat.");

            int otherUserId = match.User1Id == userId ? match.User2Id : match.User1Id;

            var messages = (await _chatMessageService.GetMessagesForMatchId(matchId)).ToList();
            var proposals = (await _sparringSessionProposalService.GetByTwoUserIdsAsync(userId, otherUserId)).ToList();

            var chatItems = new List<ChatItem>();

            chatItems.AddRange(messages.Select(m => new ChatItem{Message = m,Time = m.TimeSent}));
            chatItems.AddRange(proposals.Select(p => new ChatItem{Proposal = p,Time = p.TimeProposed}));
            chatItems = chatItems.OrderBy(i => i.Time).ToList();

            var model = new ChatViewModel
            {
                MatchId = matchId,
                CurrentUserId = userId,
                OtherUserId = otherUserId,
                ChatItems = chatItems,
                OtherUserObj = await _userService.GetUserById(otherUserId)
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
