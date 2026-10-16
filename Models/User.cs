using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public Profile? Profile { get; set; }

        public ICollection<Event> OrganisedEvents { get; set; } = new List<Event>();

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}
