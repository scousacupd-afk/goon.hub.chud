using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class FeatureFlagData
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";
        
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
        
        [JsonProperty(PropertyName = "valueType")]
        public string ValueType { get; set; } = "";

        [JsonProperty(PropertyName = "alwaysOnForUsers")]
        public List<string> AlwaysOnForUsers { get; set; } = [];
    }
}