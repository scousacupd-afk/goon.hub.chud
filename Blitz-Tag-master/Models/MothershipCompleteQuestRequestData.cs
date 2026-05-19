using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipCompleteQuestRequestData
    {
        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; } = "";

        [JsonProperty(PropertyName = "MetaNonce")]
        public string MetaNonce { get; set; } = "";

        [JsonProperty(PropertyName = "AttestationToken")]
        public string AttestationToken { get; set; } = "";
    }
}