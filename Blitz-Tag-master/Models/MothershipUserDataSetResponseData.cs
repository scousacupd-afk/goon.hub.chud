using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipUserDataSetResponseData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "";

        [JsonProperty(PropertyName = "key_name")]
        public string KeyName { get; set; } = "";

        [JsonProperty(PropertyName = "generation")]
        public int Generation { get; set; }
    }
}