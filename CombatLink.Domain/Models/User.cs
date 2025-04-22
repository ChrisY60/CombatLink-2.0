namespace CombatLink.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public int? MonthsOfExperience { get; set; }
        public string? ProfilePictureURL { get; set; }
        public ICollection<Sport> Sports { get; set; } = new List<Sport>();
        public ICollection<Language> Languages { get; set; } = new List<Language>();

    }
}
