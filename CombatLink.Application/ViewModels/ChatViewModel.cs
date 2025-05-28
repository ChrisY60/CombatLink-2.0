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
        public int CurrentUserId { get; set; }
        public int OtherUserId { get; set; }
        public User? OtherUserObj { get; set; } = new User();
        public IEnumerable<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public IEnumerable<SparringSessionProposal> Proposals { get; set; } = new List<SparringSessionProposal>();
        public IEnumerable<ChatItem> ChatItems { get; set; } = new List<ChatItem>();
    }

}
