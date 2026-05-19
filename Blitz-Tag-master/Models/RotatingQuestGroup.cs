using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class QuestGroup
    {
        [JsonProperty(PropertyName = "selectCount")]
        public int SelectCount { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";

        [JsonProperty(PropertyName = "quests")]
        public List<RotatingQuest> Quests { get; set; } = [];
    }
}
