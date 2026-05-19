using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipRequestBase
    {
        [JsonProperty(PropertyName = "mothershipId")]
        public string MothershipId { get; set; } = "";

        [JsonProperty(PropertyName = "mothershipToken")]
        public string MothershipToken { get; set; } = "";

        [JsonProperty(PropertyName = "mothershipEnvId")]
        public string MothershipEnvId { get; set; } = "";
    }
}