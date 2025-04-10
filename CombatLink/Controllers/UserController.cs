using CombatLink.Repositories.IRepositories;
using CombatLink.Services.IServices;
using CombatLink.ViewModels;
using CombatLinkMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace CombatLink.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISportService _sportsService;
        private readonly ILanguageService _languageService;

        public UserController(IUserService userService, ISportService sportsService, ILanguageService languageService)
        {
            _sportsService = sportsService;
            _userService = userService;
            _languageService = languageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var isRegistered = await _userService.RegisterUserAsync(model.Email, model.Password);

            if (isRegistered)
            {
                return RedirectToAction("LogIn", "User");
            }else
            {
                return View();
            }

        }
        [AllowAnonymous]
        public IActionResult LogIn()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string passwordHash = model.Password;
            int? id = (int?)await _userService.LogInUserAsync(model.Email, passwordHash);
            if (id.HasValue)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Role, "User")
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authenticationProperties
                );

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Incorrect credentials");
                return View(model);
            }

        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ManageProfile()
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User profile not found.");
            }

            List<Sport> allSports = (List<Sport>)await _sportsService.GetAllSportsAsync();
            List<Language> allLanguages = (List<Language>)await _languageService.GetAllLanguagesAsync();
            List<Language> selectedLanguages = (List<Language>)await _languageService.GetLanguagesByUserIdAsync(userId);
            List<Sport> selectedSports = (List<Sport>)await _sportsService.GetSportsByUserIdAsync(userId);

            UserProfileViewModel model = new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Weight = user.Weight,
                Height = user.Height,
                MonthsOfExperience = user.MonthsOfExperience,
                AvailableSports = allSports,
                AvailableLanguages = allLanguages,
                SelectedSportIds = selectedSports.Select(s => s.Id).ToList(),
                SelectedLanguageIds = selectedLanguages.Select(l => l.Id).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ManageProfile(UpdateUserProfileManagementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            bool isUpdated = await _userService.UpdateUserProfile(
                userId,
                model.FirstName,
                model.LastName,
                model.DateOfBirth,
                model.Weight,
                model.Height,
                model.MonthsOfExperience
            );

            bool sportsAdded = await _sportsService.AddSportsToUserAsync(userId, model.SelectedSportIds);
            bool languagesAdded = await _languageService.AddLanguagesToUserAsync(userId, model.SelectedLanguageIds);

            if (isUpdated && sportsAdded && languagesAdded)
            {
                ViewBag.Message = "Profile updated successfully!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Failed to update profile completely.");
            return View(model);

        }
    }
}
