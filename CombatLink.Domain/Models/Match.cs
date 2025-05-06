using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.Models
{
    public class Match
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime TimeOfMatch { get; set; }
    }
}
