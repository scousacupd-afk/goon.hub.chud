using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Blitz_Tag.Models
{
    public class VODHourlyStream
    {
        [JsonProperty("stream")]
        public VODStream Stream { get; set; } = new();

        [Range(0f, 59f)]
        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("startDateTime")]
        public string StartDateTime { get; set; } = "";

        [JsonProperty("endDateTime")]
        public string EndDateTime { get; set; } = "";
    }
}