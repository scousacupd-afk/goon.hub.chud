using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class VODSchedule
    {
        [JsonProperty("hourly")]
        public VODHourlyStream[] Hourly { get; set; } = [];
    }
}