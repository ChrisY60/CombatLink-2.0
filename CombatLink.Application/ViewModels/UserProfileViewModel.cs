using CombatLink.Domain.Models;
namespace CombatLink.Application.ViewModels
{
    public class UserProfileViewModel
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public int? MonthsOfExperience { get; set; }
        public List<int> SelectedSportIds { get; set; } = new();
        public List<int> SelectedLanguageIds { get; set; } = new();
        public List<Sport> AvailableSports { get; set; } = new();
        public List<Language> AvailableLanguages { get; set; } = new();


    }
}
