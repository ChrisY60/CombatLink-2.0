using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.Models
{
    public class SparringSessionProposal
    {
        public int Id { get; set; }
        public int ChallengerUserId { get; set; }
        public int ChallengedUserId { get; set; }
        public DateTime TimeProposed { get; set; }
        public int SportId { get; set; }
        public decimal Longtitude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime TimeOfSession { get; set; }
        public bool IsAccepted { get; set; }

        public User ChallengerUser = new();
        public User ChallengedUser = new();
        public Sport RelatedSport = new();
    }
}
