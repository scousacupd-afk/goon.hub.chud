using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class BanInfo
    {
        [JsonProperty(PropertyName = "BanMessage")]
        public string BanMessage { get; set; } = "";
        
        [JsonProperty(PropertyName = "BanExpirationTime")]
        public string BanExpirationTime { get; set; } = "";
    }
}