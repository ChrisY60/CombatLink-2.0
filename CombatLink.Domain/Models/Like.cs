using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Domain.Models
{
    public class Like
    {
        public int LikerUserId { get; set; }
        public int LikedUserId { get; set; }
        public DateTime TimeOfLike { get; set; }
    }
}
