using CombatLink.Application.DTOs;
using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.Mappers
{
    public static class DTOsMapper
    {
        public static UserPreferenceDTO ToDTO(UserPreference preference)
        {
            return new UserPreferenceDTO
            {
                Id = preference.Id,
                UserId = preference.RelatedUser?.Id ?? 0,
                WeightMin = preference.WeightMin,
                WeightMax = preference.WeightMax,
                HeightMin = preference.HeightMin,
                HeightMax = preference.HeightMax,
                ExperienceMin = preference.ExperienceMin,
                ExperienceMax = preference.ExperienceMax
            };
        }

        public static UserPreference ToEntity(UserPreferenceDTO dto, User user)
        {
            return new UserPreference
            {
                Id = dto.Id,
                RelatedUser = user,
                WeightMin = dto.WeightMin,
                WeightMax = dto.WeightMax,
                HeightMin = dto.HeightMin,
                HeightMax = dto.HeightMax,
                ExperienceMin = dto.ExperienceMin,
                ExperienceMax = dto.ExperienceMax
            };
        }
    }
}
