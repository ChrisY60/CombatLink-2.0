using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.ViewModels
{
    public class IndexViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public string Message { get; set; }
    }
}
