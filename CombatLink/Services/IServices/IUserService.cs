namespace CombatLink.Services.IServices
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string email, string passwordHash);

        Task<int?> LogInUserAsync(string email, string passwordHash);
    }

}
