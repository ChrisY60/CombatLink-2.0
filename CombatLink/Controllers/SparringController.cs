using CombatLink.Application.ViewModels;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                IsAccepted = false
            };

            try
            {
                var success = await _sparringService.AddAsync(proposal);
                Console.WriteLine("here");
                return success ? Ok("Proposal sent.") : StatusCode(500, "Failed to send proposal.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("here");
                return BadRequest(ex.Message);
            }
        }
    }

}