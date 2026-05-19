using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class SAnnouncementData
    {
        [JsonProperty(PropertyName = "ShowAnnouncement")]
        public string ShowAnnouncement { get; set; } = "";

        [JsonProperty(PropertyName = "AnnouncementID")]
        public string AnnouncementID { get; set; } = "";

        [JsonProperty(PropertyName = "AnnouncementTitle")]
        public string AnnouncementTitle { get; set; } = "";

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; } = "";
    }
}