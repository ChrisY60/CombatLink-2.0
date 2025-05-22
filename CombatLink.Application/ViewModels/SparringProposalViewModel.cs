using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.ViewModels
{
    public class SparringProposalViewModel
    {
        public int ChallengerUserId { get; set; }
        public int ChallengedUserId { get; set; }
        public int SportId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime TimeOfSession { get; set; }
    }
}
