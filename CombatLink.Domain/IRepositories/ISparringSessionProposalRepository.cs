using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.IRepositories
{
    public interface ISparringSessionProposalRepository
    {
        Task<bool> AddAsync(SparringSessionProposal proposal);
        Task<IEnumerable<SparringSessionProposal>> GetByUserIdAsync(int userId);
        Task<SparringSessionProposal?> GetByIdAsync(int id);
        Task<IEnumerable<SparringSessionProposal>> GetByTwoUserIdsAsync(int userId1, int userId2);
        Task<bool> UpdateAsync(SparringSessionProposal proposal);
        Task<bool> DeleteAsync(int id);

    }
}
