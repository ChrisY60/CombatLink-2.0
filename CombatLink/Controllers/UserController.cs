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
        public UserController(IUserService userService)
        {
            _userService = userService;
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
            Debug.WriteLine(user.FirstName);
            UserProfileViewModel model = new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Weight = user.Weight,
                Height = user.Height,
                MonthsOfExperience = user.MonthsOfExperience
            };


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageProfile(UpdateUserProfileManagementViewModel model)
        {
            Debug.WriteLine("Got here 1");
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

            if (isUpdated)
            {
                ViewBag.Message = "Profile updated successfully!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View(model);
            }
        }
    }
}
