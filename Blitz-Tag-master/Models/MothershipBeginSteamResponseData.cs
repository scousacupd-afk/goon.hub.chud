using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipBeginSteamResponseData
    {
        [JsonProperty(PropertyName = "Nonce")]
        public string Nonce { get; set; } = "";
    }
}