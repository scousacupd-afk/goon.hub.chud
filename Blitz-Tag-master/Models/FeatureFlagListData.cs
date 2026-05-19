using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class FeatureFlagListData
    {
        [JsonProperty(PropertyName = "flags")]
        public List<FeatureFlagData> Flags { get; set; } = [];
    }
}