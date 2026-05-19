using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class VODStream
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("url")]
        public string Url { get; set; } = "";

        [JsonProperty("type")]
        public VODStreamType Type { get; set; } = VODStreamType.VIDEO;

        [JsonProperty("duration")]
        public int Duration { get; set; }

        public enum VODStreamType
        {
            VIDEO,
            IMAGE
        }
    }
}