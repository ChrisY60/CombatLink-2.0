using CombatLink.Domain.Models;

namespace CombatLink.Domain.IRepositories
{
    public interface ISportRepository
    {
        public Task<bool> CreateSport(Sport sport);
        public Task<Sport> GetSportById(int sportId);
        public Task<IEnumerable<Sport>> GetAllSports();
        public Task<IEnumerable<Sport>> GetSportsByUserId(int userId);
        public Task<IEnumerable<User>> GetUsersBySportId(int sportId);



    }
}
