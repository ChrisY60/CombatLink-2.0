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

        public HomeController(ILogger<HomeController> logger, IMatchmakingService matchmakingService)
        {
            _logger = logger;
            _matchmakingService = matchmakingService;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
