using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipUserDataGetResponseData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "metadata_id")]
        public string MetadataId { get; set; } = "";

        [JsonProperty(PropertyName = "key_name")]
        public string KeyName { get; set; } = "";

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "";

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; } = "";

        [JsonProperty(PropertyName = "generation")]
        public int Generation { get; set; }

        [JsonProperty(PropertyName = "created_by")]
        public string CreatedBy { get; set; } = "";

        [JsonProperty(PropertyName = "last_written_by")]
        public string LastWrittenBy { get; set; } = "";

        [JsonProperty("created_time", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("last_updated_time", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime LastUpdatedTime { get; set; }
    }
}
