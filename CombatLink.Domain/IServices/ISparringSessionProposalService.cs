using CombatLink.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CombatLink.Domain.IServices
{
    public interface ISparringSessionProposalService
    {
        Task<bool> AddAsync(SparringSessionProposal proposal);
        Task<SparringSessionProposal?> GetByIdAsync(int id);
        Task<IEnumerable<SparringSessionProposal>> GetByUserIdAsync(int userId);
        Task<IEnumerable<SparringSessionProposal>> GetByTwoUserIdsAsync(int user1Id, int user2Id);
        Task<bool> UpdateAsync(SparringSessionProposal proposal);
        Task<bool> DeleteAsync(int id);
    }
}
