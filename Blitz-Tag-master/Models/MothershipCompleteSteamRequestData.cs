using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipCompleteSteamRequestData
    {
        [JsonProperty(PropertyName = "SteamTicket")]
        public string SteamTicket { get; set; } = "";
    }
}