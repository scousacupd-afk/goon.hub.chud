using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blitz_Tag.Models
{
    public class RotatingQuest
    {
        [JsonProperty(PropertyName = "disable")]
        public bool Disable { get; set; }

        [JsonProperty(PropertyName = "questID")]
        public int QuestId { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }

        [JsonProperty(PropertyName = "category")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QuestCategory Category { get; set; }

        [JsonProperty(PropertyName = "questName")]
        public string QuestName { get; set; } = "UNNAMED QUEST";

        [JsonProperty(PropertyName = "questType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QuestType QuestType { get; set; }

        [JsonProperty(PropertyName = "questOccurenceFilter")]
        public string QuestOccurenceFilter { get; set; } = "";

        [JsonProperty(PropertyName = "requiredOccurenceCount")]
        public int RequiredOccurenceCount { get; set; }

        [JsonProperty(PropertyName = "requiredZones", ItemConverterType = typeof(StringEnumConverter))]
        public List<GTZone> RequiredZones { get; set; } = [];
    }
}
