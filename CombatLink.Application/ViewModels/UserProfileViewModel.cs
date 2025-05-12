using CombatLink.Domain.Models;
using System.Collections.Generic;

namespace CombatLink.Application.ViewModels
{
    public class UserProfileViewModel
    {
        public string? ProfilePictureURL { get; set; }

        public List<Sport> AvailableSports { get; set; } = new();
        public List<Language> AvailableLanguages { get; set; } = new();

        public UpdateUserProfileManagementViewModel Form { get; set; } = new();
    }
}
