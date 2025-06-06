﻿using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.Models
{
    public class ChatSummary
    {
        public int MatchId { get; set; }
        public User User { get; set; }
        public string LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }

}
