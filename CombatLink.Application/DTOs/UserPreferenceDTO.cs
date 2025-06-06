﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Application.DTOs
{
    public class UserPreferenceDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal? WeightMin { get; set; }
        public decimal? WeightMax { get; set; }

        public decimal? HeightMin { get; set; }
        public decimal? HeightMax { get; set; }

        public int? ExperienceMin { get; set; }
        public int? ExperienceMax { get; set; }

    }
}
