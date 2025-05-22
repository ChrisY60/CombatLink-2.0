using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CombatLink.Application.Services
{
    public class SparringSessionProposalService : ISparringSessionProposalService
    {
        private readonly ISparringSessionProposalRepository _repository;

        public SparringSessionProposalService(ISparringSessionProposalRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddAsync(SparringSessionProposal proposal)
        {
            return await _repository.AddAsync(proposal);
        }

        public async Task<SparringSessionProposal?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetByUserIdAsync(int userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<bool> UpdateAsync(SparringSessionProposal proposal)
        {
            return await _repository.UpdateAsync(proposal);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
