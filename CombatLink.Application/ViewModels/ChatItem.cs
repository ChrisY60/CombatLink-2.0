using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.ViewModels
{
    public class ChatItem
    {
        public ChatMessage? Message { get; set; }
        public SparringSessionProposal? Proposal { get; set; }
        public DateTime Time { get; set; }
    }

}
