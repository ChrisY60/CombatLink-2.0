using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public Match RelatedMatch { get; set; }
        public User Sender { get; set; }

        private string _messageContent = string.Empty;
        public string MessageContent
        {
            get { return _messageContent; }
            set
            {
                if (value.Length > 500)
                {
                    _messageContent = value.Substring(0, 500);
                }
                else
                {
                    _messageContent = value;
                }
            }
        }
        public DateTime TimeSent { get; set; }
        public bool SeenByReceiver { get; set; }
    }
}
