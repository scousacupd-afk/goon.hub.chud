using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipBeginQuestResponseData
    {
        [JsonProperty(PropertyName = "AttestationNonce")]
        public string AttestationNonce { get; set; } = "";
    }
}