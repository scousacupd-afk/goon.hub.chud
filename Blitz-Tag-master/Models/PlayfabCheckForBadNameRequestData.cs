using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class PlayfabCheckForBadNameRequestData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("forRoom")]
        public bool ForRoom { get; set; } = false;

        [JsonProperty("forTroop")]
        public bool ForTroop { get; set; } = false;
    }
}
