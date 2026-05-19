using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
	public class PlayfabAuthResponseData
	{
        [JsonProperty(PropertyName = "SessionTicket")]
		public string SessionTicket { get; set; } = "";
		
        [JsonProperty(PropertyName = "EntityToken")]
		public string EntityToken { get; set; } = "";
		
        [JsonProperty(PropertyName = "PlayFabId")]
		public string PlayFabId { get; set; } = "";
		
        [JsonProperty(PropertyName = "EntityId")]
		public string EntityId { get; set; } = "";
		
        [JsonProperty(PropertyName = "EntityType")]
		public string EntityType { get; set; } = "";

        [JsonProperty(PropertyName = "AccountCreationIsoTimestamp")]
		public DateTime? AccountCreationIsoTimestamp { get; set; }

        [JsonProperty(PropertyName = "FriendCode")]
        public string? FriendCode { get; set; }
    }
}