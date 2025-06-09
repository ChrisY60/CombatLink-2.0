using CombatLink.Application.ViewModels;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace CombatLink.Web.Controllers
{
    [Authorize]
    public class SparringController : Controller
    {
        private readonly ISparringSessionProposalService _sparringService;

        public SparringController(ISparringSessionProposalService sparringService)
        {
            _sparringService = sparringService;
        }

        [HttpPost]
        public async Task<IActionResult> Propose([FromBody] SparringProposalViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var proposal = new SparringSessionProposal
            {
                ChallengerUserId = model.ChallengerUserId,
                ChallengedUserId = model.ChallengedUserId,
                SportId = model.SportId,
                TimeProposed = DateTime.UtcNow,
                TimeOfSession = model.TimeOfSession,
                Longtitude = model.Longitude,
                Latitude = model.Latitude,
                Status = ProposalStatus.Pending
            };

            try
            {
                var success = await _sparringService.AddAsync(proposal);
                return success ? Ok("Proposal sent.") : StatusCode(500, "Failed to send proposal.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var proposal = await _sparringService.GetByIdAsync(id);
            if (proposal == null)
                return NotFound();

            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["CurrentUserId"] = currentUserId;

            return PartialView("_SparringProposalDetails", proposal);

        }

        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var proposal = await _sparringService.GetByIdAsync(id);
            if (proposal == null)
                return NotFound("Proposal not found.");

            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (proposal.ChallengedUserId != currentUserId)

            return StatusCode(403, "Only the challenged user can accept the proposal.");

            var result = await _sparringService.AcceptProposalAsync(id);
            if (!result)
                return StatusCode(500, "Failed to accept proposal.");

            return Ok("Proposal accepted.");
        }

        [HttpPost]
        public async Task<IActionResult> Decline(int id)
        {
            var proposal = await _sparringService.GetByIdAsync(id);
            if (proposal == null)
                return NotFound("Proposal not found.");

            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (proposal.ChallengedUserId != currentUserId)
                StatusCode(403, "Only the challenged user can decline the proposal.");

            var result = await _sparringService.DeclineProposalAsync(id);
            if (!result)
                return StatusCode(500, "Failed to decline proposal.");

            return Ok("Proposal declined.");
        }



    }
}
