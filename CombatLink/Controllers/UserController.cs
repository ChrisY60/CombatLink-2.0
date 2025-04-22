using CombatLink.Application.ViewModels;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using CombatLink.Web.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace CombatLink.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISportService _sportsService;
        private readonly ILanguageService _languageService;
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly IMatchmakingService _matchmakingService;
        private readonly IAuthService _authService;
        private readonly IBlobService _blobService;


        public UserController(IUserService userService, ISportService sportsService, ILanguageService languageService, IUserPreferenceService userPreferenceService, IMatchmakingService matchmakingService, IAuthService authService, IBlobService blobService)
        {
            _sportsService = sportsService;
            _userService = userService;
            _languageService = languageService;
            _userPreferenceService = userPreferenceService;
            _matchmakingService = matchmakingService;
            _authService = authService;
            _blobService = blobService;
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
            }
            else
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
            int? id = await _userService.LogInUserAsync(model.Email, model.Password);
            if (id.HasValue)
            {
                await _authService.SignInAsync(HttpContext, id.Value, model.Email);
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

            UserProfileViewModel model = new UserProfileViewModel{FirstName = user.FirstName,LastName = user.LastName,DateOfBirth = user.DateOfBirth,Weight = user.Weight,Height = user.Height,MonthsOfExperience = user.MonthsOfExperience,AvailableSports = allSports,AvailableLanguages = allLanguages,SelectedSportIds = selectedSports.Select(s => s.Id).ToList(),SelectedLanguageIds = selectedLanguages.Select(l => l.Id).ToList(), ProfilePictureURL = user.ProfilePictureURL};

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
            string? profilePictureUrl = null;

            if (model.ProfilePicture != null)
            {
                profilePictureUrl = await _blobService.UploadImageAsync(model.ProfilePicture);
            }

            bool isUpdated = await _userService.UpdateUserProfile(userId, model.FirstName, model.LastName, model.DateOfBirth,model.Weight,model.Height,model.MonthsOfExperience,profilePictureUrl);
            bool sportsAdded = await _sportsService.AddSportsToUserAsync(userId, model.SelectedSportIds);
            bool languagesAdded = await _languageService.AddLanguagesToUserAsync(userId, model.SelectedLanguageIds);

            if (isUpdated && sportsAdded && languagesAdded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Failed to update profile completely.");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ManagePreference()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var preference = await _userPreferenceService.GetByUserIdAsync(userId);

            if (preference == null)
            {
                preference = new UserPreference();
            }

            var viewModel = new UpdateUserPreferencesViewModel{WeightMin = preference.WeightMin,WeightMax = preference.WeightMax,HeightMin = preference.HeightMin,HeightMax = preference.HeightMax,ExperienceMin = preference.ExperienceMin,ExperienceMax = preference.ExperienceMax};

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManagePreference(UpdateUserPreferencesViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var preference = new UserPreference{WeightMin = model.WeightMin,WeightMax = model.WeightMax,HeightMin = model.HeightMin,HeightMax = model.HeightMax,ExperienceMin = model.ExperienceMin,ExperienceMax = model.ExperienceMax, RelatedUser = new User { Id = userId }};
            await _userPreferenceService.CreateOrUpdateAsync(preference);

            return RedirectToAction("Index", "Home");
        }


    }
}
