using CombatLinkMVC.Models;

namespace CombatLink.Services.IServices
{
    public interface ISportService
    {
        Task<bool> CreateSportAsync(Sport sport);
        Task<Sport?> GetSportByIdAsync(int sportId);
        Task<IEnumerable<Sport>> GetAllSportsAsync();
        Task<IEnumerable<Sport>> GetSportsByUserIdAsync(int userId);
        Task<IEnumerable<User>> GetUsersBySportIdAsync(int sportId);
        Task<bool> AddSportsToUserAsync(int userId, List<int> sportIds);
    }
}
