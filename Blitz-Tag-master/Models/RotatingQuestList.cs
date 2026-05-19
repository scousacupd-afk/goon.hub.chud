using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class RotatingQuestList
    {
        [JsonProperty(PropertyName = "DailyQuests")]
        public List<QuestGroup> DailyQuests { get; set; } = [];

        [JsonProperty(PropertyName = "WeeklyQuests")]
        public List<QuestGroup> WeeklyQuests { get; set; } = [];
    }
}