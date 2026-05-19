using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipErrorMessageResponseData
    {
        [JsonProperty(PropertyName = "MothershipErrorCode")]
        public int? MothershipErrorCode { get; set; }

        [JsonProperty(PropertyName = "ClientMessage")]
        public string ClientMessage { get; set; } = "";

        [JsonProperty(PropertyName = "TraceId")]
        public string? TraceId { get; set; } = "";
    }
}