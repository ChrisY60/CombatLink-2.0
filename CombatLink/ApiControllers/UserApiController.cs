using CombatLink.Domain.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CombatLink.Web.ApiControllers
{
    [ApiController]
    [Route("api/user")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var userId = await _userService.LogInUserAsync(request.Email, request.Password);
            if (userId != null)
            {
                return Ok(new { userId });
            }
            return Unauthorized(new { message = "Invalid email or password" });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
        {
            var success = await _userService.VerifyUserAsync(request.UserId);
            if (success)
                return Ok(new { message = "User verified successfully" });
            return BadRequest(new { message = "Verification failed" });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class VerifyRequest
    {
        public int UserId { get; set; }
    }
}
