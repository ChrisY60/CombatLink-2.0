namespace CombatLink.Repositories.IRepositories
{
    public interface IUserRepository
    {
        public Task<bool> RegisterUserAsync(string email, string passwordHash);

        public Task<int?> LogInUserAsync(string email, string passwordHash);
    }
}
