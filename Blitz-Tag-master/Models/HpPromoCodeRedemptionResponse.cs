using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class HpPromoCodeRedemptionResponse
    {
        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; } = "";

        [JsonProperty(PropertyName = "itemID")]
        public string ItemID { get; set; } = "";

        [JsonProperty(PropertyName = "playFabItemName")]
        public string PlayFabItemName { get; set; } = "";

        [JsonProperty(PropertyName = "startTime")]
        public DateTimeOffset? StartTime { get; set; }

        [JsonProperty(PropertyName = "endTime")]
        public DateTimeOffset? EndTime { get; set; }
    }
}