using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipCompleteResponseData
    {
        [JsonProperty(PropertyName = "ExternalProviderId")]
        public string ExternalProviderId { get; set; } = "";

        [JsonProperty(PropertyName = "ExternalProviderUsername")]
        public string? ExternalProviderUsername { get; set; } = "";

        [JsonProperty(PropertyName = "IsPrimaryId")]
        public bool IsPrimaryId { get; set; }

        [JsonProperty(PropertyName = "PlayerId")]
        public string PlayerId { get; set; } = "";

        [JsonProperty(PropertyName = "Tags")]
        public object? Tags { get; set; } = null;

        [JsonProperty(PropertyName = "Token")]
        public string Token { get; set; } = "";

        [JsonProperty(PropertyName = "ExpirationTime")]
        public long ExpirationTime { get; set; }
    }
}