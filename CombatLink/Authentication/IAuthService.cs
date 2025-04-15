namespace CombatLink.Web.Authentication
{
    public interface IAuthService
    {
        Task SignInAsync(HttpContext httpContext, int userId, string email);
        Task SignOutAsync(HttpContext httpContext);

    }
}
