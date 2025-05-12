using System;
using System.ComponentModel.DataAnnotations;

namespace CombatLink.Application.ViewModels
{
    public class UpdateUserPreferencesViewModel : IValidatableObject
    {
        [Range(30, 200, ErrorMessage = "Minimum weight must be between 30 and 200 kg.")]
        public decimal? WeightMin { get; set; }

        [Range(30, 200, ErrorMessage = "Maximum weight must be between 30 and 200 kg.")]
        public decimal? WeightMax { get; set; }

        [Range(100, 250, ErrorMessage = "Minimum height must be between 100 and 250 cm.")]
        public decimal? HeightMin { get; set; }

        [Range(100, 250, ErrorMessage = "Maximum height must be between 100 and 250 cm.")]
        public decimal? HeightMax { get; set; }

        [Range(0, 480, ErrorMessage = "Minimum experience must be between 0 and 480 months.")]
        public int? ExperienceMin { get; set; }

        [Range(0, 1000, ErrorMessage = "Maximum experience must be between 0 and 1000 months.")]
        public int? ExperienceMax { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WeightMin.HasValue && WeightMax.HasValue && WeightMin > WeightMax)
            {
                yield return new ValidationResult("Maximum weight must be greater than or equal to minimum weight.", new[] { nameof(WeightMax) });
            }

            if (HeightMin.HasValue && HeightMax.HasValue && HeightMin > HeightMax)
            {
                yield return new ValidationResult("Maximum height must be greater than or equal to minimum height.", new[] { nameof(HeightMax) });
            }

            if (ExperienceMin.HasValue && ExperienceMax.HasValue && ExperienceMin > ExperienceMax)
            {
                yield return new ValidationResult("Maximum experience must be greater than or equal to minimum experience.", new[] { nameof(ExperienceMax) });
            }
        }
    }
}
