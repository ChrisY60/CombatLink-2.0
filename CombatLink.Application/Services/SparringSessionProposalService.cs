using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CombatLink.Application.Services
{
    public class SparringSessionProposalService : ISparringSessionProposalService
    {
        private readonly ISparringSessionProposalRepository _repository;
        private readonly ISportService _sportService;

        public SparringSessionProposalService(ISparringSessionProposalRepository repository, ISportService sportService)
        {
            _repository = repository;
            _sportService = sportService;
        }

        public async Task<bool> AddAsync(SparringSessionProposal proposal)
        {
            return await _repository.AddAsync(proposal);
        }

        public async Task<SparringSessionProposal?> GetByIdAsync(int id)
        {
            var proposal = await _repository.GetByIdAsync(id);
            if (proposal == null)
                return null;

            var sport = await _sportService.GetSportByIdAsync(proposal.SportId);
            proposal.RelatedSport = sport;
            return proposal;
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetByUserIdAsync(int userId)
        {
            var proposals = (await _repository.GetByUserIdAsync(userId)).ToList();
            var allSports = (await _sportService.GetAllSportsAsync()).ToDictionary(s => s.Id);

            foreach (var proposal in proposals)
            {
                if (allSports.TryGetValue(proposal.SportId, out var sport))
                {
                    proposal.RelatedSport = sport;
                }
            }

            return proposals;
        }

        public async Task<bool> UpdateAsync(SparringSessionProposal proposal)
        {
            var success = await _repository.UpdateAsync(proposal);
            if (!success) return false;

            // Enrich after update if needed
            var sport = await _sportService.GetSportByIdAsync(proposal.SportId);
            proposal.RelatedSport = sport;
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetByTwoUserIdsAsync(int user1Id, int user2Id)
        {
            var proposals = (await _repository.GetByTwoUserIdsAsync(user1Id, user2Id)).ToList();
            var allSports = (await _sportService.GetAllSportsAsync()).ToDictionary(s => s.Id);

            foreach (var proposal in proposals)
            {
                if (allSports.TryGetValue(proposal.SportId, out var sport))
                {
                    proposal.RelatedSport = sport;
                }
            }

            return proposals;
        }

    }
}
