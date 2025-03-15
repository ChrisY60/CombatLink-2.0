using CombatLink.Services.IServices;
using CombatLink.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace CombatLink.Controllers
{
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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //add encryption (probably in the service is better)
            string passwordHash = model.Password;
            var isRegistered = await _userService.RegisterUserAsync(model.Email, passwordHash);

            if (isRegistered)
            {
                return RedirectToAction("Index", "Home");
            }else
            {
                return View();
            }

        }
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string passwordHash = model.Password;
            int? id = (int)await _userService.LogInUserAsync(model.Email, passwordHash);
            if (id != null)
            {
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                };

                Response.Cookies.Append("UserId", id.ToString(), cookieOptions);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Incorrect credentials");
                return View(model);
            }

        }
        public IActionResult LogOut()
        {
            return RedirectToAction("Index", "Home");
        }

    }   
}
