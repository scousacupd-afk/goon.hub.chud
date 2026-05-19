using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipErrorResponseData
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = "";

        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }
    }
}