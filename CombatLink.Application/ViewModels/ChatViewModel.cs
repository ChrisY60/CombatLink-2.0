using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.ViewModels
{
    public class ChatViewModel
    {
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public IEnumerable<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

}
