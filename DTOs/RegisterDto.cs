using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Organiser|Participant)$")] 
        public string Role { get; set; } = "Participant";
        public string? Phone { get; set; }



    }
}
