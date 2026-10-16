namespace RaceDay.Models
{
    public class EventBanner
    {
        public int BannerID { get; set; }
        public int EventID{get;set;}
    public string ImageURL { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
        public Event? Event { get; set; }
    }
}

