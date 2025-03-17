using System.ComponentModel.DataAnnotations;

namespace CombatLink.ViewModels
{
    public class UserProfileViewModel 
    { 
    
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public int? MonthsOfExperience { get; set; }

    }
}
