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
        private readonly IUserService _userService;

        public SparringSessionProposalService(ISparringSessionProposalRepository repository, ISportService sportService, IUserService userService)
        {
            _repository = repository;
            _sportService = sportService;
            _userService = userService;
        }

        public async Task<bool> AddAsync(SparringSessionProposal proposal)
        {
            proposal.Status = ProposalStatus.Pending;
            return await _repository.AddAsync(proposal);
        }

        public async Task<SparringSessionProposal?> GetByIdAsync(int id)
        {
            var proposal = await _repository.GetByIdAsync(id);
            if (proposal == null)
                return null;

            var sport = await _sportService.GetSportByIdAsync(proposal.SportId);
            proposal.RelatedSport = sport;
            proposal.ChallengerUser = await _userService.GetUserById(proposal.ChallengerUserId);
            proposal.ChallengedUser = await _userService.GetUserById(proposal.ChallengedUserId);

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
                    proposal.ChallengerUser = await _userService.GetUserById(proposal.ChallengerUserId);
                    proposal.ChallengedUser = await _userService.GetUserById(proposal.ChallengedUserId);

                }
            }

            return proposals;
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
                    proposal.ChallengerUser = await _userService.GetUserById(proposal.ChallengerUserId);
                    proposal.ChallengedUser = await _userService.GetUserById(proposal.ChallengedUserId);
                }
            }

            return proposals;
        }

        public async Task<bool> UpdateAsync(SparringSessionProposal proposal)
        {
            var success = await _repository.UpdateAsync(proposal);
            if (!success) return false;

            var sport = await _sportService.GetSportByIdAsync(proposal.SportId);
            proposal.RelatedSport = sport;
            proposal.ChallengerUser = await _userService.GetUserById(proposal.ChallengerUserId);
            proposal.ChallengedUser = await _userService.GetUserById(proposal.ChallengedUserId);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> AcceptProposalAsync(int proposalId)
        {
            var proposal = await _repository.GetByIdAsync(proposalId);
            if (proposal == null) return false;

            proposal.Status = ProposalStatus.Accepted;
            return await _repository.UpdateAsync(proposal);
        }

        public async Task<bool> DeclineProposalAsync(int proposalId)
        {
            var proposal = await _repository.GetByIdAsync(proposalId);
            if (proposal == null) return false;

            proposal.Status = ProposalStatus.Declined;
            return await _repository.UpdateAsync(proposal);
        }
        public async Task<IEnumerable<SparringSessionProposal>> GetUpcomingSparringSessionsForUserId(int userId)
        {
            var proposals = (await _repository.GetUpcommingSparringsForUserId(userId)).ToList();
            var allSports = (await _sportService.GetAllSportsAsync()).ToDictionary(s => s.Id);

            foreach (var proposal in proposals)
            {
                if (allSports.TryGetValue(proposal.SportId, out var sport))
                {
                    proposal.RelatedSport = sport;
                }
                proposal.ChallengerUser = await _userService.GetUserById(proposal.ChallengerUserId);
                proposal.ChallengedUser = await _userService.GetUserById(proposal.ChallengedUserId);
            }

            return proposals;
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetCompletedSparringSessionsForUserId(int userId)
        {
            var proposals = (await _repository.GetCompletedSparringSessionsForUserId(userId)).ToList();
            var allSports = (await _sportService.GetAllSportsAsync()).ToDictionary(s => s.Id);

            foreach (var proposal in proposals)
            {
                if (allSports.TryGetValue(proposal.SportId, out var sport))
                {
                    proposal.RelatedSport = sport;
                }
                proposal.ChallengerUser = await _userService.GetUserById(proposal.ChallengerUserId);
                proposal.ChallengedUser = await _userService.GetUserById(proposal.ChallengedUserId);
            }

            return proposals;
        }

    }
}
