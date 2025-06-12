using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.ViewModels
{
    public class SparringUpcomingViewModel
    {
        public List<SparringSessionProposal> UpcomingSessions = new List<SparringSessionProposal>();
        public int CurrentUserId;
    }
}
