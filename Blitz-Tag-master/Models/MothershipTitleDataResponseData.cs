using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipTitleDataResponseData
    {
        [JsonProperty(PropertyName = "Results")]
        public List<MothershipTitleDataResponseKey> Results { get; set; } = [];
    }

    public class MothershipTitleDataResponseKey
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; } = "";

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; } = "";
    }
}