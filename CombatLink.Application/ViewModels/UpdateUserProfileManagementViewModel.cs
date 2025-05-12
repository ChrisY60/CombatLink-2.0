using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CombatLink.Application.ViewModels
{
    public class UpdateUserProfileManagementViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(30, 200, ErrorMessage = "Weight must be between 30 and 200 kg.")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        [Range(100, 250, ErrorMessage = "Height must be between 100 and 250 cm.")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Experience is required.")]
        [Range(0, 480, ErrorMessage = "Experience must be between 0 and 480 months.")]
        public int MonthsOfExperience { get; set; }

        public List<int> SelectedSportIds { get; set; } = new();
        public List<int> SelectedLanguageIds { get; set; } = new();
        public IFormFile? ProfilePicture { get; set; }
    }
}
