using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipPurchaseRefreshIAPResponseData
    {
        [JsonProperty(PropertyName = "NewInventoryChangesAvailable")]
        public bool NewInventoryChangesAvailable { get; set; }
    }
}