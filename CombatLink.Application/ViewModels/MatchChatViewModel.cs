using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.ViewModels
{
    public class MatchChatViewModel
    {
        public List<User> MatchedUsers { get; set; }
        public List<ChatSummary> Chats { get; set; }
    }
}
