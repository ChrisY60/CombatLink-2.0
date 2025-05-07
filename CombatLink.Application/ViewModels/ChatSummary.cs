using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.ViewModels
{
    public class ChatSummary
    {
        public User User { get; set; }
        public string LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }

}
