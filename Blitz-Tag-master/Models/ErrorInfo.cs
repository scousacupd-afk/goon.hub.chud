using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class ErrorInfo
    {
        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; } = "";
        
        [JsonProperty(PropertyName = "Error")]
        public string Error { get; set; } = "";
    }
}