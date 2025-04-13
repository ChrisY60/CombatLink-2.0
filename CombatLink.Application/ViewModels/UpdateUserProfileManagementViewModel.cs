using System.ComponentModel.DataAnnotations;

namespace CombatLink.Application.ViewModels
{
    public class UpdateUserProfileManagementViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public decimal Weight { get; set; }

        [Required]
        public decimal Height { get; set; }

        [Required]
        public int MonthsOfExperience { get; set; }

        public List<int> SelectedSportIds { get; set; } = new List<int>();
        public List<int> SelectedLanguageIds { get; set; } = new List<int>();


    }
}
