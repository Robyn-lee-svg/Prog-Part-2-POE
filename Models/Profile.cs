namespace RaceDay.Models
{
    public class Profile
    {
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureURL { get; set; }
        public User? User { get; set; }
    }
}
