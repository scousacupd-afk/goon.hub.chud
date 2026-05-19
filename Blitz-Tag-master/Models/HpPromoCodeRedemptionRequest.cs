using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class HpPromoCodeRedemptionRequest : MothershipRequestBase
    {
        [JsonProperty(PropertyName = "itemGuid")]
        public string ItemGUID { get; set; } = "";

        [JsonProperty(PropertyName = "playFabID")]
        public string PlayFabID { get; set; } = "";

        [JsonProperty(PropertyName = "playFabSessionTicket")]
        public string PlayFabSessionTicket { get; set; } = "";
    }
}